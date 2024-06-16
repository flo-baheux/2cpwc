using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour, Interactable
{
    [SerializeField] private List<Barrier> barriers;

    public void Interact(Player player)
    {
        Debug.Log("Interacting with lever");

        foreach (Barrier barrier in barriers)
        {
            barrier.ToggleBarrier();
        }
    }
}
