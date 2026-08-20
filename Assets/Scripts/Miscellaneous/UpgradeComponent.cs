using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeComponent : MonoBehaviour
{
    [Header("Universal Upgrade Components")]
    [Tooltip("The upgrade's title.")]
    [SerializeField] protected TMP_Text title;

    [Tooltip("The color for the title.")]
    [SerializeField] protected Color titleColor = Color.white;

    [Tooltip("The upgrade's description.")]
    [SerializeField] protected TMP_Text description;

    [Tooltip("The upgrade's image.")]
    [SerializeField] protected Image image;

    [Tooltip("The upgrade's button.")]
    [SerializeField] protected GameObject button;

    // This prevents weapons from recieving multiple upgrades at once from their parents.
    protected bool recentlyUpgraded = false;
    protected bool initialized = false;

    public virtual void Start()
    {
        initialized = true;
    }

    public virtual void ReloadUpgrade()
    {
        // TODO: Randomize the data for the upgrade.
        Debug.LogWarning("Base Upgrade Component ReloadUpgrade was called.  The base upgrade function does nothing.");
    }

    public virtual void Upgrade()
    {
        UpgradeManager.instance.CloseUpgradeMenu();
        recentlyUpgraded = false;
    }

    public virtual GameObject GetButton()
    {
        return button;
    }
}
