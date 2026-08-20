using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUpgrade : UpgradeComponent
{
    [Header("Weapon Upgrade Components")]
    [Tooltip("The weapon to upgrade.")]
    [SerializeField] protected GameObject weapon;

    [Tooltip("How much extra damage the weapon will get after an upgrade.  Set to 0 to disable these upgrades.")]
    [SerializeField] protected int damageIncrease = 1;

    [Tooltip("How much extra pierce the weapon will get after an upgrade.  Set to 0 to disable these upgrades.")]
    [SerializeField] protected int pierceIncrease = 1;

    [Tooltip("How much extra firing speed the weapon will get after an upgrade.  Set to 1 to disable these upgrades.")]
    [SerializeField] protected float fireSpeedMultiplier = 1.1f;

    [Tooltip("The highest possible fire rate the player can get from upgrades.")]
    [SerializeField] protected float fireSpeedLimit = 25f;

    protected PlayerProjectileAttack weaponData;
    protected ProjectileComponent projectile;
    protected UpgradeType currentType = UpgradeType.Damage;
    protected List<UpgradeType> availableUpgrades = new List<UpgradeType>();

    protected enum UpgradeType : int
    {
        Damage,
        Pierce,
        FireSpeed
    }

    public override void Start()
    {
        if (initialized)
        {
            return;
        }

        base.Start();

        if (weapon != null)
        {
            weaponData = weapon.GetComponent<PlayerProjectileAttack>();
            if (weaponData != null)
            {
                title.text = weaponData.GetTitle();
                title.color = titleColor;
                description.text = weaponData.GetDescription();
                image.sprite = weaponData.GetImage();
                image.color = titleColor;
                projectile = weaponData.GetProjectile().GetComponent<ProjectileComponent>();
            }
            else
            {
                Debug.LogError("Weapon data not found.");
            }
        }
        else
        {
            Debug.LogError("Weapon not found.");
        }
    }

    protected virtual void RefreshAvailableUpgrades()
    {
        availableUpgrades.Clear();

        foreach (UpgradeType upgrade in Enum.GetValues(typeof(UpgradeType)))
        {
            availableUpgrades.Add(upgrade);
        }

        if (damageIncrease <= 0)
        {
            availableUpgrades.Remove(UpgradeType.Damage);
        }
        if (pierceIncrease <= 0)
        {
            availableUpgrades.Remove(UpgradeType.Pierce);
        }
        if ((fireSpeedMultiplier == 1f) || ((1f / weaponData.GetLaunchDelay()) >= fireSpeedLimit))
        {
            availableUpgrades.Remove(UpgradeType.FireSpeed);
        }
    }

    public override void ReloadUpgrade()
    {
        RefreshAvailableUpgrades();

        currentType = availableUpgrades[UnityEngine.Random.Range(0, availableUpgrades.Count)];

        if (weapon.activeInHierarchy)
        {
            switch (currentType)
            {
                case UpgradeType.Damage:
                    description.text = "Damage: " + projectile.GetDamage() + " --> <color=green>" + (projectile.GetDamage() + damageIncrease) + "</color>";
                    break;
                case UpgradeType.Pierce:
                    description.text = "Pierce: " + projectile.GetPierce() + " --> <color=green>" + (projectile.GetPierce() + pierceIncrease) + "</color>";
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
                switch (currentType)
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
                }
            }
        }

        base.Upgrade();
    }

    public void ActivateWeapon()
    {
        weapon.SetActive(true);
        recentlyUpgraded = true;
    }

    public void UpgradeDamage()
    {
        projectile.IncreaseDamage(damageIncrease);
        recentlyUpgraded = true;
    }

    public void UpgradePierce()
    {
        projectile.IncreasePierce(pierceIncrease);
        recentlyUpgraded = true;
    }

    public void UpgradeFireSpeed()
    {
        float projectedFireSpeed = 1f / (weaponData.GetLaunchDelay() / fireSpeedMultiplier);
        if (projectedFireSpeed >= fireSpeedLimit)
        {
            projectedFireSpeed = fireSpeedLimit;
            weaponData.SetLaunchDelay(1f / projectedFireSpeed);
        }
        else
        {
            weaponData.DecreaseLaunchDelay(fireSpeedMultiplier);
        }
        recentlyUpgraded = true;
    }
}
