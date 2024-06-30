using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerHealthComponent : MonoBehaviour
{
  [SerializeField] private int maxHealth = 9;
  [SerializeField] private int currentHealth = 9;
  [SerializeField] private float InvulnerabilityDuration = 1;

  private Player Player;
  private bool IsInvulnerable = false;

  public event Action<int, int> PlayerHealthChanged;

  private void Awake()
  {
    Player = GetComponent<Player>();
  }

  public void AddHealth(int value)
  {
    int healthBefore = currentHealth;
    currentHealth = Math.Clamp(currentHealth + value, 0, maxHealth);
    PlayerHealthChanged?.Invoke(healthBefore, currentHealth);
  }

  public void ReduceHealth(int value)
  {
    if (!IsInvulnerable)
    {
      int healthBefore = currentHealth;
      currentHealth = Math.Clamp(currentHealth - value, 0, maxHealth);
      PlayerHealthChanged?.Invoke(healthBefore, currentHealth);
      if (currentHealth < healthBefore && currentHealth > 0)
        StartCoroutine(InvulnerabilityCoroutine());
      if (currentHealth == 0 && Player.state.currentState.state != State.DEAD)
        Player.state.TransitionToState(State.DEAD);
    }
  }

  public void ResetHealth()
  {
    int healthBefore = currentHealth;
    currentHealth = maxHealth;
    PlayerHealthChanged?.Invoke(healthBefore, currentHealth);
  }

  IEnumerator InvulnerabilityCoroutine()
  {
    IsInvulnerable = true;
    float endTimer = Time.time + InvulnerabilityDuration;
    while (Time.time <= endTimer)
    {
      Player.modelRenderer.enabled = false;
      yield return new WaitForSeconds(0.125f);
      Player.modelRenderer.enabled = true;
      yield return new WaitForSeconds(0.125f);
    }
    Player.modelRenderer.enabled = true;
    IsInvulnerable = false;
  }
}
