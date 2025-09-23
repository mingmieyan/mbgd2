using UnityEngine;
using System.Collections;

public class SpeedBoostItem : MonoBehaviour
{
    public int scoreValue = 5;         // 吃到增加分数
    public float speedMultiplier = 1.5f; // 移动速度倍率
    public float boostDuration = 5f;     // 持续时间

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // 加分
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.AddScore(scoreValue);

        // 给玩家加速度
        PlayerRunner player = other.GetComponent<PlayerRunner>();
        if (player != null)
            player.ActivateSpeedBoost(speedMultiplier, boostDuration);

        Destroy(gameObject);
    }
}
