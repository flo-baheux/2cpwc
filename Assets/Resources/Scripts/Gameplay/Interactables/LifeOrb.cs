using Unity.VisualScripting;
using UnityEngine;

public class LifeOrb : MonoBehaviour, Interactable
{
  [SerializeField] private int healthIncreased = 1;

  public void Interact(Player player)
    {
    if (player)
    {
      player.health.AddHealth(healthIncreased);
      gameObject.SetActive(false);
    }
  }
}
