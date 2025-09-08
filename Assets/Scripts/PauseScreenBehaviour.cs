using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement; // SceneManager
using System.Text;

public class PauseScreenBehaviour : MonoBehaviour
{
    /// <summary> If our game is currently paused </summary>
    public static bool paused;

    [Tooltip("Reference to the pause menu object to turn on/off")]
    public GameObject pauseMenu;

    [Tooltip("Reference to the on screen controls menu")]
    public GameObject onScreenControls;

    /// <summary> Reloads our current level, effectively "restarting" the game </summary>
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary> Will turn our pause menu on or off </summary>
    /// <param name="isPaused"></param>
    public void SetPauseMenu(bool isPaused)
    {
        paused = isPaused;
        /* If the game is paused, timeScale is 0, otherwise 1 */
        Time.timeScale = (paused) ? 0 : 1;
        pauseMenu.SetActive(paused);
        onScreenControls.SetActive(!paused);
    }

    /// <summary> Will load a new scene upon being called </summary>
    /// <param name="levelName">The name of the level we want to go to</param>
    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    void Start()
    {
        /* Must be reset in Start or else game will be paused upon restart */
        //paused = false;
        SetPauseMenu(false);
    }
}