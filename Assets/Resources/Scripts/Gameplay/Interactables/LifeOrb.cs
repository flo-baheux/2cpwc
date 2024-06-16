using UnityEngine;

public class LifeOrb : MonoBehaviour, Interactable
{
    [SerializeField] private int healthIncreased = 1;
    
    public void Interact(Player player)
    {
        Debug.Log("In");
        if (player.TryGetComponent(out PlayerHealthComponent healthComponent))
        {
            healthComponent.AddHealth(healthIncreased);
            gameObject.SetActive(false);
        }
    }
}
