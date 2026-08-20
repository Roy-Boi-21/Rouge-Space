using UnityEngine;
using System;
using System.Collections.Generic;

public class RadialUpgrade : WeaponUpgrade
{
    [Header("Radial Upgrade Components")]
    [Tooltip("How many more projectiles should each attack shoot out after getting an upgrade.")]
    [SerializeField] private int projectileCountIncrease = 2;

    [Tooltip("The maximum amount of projectiles this weapon can shoot out at once.")]
    [SerializeField] private int projectileLimit = 32;

    protected RadialPlayerAttack radialData;
    protected UpgradeType radialType = UpgradeType.Damage;
    protected List<UpgradeType> radialUpgrades = new List<UpgradeType>();

    new protected enum UpgradeType : int
    {
        Damage,
        Pierce,
        FireSpeed,
        MoreProjectiles
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        if (initialized)
        {
            return;
        }

        base.Start();

        radialData = weapon.GetComponent<RadialPlayerAttack>();

        if (radialData == null)
        {
            Debug.LogError("Radial weapon data not found.");
        }
    }

    protected override void RefreshAvailableUpgrades()
    {
        radialUpgrades.Clear();

        foreach (UpgradeType upgrade in Enum.GetValues(typeof(UpgradeType)))
        {
            radialUpgrades.Add(upgrade);
        }

        if (damageIncrease <= 0)
        {
            radialUpgrades.Remove(UpgradeType.Damage);
        }
        if (pierceIncrease <= 0)
        {
            radialUpgrades.Remove(UpgradeType.Pierce);
        }
        if ((fireSpeedMultiplier == 1f) || ((1f / weaponData.GetLaunchDelay()) >= fireSpeedLimit))
        {
            radialUpgrades.Remove(UpgradeType.FireSpeed);
        }

        if (radialData.GetProjectileCount() >= projectileLimit)
        {
            radialUpgrades.Remove(UpgradeType.MoreProjectiles);
        }
    }

    public override void ReloadUpgrade()
    {
        RefreshAvailableUpgrades();

        radialType = radialUpgrades[UnityEngine.Random.Range(0, radialUpgrades.Count)];

        if (weapon.activeInHierarchy)
        {
            switch (radialType)
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
                case UpgradeType.MoreProjectiles:
                    description.text = "Projectiles: " + radialData.GetProjectileCount() + " --> <color=green>" + (radialData.GetProjectileCount() + projectileCountIncrease) + "</color>";
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
                switch (radialType)
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
                    case UpgradeType.MoreProjectiles:
                        UpgradeProjectileCount();
                        break;
                }
            }
        }

        base.Upgrade();
    }

    public void UpgradeProjectileCount()
    {
        radialData.IncreaseProjectileCount(projectileCountIncrease);
        recentlyUpgraded = true;
    }
}
