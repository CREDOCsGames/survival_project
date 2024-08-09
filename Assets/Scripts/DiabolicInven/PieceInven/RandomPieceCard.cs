using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPieceCard : MonoBehaviour
{
    [SerializeField] DiabolicItemInfo[] items;

    List<DiabolicItemInfo> itemList = new List<DiabolicItemInfo>();

    ItemManager itemManager;

    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        itemManager = ItemManager.Instance;

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        for (int i = 0; i < items.Length; i++)
        {
            if (itemManager.itemQuantity[i] < items[i].MaxCount)
                itemList.Add(items[i]);
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            int num = Random.Range(0,itemList.Count);

            transform.GetChild(i).GetComponent<PieceCard>().GetRandomItem(itemList[num]);
            transform.GetChild(i).gameObject.SetActive(true);

            itemList.RemoveAt(num);
        }
    }
}
