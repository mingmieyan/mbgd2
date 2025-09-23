using UnityEngine;

public class CabbageBehaviour : MonoBehaviour
{
    public int scoreValue = 10; // 吃到加多少分

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 调用 ScoreManager 来加分
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(scoreValue);
                Debug.Log($"玩家吃到卷心菜，加 {scoreValue} 分!");
            }

            // 销毁卷心菜
            Destroy(gameObject);
        }
    }
}