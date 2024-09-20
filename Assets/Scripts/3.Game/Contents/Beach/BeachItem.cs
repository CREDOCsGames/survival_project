using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeachItem : MonoBehaviour, IMouseInteraction
{
    [SerializeField] DiabolicItemInfo[] beachPieceList;
    [SerializeField] Transform[] gatherPoints;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] AudioClip getWoodSound;

    bool canInteract = false;

    List<DiabolicItemInfo> itemList = new List<DiabolicItemInfo>();

    GameManager gameManager;
    GamesceneManager gamesceneManager;
    Character character;
    ItemManager itemManager;
    SoundManager soundManager;

    Color outlineColor;

    EffectSound currentSfx;

    private void Start()
    {
        itemManager = ItemManager.Instance;
        character = Character.Instance;
        gamesceneManager = GamesceneManager.Instance;
        gameManager = GameManager.Instance;
        soundManager = SoundManager.Instance;

        outlineColor = spriteRenderer.material.GetColor("_SolidOutline");

        canInteract = false;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    void GetRandomPiece()
    {
        itemList.Clear();

        for (int i = 0; i < beachPieceList.Length; ++i)
        {
            if (!itemManager.getItems.ContainsKey(beachPieceList[i]))
            {
                itemList.Add(beachPieceList[i]);
            }

            else
            {
                if (itemManager.getItems[beachPieceList[i]] < beachPieceList[i].MaxCount)
                    itemList.Add(beachPieceList[i]);
            }
        }

        if (itemList.Count <= 0)
        {
            return;
        }

        int totalWeightValue = 0;

        for (int i = 0; i < itemList.Count; ++i)
        {
            totalWeightValue += itemList[i].WeightValue;
        }

        int rand = Random.Range(0, totalWeightValue);

        float total = 0;

        for (int i = 0; i < itemList.Count; i++)
        {
            total += itemList[i].WeightValue;

            if (rand < total)
            {
                rand = i;
                break;
            }
        }

        GameSceneUI.Instance.ShowPieceCard(itemList[rand]);
        itemManager.AddItem(itemList[rand]);
    }

    public void CanInteraction(bool _canInteraction)
    {
        canInteract = _canInteraction;
    }

    public IEnumerator EndInteraction(Animator anim, float waitTime)
    {
        currentSfx = soundManager.PlaySFXAndReturn(getWoodSound, true);

        yield return CoroutineCaching.WaitForSeconds(waitTime);

        if(currentSfx != null)
        {
            soundManager.StopLoopSFX(currentSfx);
        }

        if (gamesceneManager.isNight)
        {
            character.canFlip = true;
            yield break;
        }

        gameManager.woodCount++;
        character.getItemUI.GetComponent<GetItemUI>().SetGetItemImage(GetComponent<SpriteRenderer>().sprite, 1);
        character.getItemUI.gameObject.SetActive(true);

#if UNITY_EDITOR
        if (Random.Range(0, 100) >= 0)
        {
            GetRandomPiece();
        }

#else
        if (Random.Range(0, 100) >= 96)
        {
            GetRandomPiece();
        }
#endif

        anim.SetBool("isLogging", false);
        character.canFlip = true;
        character.isCanControll = true;
        character.ChangeAnimationController(0);

        Destroy(gameObject);

        GameSceneUI.Instance.ActiveTutoPanel(TutoType.MoveTuto);
    }

    public void InteractionLeftButtonFuc(GameObject hitObject)
    {
        if (!canInteract)
            return;

        int num = (character.transform.position - transform.position).x > 0 ? 0 : 1;

        canInteract = false;
        StartCoroutine(character.MoveToInteractableObject(gatherPoints[num].position, gameObject, 1.25f, 5, -1, num));
    }

    public void InteractionRightButtonFuc(GameObject hitObject)
    {
        
    }

    public bool ReturnCanInteraction()
    {
        return canInteract;
    }

    private void OnMouseOver()
    {
        if (canInteract)
        {
            if (outlineColor.a == 1)
                return;

            outlineColor.a = 1;

            spriteRenderer.material.SetColor("_SolidOutline", outlineColor);
        }

        else
        {
            if (outlineColor.a == 0)
                return;

            outlineColor.a = 0;

            spriteRenderer.material.SetColor("_SolidOutline", outlineColor);
        }
    }

    private void OnMouseExit()
    {
        if (outlineColor.a == 0)
            return;

        outlineColor.a = 0;

        spriteRenderer.material.SetColor("_SolidOutline", outlineColor);
    }
}
