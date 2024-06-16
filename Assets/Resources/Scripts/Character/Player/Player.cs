using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
  Player()
  {
    playerGroundedState = new PlayerGroundedState(this);
    playerJumpingState = new PlayerJumpingState(this);
    playerDeadState = new PlayerDeadState(this);
    health = new PlayerHealthComponent(this);
  }
  public PlayerGroundedState playerGroundedState;
  public PlayerJumpingState playerJumpingState;
  public PlayerDeadState playerDeadState;

  public PlayerHealthComponent health;

  // Movement parameters
  public float jumpHeight = 10f;
  public float fallSpeed = 12f;
  public float runningSpeed = 15f;

  [NonSerialized] public Rigidbody2D rigidBody;
  [NonSerialized] public PlayerInput playerInput;
  private CapsuleCollider2D capsuleCollider;


  public bool controlsEnabled = true;

  private Dictionary<State, PlayerState> States;

  public PlayerState currentState;

  public event Action<Checkpoint> OnCheckpointActivated;

  private Interactable _currentInteractable;


  void Awake()
  {
    rigidBody = GetComponent<Rigidbody2D>();
    capsuleCollider = GetComponent<CapsuleCollider2D>();
    playerInput = GetComponent<PlayerInput>();
    playerInput.actions["Interact"].performed += OnInteract;

    States = new Dictionary<State, PlayerState>() {
      {State.GROUNDED, playerGroundedState},
      {State.JUMPING, playerJumpingState},
      {State.DEAD, playerDeadState},
    };
    currentState = States[State.GROUNDED];
  }

  void Update()
  {
    if (controlsEnabled)
    {
      float horizontalInput = playerInput.actions["Move"].ReadValue<float>();

      float horizontalVelocity = horizontalInput * runningSpeed; ;
      rigidBody.velocity = new Vector2(horizontalVelocity, rigidBody.velocity.y);
    }


    State? newState = currentState.CustomUpdate();
    if (newState.HasValue)
      TransitionToState(newState.Value);
  }

  public bool IsGrounded()
  {
    Vector2 center = new(capsuleCollider.bounds.center.x, capsuleCollider.bounds.min.y);
    Vector2 size = new(capsuleCollider.bounds.size.x + 0.2f, 0.05f);
    RaycastHit2D raycastHit = Physics2D.BoxCast(center, size, 0f, Vector2.down, 1f, LayerMask.GetMask("Ground"));
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

    // Reset player health
    // Re-enable controls after X time
  }

  public void OnTriggerEnter2D(Collider2D other)
  {
    if (other.gameObject.TryGetComponent(out Checkpoint checkpoint))
      OnCheckpointActivated?.Invoke(checkpoint);

    if (other.CompareTag("Interactable"))
      _currentInteractable = other.GetComponent<Interactable>();
  }

  private void OnInteract(InputAction.CallbackContext context) =>
    _currentInteractable?.Interact();

  private void OnTriggerExit2D(Collider2D other)
  {
    if (other.GetComponent<Interactable>() != null)
      _currentInteractable = null;
  }
}
