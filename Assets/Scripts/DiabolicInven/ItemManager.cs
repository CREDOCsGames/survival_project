using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : Singleton<ItemManager>
{
    [SerializeField] Transform slotParent;

    [HideInInspector] public DiabolicItemInfo[] items;
    [HideInInspector] public int[] itemQuantity;

    protected override void Awake()
    {
        base.Awake();

        items = new DiabolicItemInfo[slotParent.childCount];
        itemQuantity = new int[slotParent.childCount];
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
    }
}
