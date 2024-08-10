using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : Singleton<ItemManager>
{
    [SerializeField] Transform slotParent;
    public DiabolicItemInfo[] allPieceItemsList;

    [HideInInspector] public DiabolicItemInfo[] items;
    [HideInInspector] public int[] itemQuantity;
    [HideInInspector] public Dictionary<DiabolicItemInfo, int> itemDict = new Dictionary<DiabolicItemInfo, int>();

    protected override void Awake()
    {
        base.Awake();

        items = new DiabolicItemInfo[slotParent.childCount];
        itemQuantity = new int[slotParent.childCount];

        for (int i = 0; i < allPieceItemsList.Length; i++)
        {
            itemDict.Add(allPieceItemsList[i], 0);
        }
    }

    public void AddItem(DiabolicItemInfo selectedItem)
    {
        int emptyIndex = -1;

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null && emptyIndex == -1)
                emptyIndex = i;

            if (items[i] == selectedItem)
            {
                itemQuantity[i]++;
                return;
            }
        }

        items[emptyIndex] = selectedItem;
        itemQuantity[emptyIndex]++;

        itemDict[items[emptyIndex]] = itemQuantity[emptyIndex];
    }
}
