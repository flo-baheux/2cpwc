using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClimbingState : PlayerState
{
        
    public PlayerClimbingState(Player player) : base(player)
    {
        this.state = State.CLIMBING;
    }
    
    public override void Enter()
    {
        Player.rigidBody.isKinematic = true;
        Player.rigidBody.velocity = Vector2.zero;
        Player.transform.position = Player.Climbable.transform.position;
    }

    public override State? CustomUpdate()
    {
        if (Player.controlsEnabled && Player.playerInput.actions["Jump"].WasPressedThisFrame())
        {
            Player.rigidBody.isKinematic = false;
            Player.rigidBody.AddForce(Player.jumpHeight * 7f * Vector2.up, ForceMode2D.Impulse);
            return State.JUMPING;
        }
        
        if (Player.controlsEnabled && Player.playerInput.actions["DropDown"].WasPressedThisFrame())
        {
            Player.rigidBody.isKinematic = false;
            return State.JUMPING;
        }
        
        return null;
    }

    public override void Exit()
    {
        
    }
}
