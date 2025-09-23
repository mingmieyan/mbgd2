using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HealthManager : MonoBehaviour
{
    public int maxHealth = 3;
    public int currentHealth;
    public Transform heartContainer;     // Canvas 下的父物体
    public GameObject heartPrefab;       // 爱心图标 Prefab

    private List<GameObject> hearts = new List<GameObject>();

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHearts();
    }

    // 更新UI显示
    public void UpdateHearts()
    {
        // 先清空旧图标
        foreach (var h in hearts)
            Destroy(h);
        hearts.Clear();

        // 创建对应数量的血量图标
        for (int i = 0; i < currentHealth; i++)
        {
            GameObject heart = Instantiate(heartPrefab, heartContainer);
            hearts.Add(heart);
        }
    }

    // 扣血方法
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;

        UpdateHearts();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
       
    }
}
