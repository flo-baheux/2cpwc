using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    private Interactable _currentInteractable;
    private GameObject _currentInteractableGameObject;

    private void OnInteract()
    {
        _currentInteractable?.Interact();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Touching");
        
        if (other.CompareTag("Interactable"))
        {
            _currentInteractable = other.GetComponent<Interactable>();
            _currentInteractableGameObject = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (_currentInteractableGameObject == other.gameObject)
        {
            _currentInteractable = null;
            _currentInteractableGameObject = null;
        }
    }
}
