using System.Collections;
using UnityEngine;

public class Cage : MonoBehaviour, IMouseInteraction
{
    [SerializeField] GameObject tamingPet;
    [SerializeField] GameObject needFishImage;

    Character character;
    GameSceneUI gameSceneUI;
    GamesceneManager gameSceneManager;
    GameManager gameManager;
    SoundManager soundManager;

    bool isCanInteraction = false;

    bool isTamingGamePlay = false;

    private void Start()
    {
        character = Character.Instance;
        gameSceneUI = GameSceneUI.Instance;
        gameSceneManager = GamesceneManager.Instance;
        gameManager = GameManager.Instance;
        soundManager = SoundManager.Instance;

        gameSceneUI.tamingGame.SetActive(false);
        tamingPet.SetActive(false);

        GetComponentInChildren<CheckCharacter>().needItemImage = needFishImage;
    }

    private void Update()
    {
        if (gameSceneManager.isNight || isTamingGamePlay || character.IsTamingPet)
            isCanInteraction = false;
    }

    public void CanInteraction(bool _canInteraction)
    {
        if (character.IsTamingPet || isTamingGamePlay)
            return;

        isCanInteraction = _canInteraction;
    }

    public IEnumerator EndInteraction(Animator anim, float waitTime)
    {
        soundManager.StopBGM();

        yield return CoroutineCaching.WaitForSeconds(waitTime);

        soundManager.PlayBGM(1, true);

        Cursor.visible = true;
        gameManager.isCursorVisible = true;
        gameSceneUI.tamingGame.SetActive(false);
        character.isCanControll = true;
    }

    public void InteractionLeftButtonFuc(GameObject hitObject)
    {
        if (character.IsTamingPet)
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
        isTamingGamePlay = true;

        yield return CoroutineCaching.WaitWhile(() => tamingPet.transform.position != transform.position);

        gameSceneUI.tamingGame.GetComponent<TamingViewUI>().catchPet = hitObject;
        tamingPet.SetActive(false);

        yield return CoroutineCaching.WaitForSeconds(0.5f);

        if (gameSceneManager.isNight)
            yield break;

        gameSceneUI.ActiveTutoPanel(TutoType.TamingGameTuto);
        soundManager.PlayBGM(3, true);
        gameSceneUI.tamingGame.SetActive(true);
    }
}
