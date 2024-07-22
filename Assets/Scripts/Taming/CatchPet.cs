using System.Collections;
using UnityEngine;

public class CatchPet : MonoBehaviour, IMouseInteraction
{
    Character character;
    GameSceneUI gameSceneUI;
    bool isCanInteraction = true;

    private void Start()
    {
        character = Character.Instance;
        gameSceneUI = GameSceneUI.Instance;
        gameSceneUI.tamingGame.SetActive(false);
    }

    public void CanInteraction(bool _canInteraction)
    {
        
    }

    public IEnumerator EndInteraction(Animator anim, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        Cursor.visible = true;
        character.isCanControll = true;
        gameSceneUI.tamingGame.SetActive(false);
        gameObject.SetActive(false);
        isCanInteraction = true;
    }

    public void InteractionLeftButtonFuc(GameObject hitObject)
    {
        if (isCanInteraction)
        {
            character.isCanControll = false;
            isCanInteraction = false;
            gameSceneUI.tamingGame.GetComponent<TamingViewUI>().catchPet = hitObject;
            gameSceneUI.tamingGame.SetActive(true);
        }
    }

    public void InteractionRightButtonFuc(GameObject hitObject)
    {

    }

    public bool ReturnCanInteraction()
    {
        return isCanInteraction;
    }
}
