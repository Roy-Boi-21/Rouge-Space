using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [Tooltip("The target's transform.")]
    [SerializeField] private Transform targetTransform;

    [Tooltip("The tag of the object to look for.")]
    [SerializeField] private string targetTag;

    [Tooltip("How quickly the object follows the target.")]
    [SerializeField] private float followSpeed = 5f;

    [Tooltip("How quickly the object rotates to face the target.")]
    [SerializeField] private float followRotation = 100f;

    [Tooltip("Check if the object should accelerate to its target if it gets too far")]
    [SerializeField] private bool canRubberband = false;

    [Tooltip("How far away the target must be for the object to start rubberbanding.")]
    [SerializeField] private float rubberbandStartDistance = 64f;

    [Tooltip("How close the target must be for the object to stop rubberbanding.")]
    [SerializeField] private float rubberbandStopDistance = 32f;

    [Tooltip("How quickly the object gains speed while rubberbanding.")]
    [SerializeField] private float rubberbandAcceleration = 1f;

    [Tooltip("How quickly the object loses speed when they stop rubberbanding.")]
    [SerializeField] private float rubberbandDeceleration = 1f;

    Rigidbody2D rb;
    private float originalSpeed;
    private float rubberbandSpeed = 0;
    private bool isRubberbanding = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Enemy Rigidbody2D not found.");
        }

        originalSpeed = followSpeed;

        GameObject target = FindNearestTarget();
        if (target != null)
        {
            targetTransform = target.transform;
        }
    }

    protected GameObject FindNearestTarget()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);
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
                shortestDistance = distance;
            }
        }

        return nearestTarget;
    }

    private void FixedUpdate()
    {
        if (rb == null)
        {
            return;
        }

        // A targer is not provided, find one.
        if (targetTransform == null)
        {
            GameObject target = FindNearestTarget();
            if (target != null)
            {
                targetTransform = target.transform;
            }
            else
            {
                return;
            }
        }

        Vector2 direction = Vector2.Normalize(targetTransform.position - transform.position);

        // Rotate the follower spriteRenderer.
        const float DIRECTION_FIXER = 270f; // This direction fixer ensures that the spriteRenderer points in the direction of its movement.
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + DIRECTION_FIXER;

        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, followRotation * Time.fixedDeltaTime);

        if (canRubberband)
        {
            float distance = Vector2.Distance(targetTransform.position, transform.position);

            if (distance >= rubberbandStartDistance)
            {
                isRubberbanding = true;
            }
            else if (distance <= rubberbandStopDistance)
            {
                isRubberbanding = false;
            }

            if (isRubberbanding)
            {
                rubberbandSpeed += rubberbandAcceleration * Time.fixedDeltaTime;
            }
            else
            {
                rubberbandSpeed -= rubberbandDeceleration * Time.fixedDeltaTime;
                // Prevent the object from negative rubberbanding to a standstill.
                rubberbandSpeed = Mathf.Max(0, rubberbandSpeed);
            }

            followSpeed = originalSpeed + rubberbandSpeed;
        }

        rb.linearVelocity = transform.up * followSpeed;
    }

    public float GetSpeed()
    {
        return followSpeed;
    }
}
