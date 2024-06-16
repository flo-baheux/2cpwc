using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGroundedState : PlayerState
{
  public PlayerGroundedState(Player player) : base(player)
  {
    this.state = State.GROUNDED;
  }

  public override State? CustomUpdate()
  {
    if (Player.controlsEnabled && Player.playerInput.actions["Jump"].WasPressedThisFrame())
    {
      float jumpForce = Mathf.Sqrt(Player.jumpHeight * -2 * (Physics2D.gravity.y * Player.rigidBody.gravityScale));
      Player.rigidBody.AddForce(jumpForce * Vector2.up, ForceMode2D.Impulse);
      return State.JUMPING;
    }

    if (!Player.IsGrounded())
      return State.JUMPING;

    return null;
  }
}