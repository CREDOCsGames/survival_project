using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeachItem : MonoBehaviour, IMouseInteraction
{
    [SerializeField] DiabolicItemInfo[] beachPieceList;

    bool canInteract = false;

    List<DiabolicItemInfo> itemList = new List<DiabolicItemInfo>();

    GameManager gameManager;
    GamesceneManager gamesceneManager;
    Character character;
    ItemManager itemManager;

    private void Start()
    {
        itemManager = ItemManager.Instance;
        character = Character.Instance;
        gamesceneManager = GamesceneManager.Instance;
        gameManager = GameManager.Instance;

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
        yield return CoroutineCaching.WaitForSeconds(waitTime);

        if (gamesceneManager.isNight)
            yield break;

        gameManager.woodCount++;
        character.getItemUI.GetComponent<GetItemUI>().SetGetItemImage(GetComponent<SpriteRenderer>().sprite);
        character.getItemUI.gameObject.SetActive(true);

        //int rand = Random.Range(0, 100);

        if (Random.Range(0, 100) >= 96)
        {
            GetRandomPiece();
        }

        anim.SetBool("isLogging", false);
        character.isCanControll = true;
        character.ChangeAnimationController(0);

        Destroy(gameObject);
    }

    public void InteractionLeftButtonFuc(GameObject hitObject)
    {
        if (!canInteract)
            return;

        canInteract = false;
        StartCoroutine(character.MoveToInteractableObject(transform.position, gameObject, 2));
    }

    public void InteractionRightButtonFuc(GameObject hitObject)
    {
        
    }

    public bool ReturnCanInteraction()
    {
        return canInteract;
    }
}
