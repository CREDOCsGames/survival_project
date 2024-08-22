using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatherFruit : MonoBehaviour, IMouseInteraction
{
    [SerializeField] int defaultGaugeUpValue;
    [SerializeField] DiabolicItemInfo[] bushPieceList;

    bool canGather;

    Character character;
    ItemManager itemManager;
    GameSceneUI gameSceneUI;
    GamesceneManager gamesceneManager;

    List<DiabolicItemInfo> itemList = new List<DiabolicItemInfo>();

    private void Start()
    {
        canGather = false;
        character = Character.Instance;
        itemManager = ItemManager.Instance;
        gameSceneUI = GameSceneUI.Instance;
        gamesceneManager = GamesceneManager.Instance;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    void GetRandomPiece()
    {
        itemList.Clear();

        for (int i = 0; i < bushPieceList.Length; ++i)
        {
            if (!itemManager.getItems.ContainsKey(bushPieceList[i]))
            {
                itemList.Add(bushPieceList[i]);
            }

            else
            {
                if (itemManager.getItems[bushPieceList[i]] < bushPieceList[i].MaxCount)
                    itemList.Add(bushPieceList[i]);
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

        gameSceneUI.ShowPieceCard(itemList[rand]);
        ItemManager.Instance.AddItem(itemList[rand]);
    }

    public void CanInteraction(bool _canInteraction)
    {
        canGather = _canInteraction;
    }

    public void InteractionLeftButtonFuc(GameObject hitObject)
    {
        if (canGather)
        {
            canGather = false;
            StartCoroutine(character.MoveToInteractableObject(transform.position, gameObject, 3));
        }
    }

    public IEnumerator EndInteraction(Animator anim, float waitTime)
    {
        yield return CoroutineCaching.WaitForSeconds(waitTime);

        if (gamesceneManager.isNight)
            yield break;

        int rand = Random.Range(0, 100);

        if (rand >= 96)
            GetRandomPiece();

        RecoveryGaugeUp();
        Destroy(gameObject);

        anim.SetBool("isLogging", false);
        character.isCanControll = true;
        character.ChangeAnimationController(0); 
    }

    void RecoveryGaugeUp()
    {
        int rand = Random.Range(0, 100);
        int fruitNum = (rand >= 0 && rand < 80) ? 0 : 1;
        int ratio = (fruitNum == 0) ? 1 : 2;

        character.getItemUI.GetComponent<GetItemUI>().SetFruitGetImage(fruitNum);
        character.getItemUI.gameObject.SetActive(true);

        character.currentRecoveryGauge += defaultGaugeUpValue * ratio;
    }


    public void InteractionRightButtonFuc(GameObject hitObject)
    {

    }

    public bool ReturnCanInteraction()
    {
        return canGather;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 2);
    }
#endif
}
