using UnityEngine;

public class ProjectileTrap : Trap
{
  [Space(10f)]
  [SerializeField] private Vector2 shootingDirection;
  [SerializeField] private float projectileSpeed = 5f;
  [SerializeField] private float spawnInterval = 3f;

  private Vector2 _position;

  private void Awake()
  {
    InvokeRepeating(nameof(ShootProjectile), 2f, spawnInterval);
    _position = transform.position;
  }

  private void ShootProjectile()
  {
    GameObject newProjectile = Instantiate(hitBox, _position + shootingDirection, Quaternion.identity);
    newProjectile.GetComponent<Rigidbody2D>().velocity = shootingDirection * projectileSpeed;
  }
}
