using UnityEngine;
using System;
using System.Collections.Generic;

public class ExplosiveUpgrade : WeaponUpgrade
{
    [Header("Explosion Upgrade Components")]
    [Tooltip("How much extra damage the explosion will get after an upgrade.  Set to 0 to disable these upgrades.")]
    [SerializeField] protected int explosionDamageIncrease = 1;

    [Tooltip("How much extra pierce the explosion will get after an upgrade.  Set to 0 to disable these upgrades.")]
    [SerializeField] protected int explosionPierceIncrease = 1;

    [Tooltip("How much bigger the explosion will be after an upgrade.  Set to 0 to disable these upgrades.")]
    [SerializeField] protected float explosionSizeIncrease = 0.5f;

    protected ExplosiveProjectile expProjectile;
    protected Explosion explosion;
    protected bool canExplode;
    protected bool fullyExplodable;  // If I come up with a better name, I'll replace this.
    protected UpgradeType expType = UpgradeType.Damage;
    protected List<UpgradeType> expUpgrades = new List<UpgradeType>();

    new protected enum UpgradeType : int
    {
        Damage,
        Pierce,
        FireSpeed,
        AddExplosion,
        ExplosiveDamage,
        ExplosivePierce,
        ExplosiveSize
    }

    public override void Start()
    {
        if (initialized)
        {
            return;
        }

        base.Start();

        expProjectile = weaponData.GetProjectile().GetComponent<ExplosiveProjectile>();
        explosion = expProjectile.GetExplosion();

        if (expProjectile == null)
        {
            Debug.LogError("Exploding Projectile not found.");
        }

        if (explosion == null)
        {
            Debug.LogError("Explosion not found.");
        }

        RefreshExplosionStatus();
    }

    protected void RefreshExplosionStatus()
    {
        canExplode = expProjectile.explodeOnHit || expProjectile.explodeOnDeath;
        fullyExplodable = expProjectile.explodeOnHit && expProjectile.explodeOnDeath;
    }

    protected override void RefreshAvailableUpgrades()
    {
        expUpgrades.Clear();

        foreach (UpgradeType upgrade in Enum.GetValues(typeof(UpgradeType)))
        {
            expUpgrades.Add(upgrade);
        }

        if (damageIncrease <= 0)
        {
            expUpgrades.Remove(UpgradeType.Damage);
        }
        if (pierceIncrease <= 0)
        {
            expUpgrades.Remove(UpgradeType.Pierce);
        }
        if ((fireSpeedMultiplier == 1f) || ((1f / weaponData.GetLaunchDelay()) >= fireSpeedLimit))
        {
            expUpgrades.Remove(UpgradeType.FireSpeed);
        }

        RefreshExplosionStatus();

        if (explosionDamageIncrease <= 0)
        {
            expUpgrades.Remove(UpgradeType.ExplosiveDamage);
        }
        if (explosionPierceIncrease <= 0)
        {
            expUpgrades.Remove(UpgradeType.ExplosivePierce);
        }
        if (explosionSizeIncrease <= 0f)
        {
            expUpgrades.Remove(UpgradeType.ExplosiveSize);
        }

        if (!canExplode)
        {
            expUpgrades.Remove(UpgradeType.ExplosiveDamage);
            expUpgrades.Remove(UpgradeType.ExplosivePierce);
            expUpgrades.Remove(UpgradeType.ExplosiveSize);
        }

        if (fullyExplodable)
        {
            expUpgrades.Remove(UpgradeType.AddExplosion);
        }
    }

    public override void ReloadUpgrade()
    {
        RefreshAvailableUpgrades();

        expType = expUpgrades[UnityEngine.Random.Range(0, expUpgrades.Count)];

        if (weapon.activeInHierarchy)
        {
            switch (expType)
            {
                case UpgradeType.Damage:
                    description.text = "Damage: " + expProjectile.GetDamage() + " --> <color=green>" + (expProjectile.GetDamage() + damageIncrease) + "</color>";
                    break;
                case UpgradeType.Pierce:
                    description.text = "Pierce: " + expProjectile.GetPierce() + " --> <color=green>" + (expProjectile.GetPierce() + pierceIncrease) + "</color>";
                    break;
                case UpgradeType.FireSpeed:
                    float projectedFireSpeed = 1f / (weaponData.GetLaunchDelay() / fireSpeedMultiplier);
                    if (projectedFireSpeed >= fireSpeedLimit)
                    {
                        projectedFireSpeed = fireSpeedLimit;
                    }
                    float roundedInitialDelay = Mathf.Round(100f / weaponData.GetLaunchDelay()) / 100f;
                    float roundedFinalDelay = Mathf.Round(100f / (weaponData.GetLaunchDelay() / fireSpeedMultiplier)) / 100f;
                    description.text = "Fire Speed: " + roundedInitialDelay + " --> <color=green>" + roundedFinalDelay + "</color>";
                    break;
                case UpgradeType.AddExplosion:
                    if (!canExplode)
                    {
                        description.text = "Projectiles now <color=orange>explode</color> on death.";
                    }
                    else
                    {
                        description.text = "Projectiles now <color=orange>explode</color> on every hit.";
                    }
                    break;
                case UpgradeType.ExplosiveDamage:
                    description.text = "Explosive Damage: " + explosion.GetDamage() + " --> <color=green>" + (explosion.GetDamage() + explosionDamageIncrease) + "</color>";
                    break;
                case UpgradeType.ExplosivePierce:
                    description.text = "Explosive Pierce: " + explosion.GetPierce() + " --> <color=green>" + (explosion.GetPierce() + explosionPierceIncrease) + "</color>";
                    break;
                case UpgradeType.ExplosiveSize:
                    description.text = "Explosive Size: " + explosion.GetSize() + " --> <color=green>" + (explosion.GetSize() + explosionSizeIncrease) + "</color>";
                    break;
            }
        }
    }

    public override void Upgrade()
    {
        if (!recentlyUpgraded)
        {
            if (!weapon.activeInHierarchy)
            {
                ActivateWeapon();
            }
            else
            {
                switch (expType)
                {
                    case UpgradeType.Damage:
                        UpgradeDamage();
                        break;
                    case UpgradeType.Pierce:
                        UpgradePierce();
                        break;
                    case UpgradeType.FireSpeed:
                        UpgradeFireSpeed();
                        break;
                    case UpgradeType.AddExplosion:
                        ActivateExplosion();
                        break;
                    case UpgradeType.ExplosiveDamage:
                        UpgradeExplosionDamage();
                        break;
                    case UpgradeType.ExplosivePierce:
                        UpgradeExplosionPierce();
                        break;
                    case UpgradeType.ExplosiveSize:
                        UpgradeExplosionSize();
                        break;
                }
            }
        }

        base.Upgrade();
    }

    public void ActivateExplosion()
    {
        if (!canExplode)
        {
            expProjectile.explodeOnDeath = true;
            canExplode = true;
        }
        else if (!fullyExplodable)
        {
            expProjectile.explodeOnHit = true;
            fullyExplodable = true;
        }
        else
        {
            Debug.LogWarning("The activate explosion function was called even though this weapon already has every unique explosion upgrade.");
        }
        recentlyUpgraded = true;
    }

    public void UpgradeExplosionDamage()
    {
        explosion.IncreaseDamage(explosionDamageIncrease);
        recentlyUpgraded = true;
    }

    public void UpgradeExplosionPierce()
    {
        explosion.IncreasePierce(explosionPierceIncrease);
        recentlyUpgraded = true;
    }

    public void UpgradeExplosionSize()
    {
        explosion.IncreaseSize(explosionSizeIncrease);
        recentlyUpgraded = true;
    }
}
