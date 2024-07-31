using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceItemInven : Singleton<PieceItemInven>
{
    [SerializeField] Transform slotParent;
    [SerializeField] DiabolicItemInfo[] itemms;
    public GameObject DiscriptionPanel;

    public DiabolicItemInfo[] items;
    public int[] itemQuantity;

    protected override void Awake()
    {
        base.Awake();

        items = new DiabolicItemInfo[slotParent.childCount];
        itemQuantity = new int[slotParent.childCount];

        for (int i = 0; i < itemms.Length; i++)
        {
            items[i] = itemms[i];
            itemQuantity[i]++;
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
    }
}
