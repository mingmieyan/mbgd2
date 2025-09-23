using UnityEngine;
using System.Collections; 

public class JumpBoostItem : MonoBehaviour
{
    public int scoreValue = 10;      // 加分数
    public float boostMultiplier = 1.5f; // 跳跃力倍数
    public float boostDuration = 5f; // 持续时间

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // 加分
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.AddScore(scoreValue);

        // 给玩家加跳跃
        PlayerRunner player = other.GetComponent<PlayerRunner>();
        if (player != null)
            player.ActivateJumpBoost(boostMultiplier, boostDuration);

        Destroy(gameObject);
    }
    

    private IEnumerator BoostJump(PlayerRunner player)
    {
        if (player.isJumpBoosted)
            yield break;

        player.isJumpBoosted = true;
        player.originalJumpForce = player.jumpForce;   // 记录原始跳跃力
        player.jumpForce *= boostMultiplier;

        yield return new WaitForSeconds(boostDuration);

        player.jumpForce = player.originalJumpForce;   // 恢复原始跳跃力
        player.isJumpBoosted = false;
    }
}