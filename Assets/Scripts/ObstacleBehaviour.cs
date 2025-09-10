using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ObstacleBehaviour : MonoBehaviour
{
    public bool isLowObstacle = false; // 是否是低障碍（滑行可以躲过）
    public GameObject explosion;
    public float waitTime = 2.0f;

    private void OnCollisionEnter(Collision collision)
    {
        PlayerRunner player = collision.gameObject.GetComponent<PlayerRunner>();
        {
            // Destroy the player
            Destroy(collision.gameObject);
            // Call the function ResetGame after
            // waitTime has passed
            StartCoroutine("ResetGame", waitTime);
        }
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
            Destroy(this.gameObject);

        }
    }
    [Tooltip("How long to wait before restarting thegame")]

    
    /// <summary>
    /// Will restart the currently loaded level
    /// </summary>
    IEnumerator ResetGame(float waitTime)
    {
        // Get the current level's name
        //string sceneName =
        //SceneManager.GetActiveScene().name;
        // Restarts the current level
        // SceneManager.LoadScene(sceneName);
        yield return new WaitForSeconds(waitTime);
        var go = GetGameOverMenu();
        go.SetActive(true);
    }
    GameObject GetGameOverMenu()
    {
        var canvas = GameObject.Find("Canvas").transform;
        return canvas.Find("Game Over").gameObject;
    }
}
