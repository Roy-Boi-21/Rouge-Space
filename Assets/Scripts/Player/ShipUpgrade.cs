using System;
using System.Collections.Generic;
using UnityEngine;

public class ShipUpgrade : UpgradeComponent
{
    [Header("Ship Upgrade Components")]
    [Tooltip("The ship to upgrade.")]
    [SerializeField] private GameObject ship;

    [Tooltip("How much extra health the ship will get after an upgrade.")]
    [SerializeField] private int healthIncrease = 1;

    [Tooltip("How much faster the ship will get after an upgrade.")]
    [SerializeField] private float speedIncrease = 1f;

    [Tooltip("The spriteRenderer for the upgrade.")]
    [SerializeField] private Sprite rawSprite;

    private PlayerHealth shipHealth;
    private PlayerMovement shipMovement;
    protected UpgradeType currentType = UpgradeType.Health;
    protected List<UpgradeType> availableUpgrades = new List<UpgradeType>();

    protected enum UpgradeType : int
    {
        Health,
        Speed
    }

    public override void Start()
    {
        if (initialized)
        {
            return;
        }

        base.Start();

        if (ship != null)
        {
            shipHealth = ship.GetComponent<PlayerHealth>();
            shipMovement = ship.GetComponent<PlayerMovement>();
            if ((shipHealth == null) || (shipMovement == null))
            {
                Debug.LogError("Ship data not found.");
            }

            //title.text = "placeholder";
            title.color = titleColor;
            //description.text = "placeholder";
            image.sprite = rawSprite;
        }
        else
        {
            Debug.LogError("Ship not found.");
        }
    }

    protected void RefreshAvailableUpgrades()
    {
        availableUpgrades.Clear();

        foreach (UpgradeType upgrade in Enum.GetValues(typeof(UpgradeType)))
        {
            availableUpgrades.Add(upgrade);
        }
    }

    public override void ReloadUpgrade()
    {
        RefreshAvailableUpgrades();

        currentType = availableUpgrades[UnityEngine.Random.Range(0, availableUpgrades.Count)];

        switch (currentType)
        {
            case UpgradeType.Health:
                title.text = "Health Upgrade";
                description.text = "Max Health: " + shipHealth.GetMaxHealth() + " --> <color=green>" + (shipHealth.GetMaxHealth() + healthIncrease) + "</color>";
                break;
            case UpgradeType.Speed:
                title.text = "Speed Upgrade";
                description.text = "Top Speed: " + shipMovement.GetMaxSpeed() + " --> <color=green>" + (shipMovement.GetMaxSpeed() + speedIncrease) + "</color>";
                break;
        }
    }

    public override void Upgrade()
    {
        switch (currentType)
        {
            case UpgradeType.Health:
                UpgradeHealth();
                break;
            case UpgradeType.Speed:
                UpgradeSpeed();
                break;
        }

        base.Upgrade();
    }

    public void UpgradeHealth()
    {
        shipHealth.SetMaxHealth(shipHealth.GetMaxHealth() + healthIncrease);
        shipHealth.Heal(healthIncrease);
    }

    public void UpgradeSpeed()
    {
        shipMovement.SetSpeed(shipMovement.GetMaxSpeed() + speedIncrease);
    }
}
