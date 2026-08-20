using UnityEngine;

public class RicochetProjectile : ProjectileComponent
{
    [Header("Ricochet Projectile Data")]
    [Tooltip("The minimum distance between targets to consider ricochet.")]
    [SerializeField] protected float ricochetLowerBound = 0.05f;

    [Tooltip("The maximum distance between targets to consider ricochet.")]
    [SerializeField] protected float ricochetUpperBound = 8f;

    [Tooltip("How much faster the projectile gets upon colliding with an object.")]
    [SerializeField] protected float speedOnHit = 2f;

    [Tooltip("How much more life time the projectile gets upon colliding with an object.")]
    [SerializeField] protected float lifeTimeOnHit = 2f;

    protected override void RegisterHit(GameObject receiver)
    {
        GameObject newTarget = FindRicochetTarget(receiver);

        if (newTarget != null) 
        {
            Vector3 velocity = newTarget.GetComponent<Rigidbody2D>().linearVelocity;
            float distance = Vector3.Distance(newTarget.transform.position, transform.position);
            Vector2 targetPos = Vector2.Normalize((newTarget.transform.position + (velocity * (distance / (rb.linearVelocity.magnitude + speedOnHit)))) - transform.position);

            // Rotate the projectile.
            const float DIRECTION_FIXER = 270f; // This direction fixer ensures that the spriteRenderer points in the direction of its movement.
            float angle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg + DIRECTION_FIXER;

            Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360f);

            rb.linearVelocity = targetPos * (rb.linearVelocity.magnitude + speedOnHit);
            lifeTime += lifeTimeOnHit;
        }

        base.RegisterHit(receiver);
    }

    protected GameObject FindRicochetTarget(GameObject excludedTarget = null)
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject ricochetTarget = null;
        float shortestDistance = ricochetUpperBound;

        foreach (GameObject target in targets)
        {
            // Ignore bullets.
            if (target.GetComponent<BaseHealth>() == null)
            {
                continue;
            }
            
            // Do not ricochet into the enemy you just hit.
            if (target == excludedTarget)
            {
                continue;
            }

            float distance = Vector2.Distance(transform.position, target.transform.position);
            if ((distance < shortestDistance) && (distance > ricochetLowerBound))
            {
                ricochetTarget = target;
                shortestDistance = distance;
            }
        }

        return ricochetTarget;
    }
}
