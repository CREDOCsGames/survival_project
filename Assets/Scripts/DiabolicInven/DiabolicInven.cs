using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiabolicInven : Singleton<DiabolicInven>
{
    [SerializeField] Transform slotParent;
    [SerializeField] Transform itemImageParent;
    [SerializeField] GameObject itemImage;

    DiabolicInvenSlot[] slots;

    private void Start()
    {
        slots = new DiabolicInvenSlot[slotParent.childCount];

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = slotParent.GetChild(i).GetComponent<DiabolicInvenSlot>();
        }
    }

    public void InstantItemImage(Vector3 instanctPos)
    {
        Instantiate(itemImage, itemImageParent).GetComponent<DiabolicSlotItem>().ItemSetOnInventory(instanctPos);
    }
}
