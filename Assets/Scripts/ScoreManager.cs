using UnityEngine;
using UnityEngine.UI; // UI Text (Legacy)


public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("Score Settings")]
    public float scoreMultiplier = 1f; // 每跑 1 个单位距离加多少分
    public Text scoreText;             // 绑定到 UI Text
    private Transform player;          // 玩家位置
    private float startZ;              // 起始位置
    public int score;
    private int scoreFromDistance;
    private int scoreFromItems;

    void Awake()
    {
        // 单例模式
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        // 找到玩家
        player = GameObject.FindGameObjectWithTag("Player").transform;
        startZ = player.position.z;
        score = 0;
    }

    void Update()
    {
        if (player == null) return;

        scoreFromDistance = Mathf.FloorToInt((player.position.z - startZ) * scoreMultiplier);
        score = scoreFromDistance + scoreFromItems;

       

        // 显示到 UI
        if (scoreText != null)
            scoreText.text = score.ToString();
    }

    // 额外加分接口（比如吃到物品时调用）
    public void AddScore(int amount)
    {
        scoreFromItems += amount; // ← 改这里

        // 更新 UI
        if (scoreText != null)
            scoreText.text = (scoreFromDistance + scoreFromItems).ToString();
    }
    public int GetScore()
    {
        return score;
    }


}