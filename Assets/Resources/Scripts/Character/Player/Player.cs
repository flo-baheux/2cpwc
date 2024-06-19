using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerAssignment
{
  Player1,
  Player2,
};

public class Player : MonoBehaviour
{
  Player()
  {
    playerGroundedState = new PlayerGroundedState(this);
    playerJumpingState = new PlayerJumpingState(this);
    playerDeadState = new PlayerDeadState(this);
    playerClimbingState = new PlayerClimbingState(this);
    health = new PlayerHealthComponent(this);
  }

  public PlayerAssignment playerAssignment { get; private set; }
  public PlayerHealthComponent health { get; private set; }

  public PlayerGroundedState playerGroundedState { get; private set; }
  public PlayerJumpingState playerJumpingState { get; private set; }
  public PlayerDeadState playerDeadState { get; private set; }
  public PlayerClimbingState playerClimbingState { get; private set; }

  // Movement parameters
  public float jumpHeight = 10f;
  public float fallSpeed = 12f;
  public float runningSpeed = 15f;

  [NonSerialized] public Rigidbody2D rigidBody;
  [NonSerialized] public PlayerInput playerInput;
  private CapsuleCollider2D capsuleCollider;
  private Animator animator;
  private GameplayManager gameplayManager;

  public PlayerState currentState;

  public bool controlsEnabled = true;
  private Dictionary<State, PlayerState> States;

  public event Action<Checkpoint> OnCheckpointActivated;
  public event Action<Player> OnInteract;

  private Interactable _currentInteractable;
  public GameObject Climbable;

  private bool facingRight = true;

  [SerializeField] public readonly int damageInvulnSecondsDuration = 1;
  public bool isInvulnerable { get; private set; }

  void Awake()
  {
    rigidBody = GetComponent<Rigidbody2D>();
    capsuleCollider = GetComponent<CapsuleCollider2D>();
    playerInput = GetComponent<PlayerInput>();
    animator = GetComponent<Animator>();
    gameplayManager = GameObject.Find("GameManager").GetComponent<GameplayManager>();
    playerInput.actions["Interact"].performed += Interact;

    States = new Dictionary<State, PlayerState>() {
      {State.GROUNDED, playerGroundedState},
      {State.JUMPING, playerJumpingState},
      {State.DEAD, playerDeadState},
      {State.CLIMBING, playerClimbingState}
    };
    currentState = States[State.GROUNDED];

    health.PlayerHealthChanged += OnHealthChange;
  }

  public void SetPlayerAssignment(PlayerAssignment pa)
  {
    playerAssignment = pa;
    playerInput.SwitchCurrentActionMap(playerAssignment == PlayerAssignment.Player1 ? "Player1" : "Player2");
  }

  void Update()
  {
    float horizontalInput = playerInput.actions["Move"].ReadValue<float>();
    if (controlsEnabled)
    {
      if (horizontalInput > 0)
        facingRight = true;
      else if (horizontalInput < 0)
        facingRight = false;
      float horizontalVelocity = horizontalInput * runningSpeed;
      rigidBody.velocity = new Vector2(horizontalVelocity, rigidBody.velocity.y);
    }

    transform.GetChild(0).transform.rotation = Quaternion.Euler(0, facingRight ? 90 : -90, 0);

    State? newState = currentState.CustomUpdate();
    if (newState.HasValue)
      TransitionToState(newState.Value);

    animator.SetFloat("HorizontalInput", Math.Abs(horizontalInput));
    animator.SetFloat("VelocityY", rigidBody.velocity.y);
    animator.SetBool("IsGrounded", currentState.state == State.GROUNDED);
    animator.SetBool("IsCrouched", currentState.state == State.CROUCHING);
    animator.SetBool("IsDead", currentState.state == State.DEAD);
    if (playerInput.actions["Pause"].WasPressedThisFrame())
      gameplayManager.PauseResumeGame();
  }

  public bool IsGrounded()
  {
    Vector2 center = new(capsuleCollider.bounds.center.x, capsuleCollider.bounds.min.y);
    Vector2 size = new(capsuleCollider.bounds.size.x + 0.2f, 0.05f);
    RaycastHit2D raycastHit = Physics2D.BoxCast(center, size, 0f, Vector2.down, 0f, LayerMask.GetMask("Ground"));
    return raycastHit.collider;
  }

  // // Debug IsGrounded box collider
  // public void OnDrawGizmos()
  // {
  //   Gizmos.DrawCube(new Vector2(capsuleCollider.bounds.center.x, capsuleCollider.bounds.min.y), new Vector2(capsuleCollider.bounds.size.x + 0.2f, 0.05f));
  // }

  public void TransitionToState(State newState)
  {
    currentState.Exit();
    currentState = States[newState];
    currentState.Enter();
  }

  public void RespawnToPosition(Vector2 Checkpoint)
  {
    rigidBody.position = Checkpoint;
    TransitionToState(State.GROUNDED);

    health.ResetHealth();
    controlsEnabled = true;
  }

  public void OnTriggerEnter2D(Collider2D other)
  {
    if (other.gameObject.TryGetComponent(out Checkpoint checkpoint))
      OnCheckpointActivated?.Invoke(checkpoint);

    if (other.CompareTag("Interactable"))
      _currentInteractable = other.GetComponent<Interactable>();

    if (other.CompareTag("Climbable"))
      Climbable = other.gameObject;
  }

  private void Interact(InputAction.CallbackContext context)
  {
    OnInteract?.Invoke(this);
    _currentInteractable?.Interact(this);
  }

  private void OnTriggerExit2D(Collider2D other)
  {
    if (other.GetComponent<Interactable>() != null)
      _currentInteractable = null;

    if (other.CompareTag("Climbable"))
      Climbable = null;
  }

  public bool canInteractWithSomething() => _currentInteractable != null;

  public void OnHealthChange(int hpBefore, int hpAfter)
  {
    if (hpAfter < hpBefore && hpAfter > 0)
      StartCoroutine(InvulnerabilityCoroutine());
  }

  IEnumerator InvulnerabilityCoroutine()
  {
    isInvulnerable = true;
    yield return new WaitForSecondsRealtime(damageInvulnSecondsDuration);
    isInvulnerable = false;
  }
}
