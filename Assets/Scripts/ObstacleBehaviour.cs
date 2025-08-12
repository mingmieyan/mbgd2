using UnityEngine;
using UnityEngine.SceneManagement;

public class ObstacleBehaviour : MonoBehaviour
{
    public bool isLowObstacle = false; // 是否是低障碍（滑行可以躲过）
    public GameObject explosion;
    public float waitTime = 2.0f;

    private void OnCollisionEnter(Collision collision)
    {
        PlayerRunner player = collision.gameObject.GetComponent<PlayerRunner>();

        if (player != null)
        {
            // 如果是低障碍并且玩家正在滑行 => 忽略碰撞
            if (isLowObstacle && player.IsSliding())
            {
                Debug.Log("玩家滑行通过低障碍");
                return;
            }

            // 普通障碍或没滑行 => 受伤并触发爆炸
            if (explosion != null)
            {
                var particles = Instantiate(explosion, transform.position, Quaternion.identity);
                Destroy(particles, 1.0f);
            }

            player.SendMessage("TakeDamage", 1, SendMessageOptions.DontRequireReceiver);
            Invoke("ResetGame", waitTime);
        }
    }

    private void ResetGame()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);
    }
}
