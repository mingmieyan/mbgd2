using UnityEngine;

public class Collectible : MonoBehaviour
{
    public int scoreValue = 10; // 这个物体加多少分

    void OnTriggerEnter(Collider other)
    {
        PlayerRunner player = other.GetComponent<PlayerRunner>();
        if (player != null)
        {
            // 调用 GameManager 来加分
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(scoreValue);
                Debug.Log($"玩家获得 {scoreValue} 分!");
            }

            // 销毁卷心菜
            Destroy(gameObject);
        }
    }
}
