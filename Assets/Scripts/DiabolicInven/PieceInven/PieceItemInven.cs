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
    }

    public void SlotBlockImage(int index)
    {
        slotParent.GetChild(index).GetComponent<PieceItemInvenSlot>().OffBlockImage();
    }
}
