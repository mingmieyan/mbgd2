using UnityEngine;
using System.Collections;

public class ObstacleBehaviour : MonoBehaviour
{
    public bool isLowObstacle = false; // 是否是低障碍（滑行可以躲过）
    public float waitTime = 2.0f;

    private void OnCollisionEnter(Collision collision)
    {
        PlayerRunner player = collision.gameObject.GetComponent<PlayerRunner>();
        if (player == null) return;

        // 低障碍且玩家正在滑行 => 忽略
        if (isLowObstacle && player.IsSliding())
        {
            Debug.Log("玩家滑行通过低障碍");
            return;
        }

        // 低障碍且玩家正在滑行 => 忽略
        if (isLowObstacle && player.IsSliding()) return;

        // 扣血到死亡时，会触发 PlayerRunner.Die() -> GameManager.GameOver()
        player.TakeDamage(player.MaxHealth); // 或者直接死亡
    }

    private IEnumerator ShowYouLostMenu(float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject youLost = GetYouLostMenu();
        if (youLost != null)
        {
            youLost.SetActive(true);   // 显示 "You Lost" Canvas
            Time.timeScale = 0f;       // 暂停游戏
        }
    }

    private GameObject GetYouLostMenu()
    {
        var canvas = GameObject.Find("Canvas")?.transform;
        if (canvas == null) return null;

        // 找到名为 "You Lost" 的 Canvas
        return canvas.Find("You Lost")?.gameObject;
    }
}
