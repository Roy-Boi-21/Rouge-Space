using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

public class DefeatManager : MonoBehaviour
{
    [Tooltip("The shade for the defeat screen.")]
    [SerializeField] private GameObject shade;

    [Tooltip("The title for the defeat screen.")]
    [SerializeField] private TMP_Text title;

    [Tooltip("The title's text.")]
    [SerializeField] private string titleRaw = "GAME OVER";

    [Tooltip("The text that displays the user's score.")]
    [SerializeField] private TMP_Text scoreText;

    [Tooltip("The score's prefix.")]
    [SerializeField] private string scorePrefix = "Score: ";

    [Tooltip("The text that displays the user's highest wave.")]
    [SerializeField] private TMP_Text waveText;

    [Tooltip("The wave's prefix.")]
    [SerializeField] private string wavePrefix = "Wave: ";

    [Tooltip("The list of buttons in the defeat menu.")]
    [SerializeField] private GameObject[] buttons;

    [Tooltip("How long it takes for the defeat screen to show up after the player dies.")]
    [SerializeField] private float loadTime = 2f;

    public static DefeatManager instance;
    private bool isDefeated = false;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        // Hide everything.
        foreach (GameObject button in buttons)
        {
            button.SetActive(false);
        }
        shade.SetActive(false);

        title.text = "";
        scoreText.text = "";
        waveText.text = "";
    }

    private IEnumerator ShowDefeatMenu()
    {
        yield return new WaitForSeconds(loadTime);

        foreach (GameObject button in buttons)
        {
            button.SetActive(true);
        }
        shade.SetActive(true);

        title.text = titleRaw;
        scoreText.text = scorePrefix + ScoreManager.instance.GetScore();
        waveText.text = wavePrefix + EnemyManager.instance.GetWave();

        EventSystem.current.SetSelectedGameObject(buttons[0]);
    }

    public void OpenDefeatMenu()
    {
        if (PauseManager.instance != null)
        {
            PauseManager.instance.DisablePausing();
        }

        StartCoroutine(ShowDefeatMenu());
    }

    public bool IsDefeated()
    {
        return isDefeated;
    }
}
