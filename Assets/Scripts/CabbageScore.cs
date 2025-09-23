using UnityEngine;

public class CabbageScore : MonoBehaviour
{
    public int scoreNum = 1;

    //when player touch the thing will add score
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Score"))
        {
            //need to take the function from gameManager
            GameManager gameManager = other.gameObject.GetComponent<GameManager>();
            gameManager.AddScore(scoreNum); //send number to gameManager function

            Destroy(gameObject); //Destroy object
        }
    }
}