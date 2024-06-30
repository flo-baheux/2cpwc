using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Barrier : MonoBehaviour
{
  [SerializeField] private bool disableColorChange = false;
  [SerializeField] private bool defaultOpen = true;
  [SerializeField] private Color openColor;
  [SerializeField] private Color closedColor;

  private BoxCollider2D _collider2D;
  private SpriteRenderer _sprite;

  private void Awake()
  {
    _collider2D = GetComponent<BoxCollider2D>();
    _sprite = GetComponent<SpriteRenderer>();
    _collider2D.enabled = !defaultOpen;
    if (!disableColorChange)
      _sprite.color = _collider2D.enabled ? closedColor : openColor;
  }

  public void ToggleBarrier()
  {
    _collider2D.enabled = !_collider2D.enabled;

    if (!disableColorChange)
      _sprite.color = _collider2D.enabled ? closedColor : openColor;
  }
}
