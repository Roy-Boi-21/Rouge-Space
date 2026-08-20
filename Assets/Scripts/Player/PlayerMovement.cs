using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Tooltip("The player's top speed.")]
    [SerializeField] private float maxSpeed = 10f;

    [Tooltip("The rate at which the player gains speed.")]
    [SerializeField] private float acceleration = 10f;

    [Tooltip("The rate at which the player rotates to the direction they're moving.")]
    [SerializeField] private float rotationSpeed = 100f;

    private float moveSpeed = 0f;
    Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D not assigned to the player.");
        }
    }

    private void FixedUpdate()
    {
        Vector2 axis = UserInput.instance.moveInput;
        Vector2 direction = axis.normalized;
        if (axis != Vector2.zero)
        {
            moveSpeed += acceleration * Time.fixedDeltaTime;
            if (moveSpeed > maxSpeed)
            {
                moveSpeed = maxSpeed;
            }

            // Rotate the player spriteRenderer.
            // These 5 lines were made with generative AI.
            const float DIRECTION_FIXER = 270f; // This direction fixer ensures that the spriteRenderer points in the direction of its movement.
            float angle = Mathf.Atan2(axis.y, axis.x) * Mathf.Rad2Deg + DIRECTION_FIXER;

            Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
        else
        {
            moveSpeed = 0f;
        }
        rb.linearVelocity = axis * moveSpeed;
    }

    public float GetSpeed()
    {
        return moveSpeed;
    }

    public float GetMaxSpeed()
    {
        return maxSpeed;
    }

    public void SetSpeed(float newSpeed)
    {
        maxSpeed = newSpeed;
    }
}
