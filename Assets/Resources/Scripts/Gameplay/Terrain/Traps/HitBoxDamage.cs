using System;
using UnityEngine;

public class HitboxDamage : MonoBehaviour
{
  [NonSerialized] public int damage = 5;
  [NonSerialized] public bool destroyAfterHit = false;

  public void OnTriggerEnter2D(Collider2D other)
  {
    if (other.CompareTag("Player"))
      other.GetComponent<Player>().health.ReduceHealth(damage);

    if (destroyAfterHit)
      Destroy(gameObject);
  }
}
