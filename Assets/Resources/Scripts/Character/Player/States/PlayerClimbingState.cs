using UnityEngine;

public class PlayerClimbingState : PlayerState
{
  Vector2 lockPosition = Vector2.zero;

  public PlayerClimbingState(Player player) : base(player)
  {
    this.state = State.CLIMBING;
  }

  public override void Enter()
  {
    lockPosition = Player.Climbable.transform.GetChild(0).position;
  }

  public override State? CustomUpdate()
  {
    Player.transform.position = lockPosition;
    Player.rigidBody.velocity = Vector2.zero;

    if (Player.controlsEnabled)
    {
      if (Player.playerInput.actions["Jump"].WasPressedThisFrame())
      {
        Player.transform.position = Player.Climbable.transform.GetChild(1).position;
        return State.JUMPING;
      }
      if (Player.playerInput.actions["DropDown"].WasPressedThisFrame())
        return State.JUMPING;
    }

    return null;
  }
}
