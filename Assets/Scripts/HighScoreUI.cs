using UnityEngine;
using UnityEngine.UI;

public class HighScoreUI : MonoBehaviour
{
    public Text highScoreText;

    void Start()
    {
        // 从 PlayerPrefs 读取最高分
        int highScore = PlayerPrefs.GetInt("HighScore", 0);

        if (highScoreText != null)
            highScoreText.text = highScore.ToString();
    }
}
