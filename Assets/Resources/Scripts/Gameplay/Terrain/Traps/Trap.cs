using UnityEngine;

public class Trap : MonoBehaviour
{
  [SerializeField] protected GameObject hitbox;
  [SerializeField] private int onHitDamage = 1;

  private void Awake()
  {
    if (hitbox)
      hitbox.GetComponent<HitboxDamage>().damage = onHitDamage;
  }
}
