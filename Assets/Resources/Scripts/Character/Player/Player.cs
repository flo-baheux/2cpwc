using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
  public float jumpHeight = 10f;
  public float fallSpeed = 12f;
  public float runningSpeed = 15f;


  [NonSerialized] public Rigidbody2D rigidBody;
  private CapsuleCollider2D capsuleCollider;

  public bool controlsEnabled = true;

  public Dictionary<State, PlayerState> States;

  public PlayerState currentState;

  public PlayerGroundedState playerGroundedState;
  public PlayerJumpingState playerJumpingState;

  void Awake()
  {
    rigidBody = GetComponent<Rigidbody2D>();
    capsuleCollider = GetComponent<CapsuleCollider2D>();
    playerGroundedState = new PlayerGroundedState(this);
    playerJumpingState = new PlayerJumpingState(this);

    States = new Dictionary<State, PlayerState>() {
      {State.GROUNDED, playerGroundedState},
      {State.JUMPING, playerJumpingState},
    };
    currentState = States[State.GROUNDED];
  }

  void Update()
  {
    float horizontalInput = Input.GetAxisRaw("Horizontal");

    float horizontalVelocity = horizontalInput * runningSpeed;
    rigidBody.velocity = new Vector2(horizontalVelocity, rigidBody.velocity.y);

    State? newState = currentState.CustomUpdate();
    if (newState.HasValue)
    {
      Debug.Log("New state - " + newState);
      currentState.Exit();
      currentState = States[newState.Value];
      currentState.Enter();
    }
  }

  public bool IsGrounded()
  {
    Vector2 center = new(capsuleCollider.bounds.center.x, capsuleCollider.bounds.min.y);
    Vector2 size = new(capsuleCollider.bounds.size.x + 0.2f, 0.05f);
    RaycastHit2D raycastHit = Physics2D.BoxCast(center, size, 0f, Vector2.down, 1f, ~gameObject.layer);
    return raycastHit.collider;
  }

  // Debug IsGrounded box collider
  // public void OnDrawGizmos()
  // {
  //   Gizmos.DrawCube(new Vector2(capsuleCollider.bounds.center.x, capsuleCollider.bounds.min.y), new Vector2(capsuleCollider.bounds.size.x + 0.2f, 0.05f));
  // }
}
