using UnityEngine;

public class PlayerGroundedState : PlayerState
{
  public PlayerGroundedState(Player player) : base(player)
  {
    this.state = State.GROUNDED;
  }

  public override State? CustomUpdate()
  {
    if (Input.GetKeyDown(KeyCode.Space))
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