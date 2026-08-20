using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class PauseManager : MonoBehaviour
{
    [Tooltip("The title for the pause screen.")]
    [SerializeField] private TMP_Text title;

    [Tooltip("The text for the pause screen's title.")]
    [SerializeField] private string titleText = "GAME PAUSED";

    [Tooltip("The shade for the pause screen.")]
    [SerializeField] private GameObject shade;

    [Tooltip("The button that pauses the game while the game is active.")]
    [SerializeField] private GameObject pauseButton;

    [Tooltip("The pause screen's buttons.")]
    [SerializeField] private GameObject[] buttons;

    public static PauseManager instance;
    private bool isPaused;
    private bool canPause = true;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        ClosePauseMenu();
    }

    private void Update()
    {
        if (canPause && UserInput.instance.pauseInput)
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        MusicManager.instance.MuffleMusic();

        OpenPauseMenu();

        FreezeTime();
    }

    public void ResumeGame()
    {
        MusicManager.instance.UnmuffleMusic();

        ClosePauseMenu();

        UnfreezeTime();
    }

    public void FreezeTime()
    {
        Time.timeScale = 0f;
        isPaused = true;
        UpdatePauseButtonVisibility();
    }

    public void UnfreezeTime()
    {
        Time.timeScale = 1f;
        isPaused = false;
        UpdatePauseButtonVisibility();
    }

    public void OpenPauseMenu()
    {
        title.text = titleText;

        foreach (GameObject button in buttons)
        {
            button.SetActive(true);
        }

        EventSystem.current.SetSelectedGameObject(buttons[0]);

        shade.SetActive(true);
    }

    public void ClosePauseMenu() 
    {
        title.text = "";

        EventSystem.current.SetSelectedGameObject(null);

        foreach (GameObject button in buttons)
        {
            button.SetActive(false);
        }

        shade.SetActive(false);
    }

    public void DisablePausing()
    {
        canPause = false;
        UpdatePauseButtonVisibility();
    }

    public void EnablePausing()
    {
        canPause = true;
        UpdatePauseButtonVisibility();
    }

    private void UpdatePauseButtonVisibility()
    {
        pauseButton.SetActive(canPause && !isPaused);
    }

    public bool CanPause() 
    {
        return canPause;
    }
}
