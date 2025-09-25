using UnityEngine;

public class HealPickup : MonoBehaviour
{
    public int healAmount = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerRunner player = other.GetComponent<PlayerRunner>();
            if (player != null)
            {
                player.AddHealth(healAmount);
            }

            Destroy(gameObject);
        }
    }
}
