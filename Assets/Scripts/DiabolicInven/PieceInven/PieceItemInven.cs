using System.Collections.Generic;
using UnityEngine;

public class PieceItemInven : Singleton<PieceItemInven>
{
    [SerializeField] Transform slotParent;
    public GameObject DiscriptionPanel;
    
    [HideInInspector] public DiabolicItemInfo[] items;
    [HideInInspector] public int[] itemQuantity;

    ItemManager itemManager;

    private void OnEnable()
    {
        itemManager = ItemManager.Instance;

        items = itemManager.items;
        itemQuantity = itemManager.itemQuantity;

        for(int i = 0; i < items.Length; i++) 
        {
            if (items[i] != null)
                Debug.Log(i + ": " + items[i].ItemName);
        }
    }
}
