using UnityEngine;
using UnityEngine.SceneManagement;

public class ObstacleBehaviour : MonoBehaviour
{
    public bool isLowObstacle = false; // �Ƿ��ǵ��ϰ������п��Զ����
    public GameObject explosion;
    public float waitTime = 2.0f;

    private void OnCollisionEnter(Collision collision)
    {
        PlayerRunner player = collision.gameObject.GetComponent<PlayerRunner>();

        if (player != null)
        {
            // ����ǵ��ϰ�����������ڻ��� => ������ײ
            if (isLowObstacle && player.IsSliding())
            {
                Debug.Log("��һ���ͨ�����ϰ�");
                return;
            }

            // ��ͨ�ϰ���û���� => ���˲�������ը
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
