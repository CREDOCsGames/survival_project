using System.Collections;
using UnityEngine;

public class Campfire : MonoBehaviour, IMouseInteraction
{
    [SerializeField] GameObject interactionUI;

    GameManager gameManager;

    bool canInteraction = false;

    private void Start()
    {
        gameManager = GameManager.Instance;

        interactionUI.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Character"))
        {
            interactionUI.SetActive(true);
            canInteraction = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Character"))
        {
            interactionUI.SetActive(false);
            canInteraction = false;
        }
    }

    public void InteractionLeftButtonFuc(GameObject hitObject)
    {
        if (!canInteraction || gameManager.woodCount < 10)
            return;

        gameManager.woodCount -= 10;
    }

    public void InteractionRightButtonFuc(GameObject hitObject)
    {
        if (!canInteraction || (gameManager.fish1 <= 0 && gameManager.fish2 <= 0))
            return;

        if (gameManager.fish2 > 0)
            gameManager.fish2--;

        else
            gameManager.fish1--;
    }

    public void CanInteraction(bool canInteraction)
    {
        
    }

    public IEnumerator EndInteraction(Animator anim, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
    }
}
