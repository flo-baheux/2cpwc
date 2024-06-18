using System;

public class PlayerHealthComponent
{
  private readonly Player Player;

  public PlayerHealthComponent(Player player)
  {
    Player = player;
    currentHealth = maxHealth;
  }

  public int maxHealth { get; private set; } = 9;
  public int currentHealth { get; private set; }

  public event Action<int, int> PlayerHealthChanged;

  public void AddHealth(int value)
  {
    int healthBefore = currentHealth;
    currentHealth = Math.Clamp(currentHealth + value, 0, maxHealth);
    PlayerHealthChanged?.Invoke(healthBefore, currentHealth);
  }

  public void ReduceHealth(int value)
  {
    if (!Player.isInvulnerable)
    {
      int healthBefore = currentHealth;
      currentHealth = Math.Clamp(currentHealth - value, 0, maxHealth);
      PlayerHealthChanged?.Invoke(healthBefore, currentHealth);
      if (currentHealth == 0)
        Player.TransitionToState(State.DEAD);
    }
  }

  public void ResetHealth()
  {
    int healthBefore = currentHealth;
    currentHealth = maxHealth;
    PlayerHealthChanged?.Invoke(healthBefore, currentHealth);
  }
}
