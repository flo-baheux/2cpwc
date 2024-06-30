using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public enum PlayerAssignment
{
  Player1,
  Player2,
};

[RequireComponent(typeof(PlayerHealthComponent)), RequireComponent(typeof(PlayerStateComponent))]
public class Player : MonoBehaviour
{
  public PlayerAssignment playerAssignment { get; private set; }

  // Movements
  [SerializeField] private float jumpHeight = 4f;
  public float JumpHeight => jumpHeight;
  [SerializeField] private float fallSpeed = 8f;
  public float FallSpeed => fallSpeed;
  [SerializeField] private float runningSpeed = 12f;
  public float RunningSpeed => runningSpeed;

  private bool facingRight = true;
  public bool controlsEnabled = true;

  // Components
  public Renderer modelRenderer;
  [NonSerialized] public Rigidbody2D rigidBody;
  [NonSerialized] public PlayerInput playerInput;
  public PlayerHealthComponent health { get; private set; }
  public PlayerStateComponent state { get; private set; }

  private CapsuleCollider2D mainCollider;
  private Animator animator;

  // Gameplay Manager
  private GameplayManager gameplayManager;

  // Events
  public event Action<Checkpoint> OnCheckpointActivated;
  public event Action<Player> OnInteract;
  public event Action<PlayerAssignment, bool> OnInteractionDetected;

  private Interactable _currentInteractable;
  public GameObject Climbable { get; private set; }

  void Awake()
  {
    health = GetComponent<PlayerHealthComponent>();
    state = GetComponent<PlayerStateComponent>();
    rigidBody = GetComponent<Rigidbody2D>();
    mainCollider = GetComponent<CapsuleCollider2D>();
    playerInput = GetComponent<PlayerInput>();
    animator = GetComponent<Animator>();
    gameplayManager = GameObject.Find("GameManager").GetComponent<GameplayManager>();
    playerInput.actions["Interact"].performed += Interact;
  }

  public void SetPlayerAssignment(PlayerAssignment pa)
  {
    playerAssignment = pa;
    if (Gamepad.all.Count >= 2)
    {
      playerInput.user.UnpairDevices();
      InputUser.PerformPairingWithDevice(Gamepad.all[(int)playerAssignment], playerInput.user);
      playerInput.user.ActivateControlScheme("Gamepad");
    }
    if (Gamepad.all.Count == 1 && playerAssignment == PlayerAssignment.Player1)
    {
      playerInput.user.UnpairDevices();
      InputUser.PerformPairingWithDevice(Gamepad.all[0], playerInput.user);
      playerInput.user.ActivateControlScheme("Gamepad");
    }
    playerInput.SwitchCurrentActionMap(playerAssignment == PlayerAssignment.Player1 ? "Player1" : "Player2");
  }

  void Update()
  {
    if (controlsEnabled)
    {
      float horizontalInput = playerInput.actions["Move"].ReadValue<float>();

      if (horizontalInput > 0)
        facingRight = true;
      else if (horizontalInput < 0)
        facingRight = false;
      float horizontalVelocity = horizontalInput * runningSpeed;
      rigidBody.velocity = new Vector2(horizontalVelocity, rigidBody.velocity.y);

      animator.SetFloat("HorizontalInput", Math.Abs(horizontalInput));
    }

    transform.GetChild(0).transform.rotation = Quaternion.Euler(0, facingRight ? 90 : -90, 0);

    animator.SetFloat("VelocityY", rigidBody.velocity.y);
    animator.SetBool("IsGrounded", state.currentState.state == State.GROUNDED);
    animator.SetBool("IsCrouched", state.currentState.state == State.CROUCHING);
    animator.SetBool("IsDead", state.currentState.state == State.DEAD);
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

  public void RespawnToPosition(Vector2 Checkpoint)
  {
    rigidBody.position = Checkpoint;
    state.TransitionToState(State.JUMPING);
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
    {
      _currentInteractable = null;
      OnInteractionDetected?.Invoke(playerAssignment, false);
    }

    if (other.CompareTag("Climbable"))
      Climbable = null;
  }

  public bool canInteractWithSomething() => _currentInteractable != null;
}
