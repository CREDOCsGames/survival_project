using System.Collections;
using UnityEngine;

public class Cage : MonoBehaviour, IMouseInteraction
{
    [SerializeField] GameObject tamingPet;

    Character character;
    GameSceneUI gameSceneUI;
    GamesceneManager gameSceneManager;
    GameManager gameManager;

    bool isCanInteraction = false;

    private void Start()
    {
        character = Character.Instance;
        gameSceneUI = GameSceneUI.Instance;
        gameSceneManager = GamesceneManager.Instance;
        gameManager = GameManager.Instance;

        gameSceneUI.tamingGame.SetActive(false);
        tamingPet.SetActive(false);
    }

    private void Update()
    {
        if (gameSceneManager.isNight && isCanInteraction && character.IsTamingPet)
            isCanInteraction = false;
    }

    public void CanInteraction(bool _canInteraction)
    {
        if (character.IsTamingPet)
            return;

        isCanInteraction = _canInteraction;
    }

    public IEnumerator EndInteraction(Animator anim, float waitTime)
    {
        yield return CoroutineCaching.WaitForSeconds(waitTime);

        Cursor.visible = true;
        gameSceneUI.tamingGame.SetActive(false);
        character.isCanControll = true;
    }

    public void InteractionLeftButtonFuc(GameObject hitObject)
    {
        if (character.IsTamingPet || gameManager.round % 5 != 1)
            return;

        if (isCanInteraction && (gameManager.fishHighGradeCount + gameManager.fishLowGradeCount) > 0)
        {
            character.isCanControll = false;
            isCanInteraction = false;
            tamingPet.SetActive(true);

            if (gameManager.fishLowGradeCount > 0)
                gameManager.fishLowGradeCount--;

            else
                gameManager.fishHighGradeCount--;

            StartCoroutine(StartTamingGame(hitObject));
        }
    }

    public void InteractionRightButtonFuc(GameObject hitObject)
    {

    }

    public bool ReturnCanInteraction()
    {
        return isCanInteraction;
    }

    IEnumerator StartTamingGame(GameObject hitObject)
    {
        yield return CoroutineCaching.WaitWhile(() => tamingPet.transform.position != transform.position);

        gameSceneUI.tamingGame.GetComponent<TamingViewUI>().catchPet = hitObject;
        tamingPet.SetActive(false);

        yield return CoroutineCaching.WaitForSeconds(0.5f);

        if (gameSceneManager.isNight)
            yield break;

        gameSceneUI.tamingGame.SetActive(true);
    }
}