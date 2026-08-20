using UnityEngine;

public class ExplosiveProjectile : ProjectileComponent
{
    [Header("Explosive Projectile Data")]
    [Tooltip("The explosion that appears when the projectile dies.")]
    [SerializeField] protected Explosion explosion;

    [Tooltip("Whether the explosion appears everytime this projectile hits something.")]
    [SerializeField] public bool explodeOnHit = false;

    [Tooltip("Whether the explosion appears when this projectile dies.")]
    [SerializeField] public bool explodeOnDeath = true;

    protected override void Start()
    {
        base.Start();

        if (explosion == null)
        {
            Debug.LogError("Explosion not provided.");
        }
    }

    protected override void RegisterHit(GameObject receiver)
    {
        base.RegisterHit(receiver);

        // Do not collide with projectiles.
        if (receiver.layer == LayerMask.NameToLayer("Bullet"))
        {
            return;
        }

        // Do not hit anything with the same tag as the projectile.
        if (receiver.CompareTag("ProjectileIgnore") || receiver.CompareTag(gameObject.tag))
        {
            return;
        }

        if (explodeOnHit && explosion != null)
        {
            Explosion newExplosion = Instantiate(explosion, transform.position, transform.rotation);
            newExplosion.gameObject.SetActive(true);
        }
    }

    protected override void DestroyProjectile()
    {
        if (explodeOnDeath && explosion != null)
        {
            Explosion newExplosion = Instantiate(explosion, transform.position, transform.rotation);
            newExplosion.gameObject.SetActive(true);
        }
        base.DestroyProjectile();
    }

    public Explosion GetExplosion()
    {
        return explosion;
    }
}
