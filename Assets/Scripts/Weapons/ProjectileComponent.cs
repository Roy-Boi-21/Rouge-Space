using UnityEngine;

public class ProjectileComponent : MonoBehaviour
{
    [Header("Base Projectile Data")]
    [Tooltip("How much damage this projectile does on contact.")]
    [SerializeField] protected int damage = 1;

    [Tooltip("How many times this projectile can hit something before disappearing.  Set to '-1' if the projectile has infinite pierce.")]
    [SerializeField] protected int pierce = 1;

    [Tooltip("The time until this projectile naturally destroys itself.  Set to '-1' if the projectile does not have a life time.")]
    [SerializeField] protected float lifeTime = 5f;

    [Tooltip("Optional: What particle effects will come out whenever this projectile hits something.")]
    [SerializeField] protected GameObject hitParticles;

    protected Rigidbody2D rb;
    
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Projectile is missing a rigidbody.");
        }
    }

    protected void FixedUpdate()
    {
        if (lifeTime == -1f)
        {
            return;
        }

        lifeTime -= Time.fixedDeltaTime;
        if (lifeTime <= 0)
        {
            DestroyProjectile();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        RegisterHit(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        RegisterHit(collision.gameObject);
    }

    protected virtual void RegisterHit(GameObject receiver)
    {
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

        BaseHealth health = receiver.GetComponent<BaseHealth>();
        if (health != null)
        {
            health.TakeDamage(damage); 
        }

        if (hitParticles != null) 
        {
            GameObject newParticles = Instantiate(hitParticles, transform.position, transform.rotation);
            newParticles.SetActive(true);
        }

        if (pierce == -1)
        {
            return;
        }
        pierce--;
        if (pierce <= 0)
        {
            DestroyProjectile();
        }
    }

    protected virtual void DestroyProjectile()
    {
        Destroy(gameObject);
    }

    public int GetDamage()
    {
        return damage;
    }

    public int GetPierce()
    {
        return pierce;
    }

    public void IncreaseDamage(int extraDamage)
    {
        damage += extraDamage;
    }

    public void IncreasePierce(int extraPierce)
    {
        pierce += extraPierce;
    }
}
