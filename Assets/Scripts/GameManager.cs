
using UnityEngine;
using System.Collections.Generic;
using TMPro; // List


/// <summary>
/// Manages the main gameplay of the game
/// </summary>
public class GameManager : MonoBehaviour
{
    [Tooltip("A reference to the tile we want to spawn")]
 public Transform tile;

    [Tooltip("A reference to the obstacle we want tospawn")]
 public Transform obstacle;

    [Tooltip("Where the first tile should be placed at")]
 public Vector3 startPoint = new Vector3(0, 0, -5);

    [Tooltip("How many tiles should we create in advance")]
 [Range(1, 15)]

    public int initSpawnNum = 10;
    [Tooltip("How many tiles to spawn with noobstacles")]
     public int initNoObstacles = 4;


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
    public TMP_Text ScoreText;

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    private void Start()
    {
        // Set our starting point
        nextTileLocation = startPoint;
        nextTileRotation = Quaternion.identity;

        for (int i = 0; i < initSpawnNum; ++i)
        {
            SpawnNextTile(i >= initNoObstacles);
        }
    }
    /// <summary>
    /// Will spawn a tile at a certain location and
    /// setup the next position
    /// </summary>
    /// <param name="spawnObstacles">If we should spawn an
    /// obstacle</param>
    public void SpawnNextTile(bool spawnObstacles = true)
    {
        var newTile = Instantiate(tile, nextTileLocation,
        nextTileRotation);


        // 保存到列表，方便回收
        spawnedTiles.Add(newTile);
        // Figure out where and at what rotation we should
        // spawn the next item
        var nextTile = newTile.Find("Next Spawn Point");
        nextTileLocation = nextTile.position;
        nextTileRotation = nextTile.rotation;

        if (spawnObstacles && obstacle != null)
        {
            SpawnObstacle(newTile);
        }
    }
        private void SpawnObstacle(Transform newTile)
    {
        // Now we need to get all of the possible places
        // to spawn the obstacle
        var obstacleSpawnPoints = new List<GameObject>();
        // Go through each of the child game objects in
        // our tile
        foreach (Transform child in newTile)
        {
            // If it has the ObstacleSpawn tag
            if (child.CompareTag("ObstacleSpawn"))
            {
                // We add it as a possibility
                obstacleSpawnPoints.Add(child.gameObject);
            }
        }
        // Make sure there is at least one
        if (obstacleSpawnPoints.Count > 0)
        {
            // Get a random spawn point from the ones we
            // have
            int index = Random.Range(0,
            obstacleSpawnPoints.Count);
            var spawnPoint = obstacleSpawnPoints[index];
            // Store its position for us to use
            var spawnPos = spawnPoint.transform.position;
            // Create our obstacle
            var newObstacle = Instantiate(obstacle,
            spawnPos, Quaternion.identity);
            // Have it parented to the tile
            newObstacle.SetParent(spawnPoint.transform);
        }
    }

      /// <summary>
    /// 回收最早的 tile（防止场景里无限堆积）
    /// </summary>
    public void RecycleOldestTile()
    {
        if (spawnedTiles.Count > initSpawnNum) // 超过初始数量就回收
        {
            var oldTile = spawnedTiles[0];
            spawnedTiles.RemoveAt(0);
            Destroy(oldTile.gameObject);
        }
    }

    public void AddScore(int score)
    {
        Score += score;
        ScoreText.text = Score.ToString();
        PlayerPrefs.SetInt("Score", Score);
    }
}

