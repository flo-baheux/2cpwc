using System;

public class CharacterHealthComponent
{
  public int maxHealth = 9;
  public int currentHealth = 0;

  // Triggers when health is modified - parameter is current health after change
  public event Action<int> CharacterHealthChanged;
  // Triggers when current health reaches 0
  public event Action CharacterDied;

  public void AddHealth(int value)
  {
    currentHealth = Math.Clamp(currentHealth + value, 0, maxHealth);
    CharacterHealthChanged?.Invoke(currentHealth);
  }

  public void ReduceHealth(int value)
  {
    currentHealth = Math.Clamp(currentHealth - value, 0, maxHealth);
    CharacterHealthChanged?.Invoke(currentHealth);
    if (currentHealth == 0)
      CharacterDied?.Invoke();
  }
}
