using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement; // LoadScene
using UnityEngine.UI;

public class MainMenuBehaviour : MonoBehaviour
{
    /// <summary>
    /// Will load a new scene upon being called
    /// </summary>
    /// <param name="levelName">The name of the level we want to go to</param>
    public void LoadLevel1(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    public void LoadLevel2(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    public void LoadLevelEndless(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    public void EndGame()
    {
        Application.Quit();
    }
}