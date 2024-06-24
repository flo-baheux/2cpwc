using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerAssignment
{
  Player1,
  Player2,
};

[RequireComponent(typeof(PlayerHealthComponent))]
public class Player : MonoBehaviour
{
  Player()
  {
    playerGroundedState = new PlayerGroundedState(this);
    playerJumpingState = new PlayerJumpingState(this);
    playerDeadState = new PlayerDeadState(this);
    playerClimbingState = new PlayerClimbingState(this);
  }

  public PlayerAssignment playerAssignment { get; private set; }

  // States
  public PlayerGroundedState playerGroundedState { get; private set; }
  public PlayerJumpingState playerJumpingState { get; private set; }
  public PlayerDeadState playerDeadState { get; private set; }
  public PlayerClimbingState playerClimbingState { get; private set; }
  private Dictionary<State, PlayerState> States;
  public PlayerState currentState;

  // Movements
  [SerializeField] public float jumpHeight { get; private set; } = 4f;
  [SerializeField] public float fallSpeed { get; private set; } = 8f;
  [SerializeField] public float runningSpeed { get; private set; } = 12f;
  private bool facingRight = true;
  public bool controlsEnabled = true;

  // Components
  [NonSerialized] public Rigidbody2D rigidBody;
  [NonSerialized] public PlayerInput playerInput;
  public PlayerHealthComponent health { get; private set; }
  private CapsuleCollider2D mainCollider;
  private Animator animator;

  // Gameplay Manager
  private GameplayManager gameplayManager;

  // Events
  public event Action<Checkpoint> OnCheckpointActivated;
  public event Action<Player> OnInteract;
  public event Action<PlayerAssignment, bool> OnInteractionDetected;
  public event Action<PlayerAssignment, bool> OnClimbingDetected;

  private Interactable _currentInteractable;
  public GameObject Climbable { get; private set; }

  void Awake()
  {
    health = GetComponent<PlayerHealthComponent>();
    rigidBody = GetComponent<Rigidbody2D>();
    mainCollider = GetComponent<CapsuleCollider2D>();
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
    Vector2 center = new(mainCollider.bounds.center.x, mainCollider.bounds.min.y);
    Vector2 size = new(mainCollider.bounds.size.x + 0.2f, 0.05f);
    RaycastHit2D raycastHit = Physics2D.BoxCast(center, size, 0f, Vector2.down, 0f, LayerMask.GetMask("Ground"));
    return raycastHit.collider;
  }

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
    {
      _currentInteractable = other.GetComponent<Interactable>();
      OnInteractionDetected?.Invoke(playerAssignment, true);
    }

    if (other.CompareTag("Climbable"))
    {
      Climbable = other.gameObject;
      OnClimbingDetected?.Invoke(playerAssignment, true);
    }

  }

  private void Interact(InputAction.CallbackContext context)
  {
    OnInteract?.Invoke(this);
    _currentInteractable?.Interact(this);
  }

  private void OnTriggerExit2D(Collider2D other)
  {
    if (other.GetComponent<Interactable>() != null)
    {
      _currentInteractable = null;
      OnInteractionDetected?.Invoke(playerAssignment, false);
    }

    if (other.CompareTag("Climbable"))
    {
      Climbable = null;
      OnClimbingDetected?.Invoke(playerAssignment, false);
    }
  }

  public bool canInteractWithSomething() => _currentInteractable != null;
}
