using UnityEngine;

public class PauseButton : MonoBehaviour
{
    [Tooltip("The pause button.")]
    [SerializeField] private GameObject pauseButton;

    private void LateUpdate()
    {
        if (PauseManager.instance != null)
        {
            pauseButton.SetActive(PauseManager.instance.CanPause());
        }
    }
}
