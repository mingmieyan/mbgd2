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
    public void SpawnPickups(List<Transform> spawnPoints, Transform parentTile)
    {
        foreach (Transform spawnPoint in spawnPoints)
        {
            bool spawnJump = Random.value < jumpBoostChance;
            bool spawnSpeed = Random.value < speedBoostChance;
            bool spawnHeal = Random.value < healChance;

            int count = (spawnJump ? 1 : 0) + (spawnSpeed ? 1 : 0) + (spawnHeal ? 1 : 0);
            if (count > 1)
            {
                int choice = Random.Range(0, count);
                spawnJump = spawnSpeed = spawnHeal = false;
                if (choice == 0) spawnJump = true;
                else if (choice == 1) spawnSpeed = true;
                else spawnHeal = true;
            }

            Transform newPickup = null;

            if (spawnJump && jumpBoostPrefab != null)
                newPickup = Instantiate(jumpBoostPrefab, spawnPoint.position + Vector3.up * jumpBoostYOffset, Quaternion.identity, spawnPoint);
            else if (spawnSpeed && speedBoostPrefab != null)
                newPickup = Instantiate(speedBoostPrefab, spawnPoint.position + Vector3.up * speedBoostYOffset, Quaternion.identity, spawnPoint);
            else if (spawnHeal && healPickupPrefab != null)
                newPickup = Instantiate(healPickupPrefab, spawnPoint.position + Vector3.up * healYOffset, Quaternion.identity, spawnPoint);

            if (newPickup != null)
            {
                var id = newPickup.gameObject.AddComponent<PickupIdentifier>();
                id.parentTile = parentTile;
                activePickups.Add(newPickup);
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

    public void SpawnSpoiledFood(List<Transform> spawnPoints, Transform parentTile)
    {
        if (spoiledFoodPrefabs == null || spoiledFoodPrefabs.Length == 0 || spawnPoints.Count == 0) return;

        if (Random.value < spoiledFoodChance)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
            int index = Random.Range(0, spoiledFoodPrefabs.Length);
            GameObject prefab = spoiledFoodPrefabs[index];

            Transform spoiled = Instantiate(prefab, spawnPoint.position + Vector3.up * spoiledFoodYOffset, Quaternion.identity, spawnPoint).transform;
            var id = spoiled.gameObject.AddComponent<PickupIdentifier>();
            id.parentTile = parentTile;
            activeSpoiledFoods.Add(spoiled);
        }
    }

    public void ClearPickupsForTile(Transform tile)
    {
        // activePickups
        for (int i = activePickups.Count - 1; i >= 0; i--)
        {
            var p = activePickups[i];
            if (p == null) { activePickups.RemoveAt(i); continue; }
            var id = p.GetComponent<PickupIdentifier>();
            if (id != null && id.parentTile == tile)
            {
                Destroy(p.gameObject);
                activePickups.RemoveAt(i);
            }
        }

        // activeSpoiledFoods
        for (int i = activeSpoiledFoods.Count - 1; i >= 0; i--)
        {
            var p = activeSpoiledFoods[i];
            if (p == null) { activeSpoiledFoods.RemoveAt(i); continue; }
            var id = p.GetComponent<PickupIdentifier>();
            if (id != null && id.parentTile == tile)
            {
                Destroy(p.gameObject);
                activeSpoiledFoods.RemoveAt(i);
            }
        }
    }
}
