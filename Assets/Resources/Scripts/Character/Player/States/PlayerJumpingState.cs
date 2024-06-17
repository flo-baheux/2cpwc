using UnityEngine;

public class PlayerJumpingState : PlayerState
{
  public PlayerJumpingState(Player player) : base(player)
  {
    this.state = State.JUMPING;
  }

  public override State? CustomUpdate()
  {
    if (Player.IsGrounded() && Player.rigidBody.velocity.y <= 0.1f)
      return State.GROUNDED;

    if (Player.controlsEnabled && Player.playerInput.actions["Jump"].WasReleasedThisFrame())
      Player.rigidBody.velocity = new Vector2(Player.rigidBody.velocity.x, -Player.fallSpeed);

    // Never fall faster than Player.fallSpeed
    Player.rigidBody.velocity = new Vector2(Player.rigidBody.velocity.x, Mathf.Max(Player.rigidBody.velocity.y, -Player.fallSpeed));
    
    // This part is for the climbing mechanic
    if (Player.controlsEnabled && Player.playerInput.actions["Crouch"].WasPressedThisFrame())
    {
      Debug.Log("Reaches this part");

      if (Player.Climbable != null)
      {
        return State.CLIMBING;
      }
    }
    
    return null;
  }
}