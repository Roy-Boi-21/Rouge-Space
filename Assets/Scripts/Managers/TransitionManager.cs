using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    [Tooltip("The image that fades in and out.")]
    [SerializeField] private Image image;

    [Tooltip("The color that's seen when fading into a new scene or out of a scene.")]
    [SerializeField] private Color hiddenColor = Color.black;

    [Tooltip("The color that's seen during active gameplay.")]
    [SerializeField] private Color visibleColor = Color.clear;

    [Tooltip("How many seconds it takes for the fade to complete.")]
    [SerializeField] private float fadeTime = 1f;

    [Tooltip("The amount of time that must pass before transitioning to a new scene.")]
    [SerializeField] private float transitionTime = 2f;

    public static TransitionManager instance;

    private bool transitioning = false;

    private void Awake()
    {
        if (instance == null) 
        {
            instance = this;
        }

        if (image == null)
        {
            Debug.LogError("Image asset not provided.  Fading will not work.");
        }
    }

    private void Start()
    {
        StartCoroutine(FadeColorCoroutine(hiddenColor, visibleColor));
    }

    public void ReloadScene()
    {
        if (!transitioning)
        {
            transitioning = true;
            MusicManager.instance.FadeOutMusic();
            StartCoroutine(FadeColorCoroutine(visibleColor, hiddenColor));
            StartCoroutine(LoadSceneCoroutine(SceneManager.GetActiveScene().name));
        }
    }

    public void LoadScene(string sceneName)
    {
        if (!transitioning)
        {
            transitioning = true;
            MusicManager.instance.FadeOutMusic();
            StartCoroutine(FadeColorCoroutine(visibleColor, hiddenColor));
            StartCoroutine(LoadSceneCoroutine(sceneName));
        }
    }

    public void CloseGame()
    {
        if (!transitioning)
        {
            transitioning = true;
            MusicManager.instance.FadeOutMusic();
            StartCoroutine(FadeColorCoroutine(visibleColor, hiddenColor));
            StartCoroutine(CloseGameCoroutine());
        }
    }

    private IEnumerator FadeColorCoroutine(Color startColor, Color endColor)
    {
        image.color = startColor;

        float timeElapsed = 0f;
        while (timeElapsed <= fadeTime)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            timeElapsed += Time.deltaTime;
            image.color = Color.Lerp(startColor, endColor, timeElapsed / fadeTime);
        }

        image.color = endColor;
    }

    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator CloseGameCoroutine()
    {
        yield return new WaitForSeconds(transitionTime);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnValidate()
    {
        transitionTime = Mathf.Max(0f, transitionTime);
    }
}
