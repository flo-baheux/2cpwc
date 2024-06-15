using System;

public class PlayerHealthComponent
{
  private readonly Player Player;

  public PlayerHealthComponent(Player player)
  {
    Player = player;
  }

  public int maxHealth = 9;
  public int currentHealth = 0;

  // Triggers when health is modified - parameter is current health after change
  public event Action<int> PlayerHealthChanged;

  public void AddHealth(int value)
  {
    currentHealth = Math.Clamp(currentHealth + value, 0, maxHealth);
    PlayerHealthChanged?.Invoke(currentHealth);
  }

  public void ReduceHealth(int value)
  {
    currentHealth = Math.Clamp(currentHealth - value, 0, maxHealth);
    PlayerHealthChanged?.Invoke(currentHealth);
    if (currentHealth == 0)
      Player.TransitionToState(State.DEAD);
  }
}
