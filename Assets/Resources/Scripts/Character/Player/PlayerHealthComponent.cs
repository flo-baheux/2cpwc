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

  public event Action<int, int> PlayerHealthChanged;

  public void AddHealth(int value)
  {
    int healthBefore = currentHealth;
    currentHealth = Math.Clamp(currentHealth + value, 0, maxHealth);
    PlayerHealthChanged?.Invoke(healthBefore, currentHealth);
  }

  public void ReduceHealth(int value)
  {
    int healthBefore = currentHealth;
    currentHealth = Math.Clamp(currentHealth - value, 0, maxHealth);
    PlayerHealthChanged?.Invoke(healthBefore, currentHealth);
    if (currentHealth == 0)
      Player.TransitionToState(State.DEAD);
  }
}
