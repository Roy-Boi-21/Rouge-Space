using UnityEngine;
using UnityEngine.EventSystems;

public class SettingsManager : MonoBehaviour
{
    [Tooltip("The parent game object that contains every UI element in the settings menu.")]
    [SerializeField] private GameObject menu;

    [Tooltip("The button that becomes selected when the player opens up the settings menu.")]
    [SerializeField] private GameObject firstButton;

    [Tooltip("The button that becomes selected when the player closes the settings menu.")]
    [SerializeField] private GameObject returnButton;

    [Tooltip("Whether the settings should open up the pause menu when the settings close.")]
    [SerializeField] private bool openPauseMenu = true;

    public static SettingsManager instance;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        menu.SetActive(false);
    }

    public void OpenSettingsMenu()
    {
        menu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstButton);
    }

    public void CloseSettingsMenu()
    {
        if (PauseManager.instance != null && openPauseMenu)
        {
            PauseManager.instance.OpenPauseMenu();
        }
        EventSystem.current.SetSelectedGameObject(returnButton);
        menu.SetActive(false);
    }
}
