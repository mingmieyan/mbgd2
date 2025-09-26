
using UnityEngine;
using System.Collections.Generic; // List
using UnityEngine.UI;   // UI
using UnityEngine.SceneManagement;


/// <summary>
/// Manages the main gameplay of the game
/// </summary>
public class GameManager : MonoBehaviour
{
    [Tooltip("A reference to the tile we want to spawn")]
 public Transform tile;

    [Tooltip("Where the first tile should be placed at")]
 public Vector3 startPoint = new Vector3(0, 0, -5);
    
    [Tooltip("How many tiles should we create in advance")]
 [Range(1, 15)]

    public int initSpawnNum = 10;
    [Tooltip("How many tiles to spawn with noobstacles")]
     public int initNoObstacles = 4;

    [Header("Spawn Settings")]
    [Tooltip("障碍物预制体")]
    public Transform obstacle;

    [Tooltip("卷心菜预制体")]
    public Transform cabbage;

    [Tooltip("障碍物生成概率 (0-1)")]
    [Range(0f, 1f)] public float obstacleChance = 0.5f;

    [Tooltip("卷心菜生成概率 (0-1)")]
    [Range(0f, 1f)] public float cabbageChance = 0.5f;

    [Tooltip("卷心菜和障碍物的最小间距")]
    public float minDistance = 3f;

    [Header("Game Timer")]
    public float levelTime = 60f; // 倒计时 (秒)

    [Header("UI References")]
    public Text timerText;          // 倒计时 UI
    public Text scoreText;          // 当前分数 UI
    public Text gameOverScoreText;  // GameOver 菜单显示当前分数
    public Text gameOverHighScoreText; // GameOver 菜单显示最高分
    public GameObject gameOverMenu; // GameOver 菜单

    [Header("Game Settings")]
    public float totalTime = 60f;   // 游戏时长
    private float remainingTime;
    private bool isGameOver = false;

    [Header("Game Modes")]
    public bool isEndlessMode = false; // 勾选后为无尽模式

    private int score = 0;          // 当前分数
    private int highScore;          // 最高分

    public PickupManager pickupManager;//apply Other code

    [Tooltip("苹果预制体")]
    public Transform apple;
    [Tooltip("苹果生成概率 (0-1)")]
    [Range(0f, 1f)] public float appleChance = 0.3f;

    // 存放生成出来的 tile
    private List<Transform> spawnedTiles = new List<Transform>();
    /// <summary>
    /// Where the next tile should be spawned at.
    /// </summary>
    private Vector3 nextTileLocation;
    /// <summary>
    /// How should the next tile be rotated?
    /// </summary>
    private Quaternion nextTileRotation;

    public int Score;
    

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
      private Transform player;
     void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Set our starting point
        nextTileLocation = startPoint;
        nextTileRotation = Quaternion.identity;

        for (int i = 0; i < initSpawnNum; ++i)
        {
            SpawnNextTile(i >= initNoObstacles);
        }

        remainingTime = totalTime;

        // 读取最高分
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateScoreUI();

        if (gameOverMenu != null)
            gameOverMenu.SetActive(false);

        // 如果是无尽模式 -> 隐藏时间 UI
        if (isEndlessMode && timerText != null)
            timerText.gameObject.SetActive(false);
    }


    /// <summary>
    /// Will spawn a tile at a certain location and
    /// setup the next position
    /// </summary>
    /// <param name="spawnObstacles">If we should spawn an
    /// obstacle</param>
    /// 
    public bool gameOver = false;
    void Update()
    {
        if (gameOver) return;

        // 无尽模式不需要倒计时
        if (!isEndlessMode)
        {
            // 正常倒计时逻辑
            if (levelTime > 0f)
            {
                levelTime -= Time.deltaTime;
                if (levelTime < 0f) levelTime = 0f;
                UpdateTimerUI();
            }

            remainingTime -= Time.deltaTime;
            if (remainingTime <= 0)
            {
                remainingTime = 0;
                GameOver();
            }
        }

        // 玩家接近生成点时生成 Tile
        if (player != null && Vector3.Distance(player.position, nextTileLocation) < 200f)
        {
            for (int i = 0; i < 7; i++) SpawnNextTile(true);
            RecycleOldestTile();
        }

        // 更新分数
        score = ScoreManager.Instance != null ? ScoreManager.Instance.GetScore() : score;
    }
    public void SpawnNextTile(bool spawnObstacles = true)
    {

        var newTile = Instantiate(tile, nextTileLocation,
        nextTileRotation);
        spawnedTiles.Add(newTile);


        // 保存到列表，方便回收
        spawnedTiles.Add(newTile);
        // Figure out where and at what rotation we should
        // spawn the next item
        var nextTile = newTile.Find("Next Spawn Point");
        nextTileLocation = nextTile.position;
        nextTileRotation = nextTile.rotation;

        if (spawnObstacles && obstacle != null)
        {
            TrySpawnObjects(newTile);
        }

        // 获取 Tile 下所有 JumpBoostSpawn 点
        List<Transform> pickupSpawnPoints = new List<Transform>();
        foreach (Transform child in newTile)
        {
            if (child.CompareTag("pickupSpawnPoints")) // 这里的标签就是你加的
                pickupSpawnPoints.Add(child);
        }

        pickupManager.SpawnPickups(pickupSpawnPoints, newTile);

        // 假设 pickupManager 已经挂在场景里
        // 每个 Tile 的变质食物生成点列表
        List<Transform> spoiledFoodPoints = new List<Transform>();
        foreach (Transform child in newTile)
        {
            if (child.CompareTag("SpoiledFoodSpawn"))
                spoiledFoodPoints.Add(child);
        }

        // 调用生成
        pickupManager.SpawnSpoiledFood(spoiledFoodPoints, newTile);
    }
    private void TrySpawnObjects(Transform newTile)
    {
        var spawnPoints = new List<Transform>();
        foreach (Transform child in newTile)
        {
            if (child.CompareTag("ObstacleSpawn"))
                spawnPoints.Add(child);
        }

        if (spawnPoints.Count == 0) return;

        // 随机选一个点来放障碍物

        Transform obstaclePoint = null;

        if (Random.value < obstacleChance && obstacle != null)
        {
            obstaclePoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
            var newObstacle = Instantiate(obstacle, obstaclePoint.position, Quaternion.identity);
            newObstacle.SetParent(obstaclePoint);

        }

        // 2. 其他点放蔬菜（卷心菜/苹果）
        foreach (var spawnPoint in spawnPoints)
        {
            // 跳过障碍物点
            if (spawnPoint == obstaclePoint) continue;

            float roll = Random.value;

            if (roll < cabbageChance) // 卷心菜
            {
                if (cabbage != null)
                {
                    var newCabbage = Instantiate(cabbage, spawnPoint.position + Vector3.up * 0.5f, Quaternion.identity);
                    newCabbage.SetParent(spawnPoint);
                }
            }
            else if (roll < cabbageChance + appleChance) // 苹果
            {
                if (apple != null)
                {
                    var newApple = Instantiate(apple, spawnPoint.position + Vector3.up * 0.5f, Quaternion.identity);
                    newApple.SetParent(spawnPoint);
                }
            }
            // roll >= cabbageChance + appleChance → 什么都不生成
        }
    }
    /// <summary>
    /// 回收最早的 tile（防止场景里无限堆积）
    /// </summary>
    public void RecycleOldestTile()
    {
        if (spawnedTiles.Count > initSpawnNum)
        {
            var oldTile = spawnedTiles[0];
            spawnedTiles.RemoveAt(0);

            // 只清理属于 oldTile 的道具
            if (pickupManager != null)
                pickupManager.ClearPickupsForTile(oldTile);

            Destroy(oldTile.gameObject);
        }
    }



    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int seconds = Mathf.CeilToInt(remainingTime);
            timerText.text = seconds.ToString();
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = score.ToString();
    }

    /// <summary>
    /// 游戏中加分调用
    /// </summary>
    public void AddScore(int amount)
    {
        if (isGameOver) return;

        score += amount;
        UpdateScoreUI();
    }

    private int finalScore = 0;
    public void GameOver()
    {
        if (isGameOver) return; // 防止重复调用
        isGameOver = true;
        // 保存最终分数
        finalScore = ScoreManager.Instance?.GetScore() ?? score;

        // 停止游戏
        Time.timeScale = 0f;

        // 显示 GameOver 菜单
        if (gameOverMenu != null)
        {
            gameOverMenu.SetActive(true);
            gameOverScoreText.text = finalScore.ToString();

            // 检查最高分
            int highScore = PlayerPrefs.GetInt("HighScore", 0);
            if (finalScore > highScore)
            {
                highScore = finalScore;
                PlayerPrefs.SetInt("HighScore", highScore);
                PlayerPrefs.Save();
            }

            gameOverHighScoreText.text =  highScore.ToString();
        }
    }

    // 重开按钮调用
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // 返回主菜单按钮调用
    public void QuitToMenu(string menuSceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }
}

