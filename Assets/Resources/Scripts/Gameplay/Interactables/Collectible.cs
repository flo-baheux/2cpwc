using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Collectible : MonoBehaviour, Interactable
{
    [SerializeField] private CollectibleScriptableObject collectibleScriptableObject;

    private void Awake()
    {
        if (collectibleScriptableObject.collected)
        {
            gameObject.SetActive(false);
        }
    }
    
    public void Interact(Player player)
    {
        if (FindObjectOfType<GameplayManager>().CompareCollectible(collectibleScriptableObject))
        {
            Debug.Log("Found collectible in list");
            FindObjectOfType<HUD>().DisplayMessage($"A collectible has been found!");
            collectibleScriptableObject.collected = true;
            gameObject.SetActive(false);
            return;
        }
        
        Debug.Log("Could not find collectible in list");
    }
}
