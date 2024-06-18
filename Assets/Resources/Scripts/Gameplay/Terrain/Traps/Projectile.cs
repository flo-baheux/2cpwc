using UnityEngine;

public class Projectile : MonoBehaviour
{
  [SerializeField] private float _lifeSpan = 3f;

  private void Awake()
  {
    Destroy(gameObject, _lifeSpan);
  }

  private void OnCollisionEnter2D(Collision2D other)
  {
    // Damage?
    Destroy(gameObject);
  }
}
