/// THIS SCRIPT IS UNUSED AND ALL GAME CLOSING ACTION SHOULD BE REDIRECTED TO THE TRANSITION MANAGER.
using UnityEngine;

public class QuitGame : MonoBehaviour
{
    [Tooltip("Check if the game should quit if the player hits the menu button.")]
    [SerializeField] private bool closeOnMenu = false;

    private void Update()
    {
        if (closeOnMenu && UserInput.instance.pauseInput) 
        {
            CloseGame();
        }
    }

    public void CloseGame()
    {
#if UNITY_EDITOR
        Debug.Log("Game quit! Exiting play mode...");
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Debug.Log("Game quit! Closing application...");
        Application.Quit();
#endif
    }
}
