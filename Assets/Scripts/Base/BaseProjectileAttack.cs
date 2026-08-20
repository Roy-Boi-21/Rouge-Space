using UnityEngine;

public class BaseProjectileAttack : MonoBehaviour
{
    [Header("Base Projectile Attack")]
    [Tooltip("The projectile to launch.")]
    [SerializeField] protected GameObject projectile;

    [Tooltip("Where the projectile should be initialized.  Leave blank for the center of the game object.")]
    [SerializeField] protected Transform launchPoint;

    [Tooltip("Where the projectile should go after being launched.  Leave blank to shoot in front of the object.")]
    [SerializeField] protected Transform target;

    [Tooltip("The projectile's initial speed after being launched.")]
    [SerializeField] protected float launchSpeed = 10f;

    [Tooltip("How much time must pass before this weapon can fire again.")]
    [SerializeField] protected float launchDelay = 1f;

    [Tooltip("Optional: The lower bound of the object's pitch shift.")]
    [SerializeField] protected float lowerPitch = 0.75f;

    [Tooltip("Optional: The upper bound of the object's pitch shift.")]
    [SerializeField] protected float upperPitch = 1.25f;

    [Tooltip("Optional: The audio source responsible for playing and categorizing the audio.")]
    [SerializeField] protected AudioSource audioSource;

    [Tooltip("Optional: The sound effect that plays when the projectile is launched.")]
    [SerializeField] protected AudioClip audioClip;

    protected GameObject targetObj;
    protected float delay = 0f;
    protected bool considerTargetVelocity = false;

    protected virtual void Start()
    {
        if (projectile == null)
        {
            Debug.LogError("Projectile not provided.");
        }

        if (launchPoint == null)
        {
            launchPoint = transform;
        }
    }

    protected void Update()
    {
        delay -= Time.deltaTime;
        if (delay <= 0)
        {
            LaunchProjectile(launchSpeed);
            delay = launchDelay;
        }
    }

    public virtual void LaunchProjectile(float speed)
    {
        if ((target != null) && (target.position == launchPoint.position))
        {
            Debug.LogError("The target and launch point are the same point!  The projectile does not know where to go!");
            return;
        }

        if (audioClip != null)
        {
            audioSource.pitch = Random.Range(lowerPitch, upperPitch);
            audioSource.PlayOneShot(audioClip);
        }

        GameObject newProjectile = Instantiate(projectile, launchPoint.position, launchPoint.rotation);
        newProjectile.SetActive(true);

        Rigidbody2D newRB = newProjectile.GetComponent<Rigidbody2D>();

        if (target != null)
        {
            Vector2 targetPos = Vector2.zero;

            if (considerTargetVelocity)
            {
                Vector3 targetVelocity = Vector3.zero;
                Rigidbody2D targetRB = target.gameObject.GetComponent<Rigidbody2D>();
                if (targetRB != null)
                {
                    targetVelocity = targetRB.linearVelocity;
                    
                }
                float distance = Vector3.Distance(target.position, transform.position);
                targetPos = Vector2.Normalize(target.position + (targetVelocity * (distance / speed)) - transform.position);
            }
            else
            {
                targetPos = Vector2.Normalize(target.position - launchPoint.position);
            }

            newRB.linearVelocity = targetPos * speed;

            Vector2 direction = Vector2.Normalize(target.position - transform.position);

            const float DIRECTION_FIXER = 270f; // This direction fixer ensures that the spriteRenderer points in the direction of its movement.
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + DIRECTION_FIXER;

            Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            newProjectile.transform.rotation = targetRotation;
        }
        else
        {
            newRB.linearVelocity = Vector2.Normalize(transform.up) * speed;
        }
    }
    
    public GameObject GetProjectile()
    {
        return projectile;
    }

    public float GetLaunchDelay()
    {
        return launchDelay;
    }

    public void DecreaseLaunchDelay(float percent)
    {
        launchDelay = launchDelay / percent;
    }

    public void SetLaunchDelay(float newDelay)
    {
        launchDelay = newDelay;
    }

    private void OnValidate()
    {
        lowerPitch = Mathf.Min(lowerPitch, upperPitch);
        upperPitch = Mathf.Max(lowerPitch, upperPitch);
    }
}
