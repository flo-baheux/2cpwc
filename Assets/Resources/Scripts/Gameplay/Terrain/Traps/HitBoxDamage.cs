using UnityEngine;

public class HitBoxDamage : MonoBehaviour
{
  private int _damageDealt = 1;

  public int GetDamageValue()
  {
    return _damageDealt;
  }

  public void SetDamageValue(int damageValue)
  {
    _damageDealt = damageValue;
  }

  public void OnTriggerEnter2D(Collider2D other)
  {
    if (other.CompareTag("Player"))
      other.GetComponent<Player>().health.ReduceHealth(_damageDealt);
  }
}
