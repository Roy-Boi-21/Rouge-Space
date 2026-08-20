using UnityEngine;

public class EnemyProjectileAttack : BaseProjectileAttack
{
    [Header("Enemy Projectile Attack")]
    [Tooltip("How close the enemy must be to the player before the enemy start firing.")]
    [SerializeField] private float shootDistance = 16f;

    [Tooltip("Whether the projectile should automatically look for targets to aim at.")]
    [SerializeField] protected bool autoAim;

    [Tooltip("Whether projectiles should gain the speed of the game object moving them.")]
    [SerializeField] protected bool inheritMoveSpeed = true;

    protected Transform defaultTarget;
    protected FollowTarget movement;

    protected override void Start()
    {
        defaultTarget = target;

        movement = GetComponent<FollowTarget>();
        if (movement == null)
        {
            Debug.LogWarning("Follow Target Component not found.");
        }

        base.Start();
    }

    protected GameObject FindPlayer()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Player");
        GameObject nearestTarget = null;
        float shortestDistance = float.MaxValue;

        foreach (GameObject target in targets)
        {
            // Ignore bullets.
            if (target.GetComponent<BaseHealth>() == null)
            {
                continue;
            }

            float distance = Vector2.Distance(transform.position, target.transform.position);
            if (distance < shortestDistance)
            {
                nearestTarget = target;
            }
        }

        return nearestTarget;
    }

    public override void LaunchProjectile(float speed)
    {
        GameObject player = FindPlayer();

        if (autoAim)
        {
            if (player != null)
            {
                target = player.transform;
            }
            else
            {
                target = defaultTarget;
            }
        }

        if (player == null)
        {
            return;
        }
        else if (Vector2.Distance(transform.position, player.transform.position) > shootDistance)
        {
            return;
        }

        if (inheritMoveSpeed)
        {
            base.LaunchProjectile(launchSpeed + movement.GetSpeed());
        }
        else
        {
            base.LaunchProjectile(launchSpeed);
        }
    }
}
