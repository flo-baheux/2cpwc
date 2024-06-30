using UnityEngine;

public class ProjectileTrap : Trap
{
  [Space(10f)]
  [SerializeField] private GameObject projectilePrefab;
  [SerializeField] private float projectileSpeed;
  [SerializeField] private float spawnInterval;
  [SerializeField] private float projectileLifespan;
  [SerializeField] private int projectileDamage;
  private Transform projectileEmitter;

  protected override void Awake()
  {
    base.Awake();
    InvokeRepeating(nameof(ShootProjectile), 0, spawnInterval);
    projectileEmitter = transform.GetChild(0);
  }

  private void ShootProjectile()
  {
    HitboxDamage projectile = Instantiate(projectilePrefab.gameObject, projectileEmitter.position, projectilePrefab.transform.rotation).GetComponent<HitboxDamage>();
    projectile.gameObject.GetComponent<Rigidbody2D>().velocity = projectileEmitter.up * projectileSpeed;
    projectile.damage = projectileDamage;
    projectile.destroyAfterHit = true;
    Destroy(projectile, projectileLifespan);
  }
}
