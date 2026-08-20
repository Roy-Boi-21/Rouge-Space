using UnityEngine;
using System.Collections;

public class PlayerProjectileAttack : BaseProjectileAttack
{
    [Header("Player Projectile Attack")]
    [Tooltip("Whether the projectile should automatically look for targets to aim at.")]
    [SerializeField] protected bool autoAim;

    [Tooltip("Whether projectiles should gain the speed of the game object moving them.")]
    [SerializeField] protected bool inheritMoveSpeed = true;

    [Header("Upgrade Manager Data")]
    [Tooltip("The weapon's title in the weapon select menu.")]
    [SerializeField] protected string title;

    [Tooltip("The weapon's description in the weapon select menu.")]
    [SerializeField] protected string description;

    [Tooltip("The weapon's image in the weapon select menu.")]
    [SerializeField] protected Sprite image;

    protected PlayerMovement movement;
    protected Transform defaultTarget;

    protected override void Start()
    {
        defaultTarget = target;

        movement = GameObject.Find("PlayerShip").GetComponent<PlayerMovement>();
        if (movement == null)
        {
            Debug.LogError("Player movement not found.");
        }

        if (title == "")
        {
            Debug.LogError("Weapon title not provided.");
        }
        if (description == "")
        {
            Debug.LogError("Weapon description not provided.");
        }
        if (image == null)
        {
            Debug.LogError("Weapon image not provided.");
        }

        if (autoAim)
        {
            considerTargetVelocity = true;
        }

        base.Start();
    }

    protected GameObject FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject nearestEnemy = null;
        float shortestDistance = float.MaxValue;

        foreach (GameObject enemy in enemies)
        {
            // Ignore bullets.
            if (enemy.GetComponent<BaseHealth>() == null)
            {
                continue;
            }

            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < shortestDistance)
            {
                nearestEnemy = enemy;
                shortestDistance = distance;
            }
        }
        
        return nearestEnemy;
    }

    public override void LaunchProjectile(float speed)
    {
        if (autoAim)
        {
            GameObject nearestEnemy = FindNearestEnemy();

            if (nearestEnemy != null)
            {
                target = nearestEnemy.transform;
            }
            else
            {
                target = defaultTarget;
            }
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

    public string GetTitle()
    {
        return title;
    }

    public string GetDescription()
    {
        return description;
    }

    public Sprite GetImage()
    {
        return image;
    }
}
