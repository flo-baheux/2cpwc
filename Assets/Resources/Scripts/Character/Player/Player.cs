using System;
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
  public PlayerAssignment playerAssignment { get; private set; }

  Player()
  {
    playerGroundedState = new PlayerGroundedState(this);
    playerJumpingState = new PlayerJumpingState(this);
    playerDeadState = new PlayerDeadState(this);
    playerClimbingState = new PlayerClimbingState(this);
    health = new PlayerHealthComponent(this);
  }
  public PlayerGroundedState playerGroundedState;
  public PlayerJumpingState playerJumpingState;
  public PlayerDeadState playerDeadState;
  public PlayerClimbingState playerClimbingState;

  public PlayerHealthComponent health;

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
  private Interactable _currentInteractable;
  public GameObject Climbable;

  void Awake()
  {
    rigidBody = GetComponent<Rigidbody2D>();
    capsuleCollider = GetComponent<CapsuleCollider2D>();
    playerInput = GetComponent<PlayerInput>();
    animator = GetComponent<Animator>();
    gameplayManager = GameObject.Find("GameManager").GetComponent<GameplayManager>();
    playerInput.actions["Interact"].performed += OnInteract;

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
      float horizontalVelocity = horizontalInput * runningSpeed;
      if (horizontalVelocity != rigidBody.velocity.x)
        transform.GetChild(0).transform.rotation = Quaternion.Euler(0, horizontalInput > 0 ? 90 : -90, 0);
      rigidBody.velocity = new Vector2(horizontalVelocity, rigidBody.velocity.y);

      //if (Input.GetKeyDown(KeyCode.LeftControl))
      //  TransitionToState(State.DEAD);      
    }

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

    // Reset player health
    // Re-enable controls after X time
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

  private void OnInteract(InputAction.CallbackContext context) =>
    _currentInteractable?.Interact(this);

  private void OnTriggerExit2D(Collider2D other)
  {
    if (other.GetComponent<Interactable>() != null)
      _currentInteractable = null;

    if (other.CompareTag("Climbable"))
      Climbable = null;
  }
}
