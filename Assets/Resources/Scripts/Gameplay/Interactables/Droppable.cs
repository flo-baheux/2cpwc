using UnityEngine;

public class Droppable : MonoBehaviour, Interactable
{
    private Rigidbody2D _rigidbody;
    private Player _currentPlayer;
    private bool _interacting = false;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }
    
    public void Interact(Player player)
    {
        if (!_interacting)
        {
            _currentPlayer = player;
            transform.parent = player.transform;
            _interacting = true;
            _rigidbody.isKinematic = false;
        }

        else if (_interacting && _currentPlayer == player)
        {
            _currentPlayer = null;
            transform.parent = null;
            _interacting = false;
            _rigidbody.isKinematic = true;
        }
    }
}
