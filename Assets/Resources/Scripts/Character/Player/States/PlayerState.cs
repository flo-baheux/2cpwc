using System;

public enum State
{
  GROUNDED,
  JUMPING,
  CLIMBING,
  CROUCHING
};

public abstract class PlayerState
{
  public State state;
  protected Player Player;

  public PlayerState(Player player)
  {
    Player = player;
  }

  public event Action OnEnter;
  public event Action OnExit;

  public virtual void Enter()
  {
    OnEnter?.Invoke();
  }

  public abstract State? CustomUpdate();

  public virtual void Exit()
  {
    OnExit?.Invoke();
  }
}