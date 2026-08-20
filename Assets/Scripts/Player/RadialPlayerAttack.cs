using UnityEngine;

public class RadialPlayerAttack : PlayerProjectileAttack
{
    [Header("Radial Projectile Attack")]
    [Tooltip("How many projectiles are shot out at once.")]
    [SerializeField] protected int projectileCount = 1;

    [Tooltip("Whether the bullets should be spread out evenly.")]
    [SerializeField] protected bool uniformDistribution = true;

    [Tooltip("The projectile output's field of view")]
    [SerializeField] protected float rotationRange = 360f;

    protected override void Start()
    {
        if (autoAim)
        {
            Debug.LogWarning("Auto aim feature not supported for the radial attack.");
        }

        base.Start();
    }

    public override void LaunchProjectile(float speed)
    {
        if ((target != null) && (target.position == launchPoint.position))
        {
            Debug.LogError("The target and launch point are the same point!  The projectile does not know where to go!");
            return;
        }

        if (audioClip != null)
        {
            audioSource.PlayOneShot(audioClip);
        }

        if (inheritMoveSpeed)
        {
            speed += movement.GetSpeed();
        }

        for (int i = 0; i < projectileCount; i++)
        {
            float angle = 0f;
            if (uniformDistribution)
            {
                angle = (rotationRange * Mathf.Deg2Rad) * ((float)i / (float)projectileCount) + (Mathf.PI / 2f)
                    + (transform.eulerAngles.z * Mathf.Deg2Rad)
                    - (rotationRange * Mathf.Deg2Rad / 2);
                if (rotationRange < 360f)
                {
                    angle += ((rotationRange / (float)projectileCount) * Mathf.Deg2Rad / 2);
                }
            }
            else
            {
                angle = (Random.Range(0f, rotationRange) * Mathf.Deg2Rad) + (Mathf.PI / 2f)
                    + (transform.eulerAngles.z * Mathf.Deg2Rad)
                    - (rotationRange * Mathf.Deg2Rad / 2);
                if (rotationRange < 360f)
                {
                    angle += ((rotationRange / (float)projectileCount) * Mathf.Deg2Rad / 2);
                }
            }
            
            Vector2 shotDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            GameObject newProjectile = Instantiate(projectile, launchPoint.position, launchPoint.rotation);
            newProjectile.SetActive(true);
            newProjectile.transform.eulerAngles = new Vector3(0, 0, (angle * Mathf.Rad2Deg) - 90f);
            Rigidbody2D newRB = newProjectile.GetComponent<Rigidbody2D>();

            if (target != null)
            {
                Vector2 targetPos = Vector2.Normalize(target.position - launchPoint.position);
                newRB.linearVelocity = targetPos * speed;
            }
            else
            {
                newRB.linearVelocity = Vector2.Normalize(shotDirection) * speed;
            }
        }
    }

    public int GetProjectileCount()
    {
        return projectileCount;
    }

    public void IncreaseProjectileCount(int addedProjectiles)
    {
        projectileCount += addedProjectiles;
    }

    private void OnValidate()
    {
        rotationRange = Mathf.Clamp(rotationRange, 0f, 360f);
    }
}
