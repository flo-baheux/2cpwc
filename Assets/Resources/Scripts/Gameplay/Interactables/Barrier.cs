using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Barrier : MonoBehaviour
{
    [SerializeField] private bool disableColorChange = false;
    [SerializeField] private Color openColor;
    [SerializeField] private Color closedColor;
    
    private BoxCollider2D _collider2D;
    private SpriteRenderer _sprite;

    private void Awake()
    {
        _collider2D = GetComponent<BoxCollider2D>();
        _sprite = GetComponent<SpriteRenderer>();
    }
    
    public void ToggleBarrier()
    {
        bool isTrigger = _collider2D.isTrigger;
        isTrigger = !isTrigger;
        _collider2D.isTrigger = isTrigger;

        if (!disableColorChange)
        {
            _sprite.color = isTrigger ? openColor : closedColor;
        }
    }
}
