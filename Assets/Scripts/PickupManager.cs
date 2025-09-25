using UnityEngine;
using System.Collections.Generic;

public class PickupManager : MonoBehaviour
{


    [Header("JumpBoost Settings")]
    public Transform jumpBoostPrefab;
    [Range(0f, 1f)]
    public float jumpBoostChance = 0.2f;     // JumpBoost 出现概率
    public float jumpBoostYOffset = 0.5f;    // 道具离 spawn 点的高度

    [Header("SpeedBoost Settings")]
    public Transform speedBoostPrefab;
    [Range(0f, 1f)]
    public float speedBoostChance = 0.2f;    // SpeedBoost 出现概率
    public float speedBoostYOffset = 0.5f;   // 道具离 spawn 点的高度

    [Header("SpoiledFood Settings")]
    public Transform spoiledFoodPrefab;
    [Range(0f, 1f)] public float spoiledFoodChance = 0.3f;
    public float spoiledFoodYOffset = 0.5f;

    [Header("Heal Settings")]
    public Transform healPickupPrefab;
    [Range(0f, 1f)]
    public float healChance = 0.2f;   // 出现概率
    public float healYOffset = 0.5f;  // 离 spawn 点高度

    [Header("Spoiled Food Prefabs")]
    public GameObject[] spoiledFoodPrefabs; // 可以拖多个进来

    [Header("Spawn Points")]
    public List<Transform> pickupSpawnPoints; // Tile 内空中 spawn 点列表

    private List<Transform> activePickups = new List<Transform>();
    private List<Transform> activeSpoiledFoods = new List<Transform>();

    // 传入 Tile 的 JumpBoostSpawn 点列表
    /// <summary>
    /// 根据传入 spawn 点列表生成道具
    /// </summary>
    /// <param name="spawnPoints">Tile 内空中生成点</param>
    public void SpawnPickups(List<Transform> spawnPoints)
    {
        foreach (Transform spawnPoint in spawnPoints)
        {
            bool spawnJump = Random.value < jumpBoostChance;
            bool spawnSpeed = Random.value < speedBoostChance;
            bool spawnHeal = Random.value < healChance;

            // 避免多个道具同点生成
            int count = (spawnJump ? 1 : 0) + (spawnSpeed ? 1 : 0) + (spawnHeal ? 1 : 0);
            if (count > 1)
            {
                // 随机只保留一个
                int choice = Random.Range(0, count);
                spawnJump = spawnSpeed = spawnHeal = false;
                if (choice == 0) spawnJump = true;
                else if (choice == 1) spawnSpeed = true;
                else spawnHeal = true;
            }

            // 生成 JumpBoost
            if (spawnJump && jumpBoostPrefab != null)
            {
                Vector3 spawnPos = spawnPoint.position + Vector3.up * jumpBoostYOffset;
                Transform jumpBoost = Instantiate(jumpBoostPrefab, spawnPos, Quaternion.identity, spawnPoint);
                activePickups.Add(jumpBoost);
            }

            // 生成 SpeedBoost
            if (spawnSpeed && speedBoostPrefab != null)
            {
                Vector3 spawnPos = spawnPoint.position + Vector3.up * speedBoostYOffset;
                Transform speedBoost = Instantiate(speedBoostPrefab, spawnPos, Quaternion.identity, spawnPoint);
                activePickups.Add(speedBoost);
            }

            // 生成 Heal
            if (spawnHeal && healPickupPrefab != null)
            {
                Vector3 spawnPos = spawnPoint.position + Vector3.up * healYOffset;
                Transform healPickup = Instantiate(healPickupPrefab, spawnPos, Quaternion.identity, spawnPoint);
                activePickups.Add(healPickup);
            }
        }
    }

    /// <summary>
    /// 随机生成一个 JumpBoost（用于单独生成）
    /// </summary>
    /// <param name="spawnPoints">Tile 内空中生成点</param>
    public void TrySpawnJumpBoost(List<Transform> spawnPoints)
    {
        if (spawnPoints == null || spawnPoints.Count == 0 || jumpBoostPrefab == null) return;
        if (Random.value > jumpBoostChance) return;

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
        Vector3 spawnPos = spawnPoint.position + Vector3.up * jumpBoostYOffset;
        Transform jumpBoost = Instantiate(jumpBoostPrefab, spawnPos, Quaternion.identity, spawnPoint);
        activePickups.Add(jumpBoost);
    }

    /// <summary>
    /// 清理所有已生成的道具（回收 Tile 时调用）
    /// </summary>
    public void ClearPickups()
    {
        foreach (var pickup in activePickups)
        {
            if (pickup != null)
                Destroy(pickup.gameObject);
        }
        activePickups.Clear();
    }

    public void SpawnSpoiledFood(List<Transform> spawnPoints)
    {
        if (spoiledFoodPrefab == null || spawnPoints.Count == 0) return;


        if (Random.value < spoiledFoodChance)
        {
            // 随机一个生成点
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

            // 随机一个 SpoiledFood prefab
            int index = Random.Range(0, spoiledFoodPrefabs.Length);
            GameObject prefab = spoiledFoodPrefabs[index];

            // 生成位置
            Vector3 spawnPos = spawnPoint.position + Vector3.up * spoiledFoodYOffset;

            // 生成并挂在 spawnPoint 下面
            Transform spoiled = Instantiate(prefab, spawnPos, Quaternion.identity, spawnPoint).transform;
            activeSpoiledFoods.Add(spoiled);
        }

    }
}
