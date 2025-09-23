using UnityEngine;

public class SpoiledFood : MonoBehaviour
{
    public int damage = 1;        // ¿ÛÑªÁ¿
    public int scorePenalty = 10; // ¿Û·ÖÊý

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // ¿ÛÑª
            PlayerRunner player = other.GetComponent<PlayerRunner>();
            if (player != null)
            {
                
                ScoreManager.Instance?.AddScore(-scorePenalty);
            }

            Destroy(gameObject);
        }
    }
}
