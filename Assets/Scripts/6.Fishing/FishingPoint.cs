using UnityEngine;

public class FishingPoint : MonoBehaviour
{
    [SerializeField] GameObject fishingGame;

    int round = 0;
    int currentRound = 0;

    GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (round > currentRound)
            return;

        if (other.CompareTag("Character"))
        {
            currentRound = gameManager.round;
            round = currentRound;

            Character.Instance.isCanControll = false;
            fishingGame.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (round != currentRound)
            return;

        if (other.CompareTag("Character"))
        {
            round++;
        }
    }
}
