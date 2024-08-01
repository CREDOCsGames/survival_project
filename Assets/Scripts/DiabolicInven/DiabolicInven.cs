using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiabolicInven : Singleton<DiabolicInven>
{
    [SerializeField] Transform slotParent;
    [SerializeField] Transform itemImageParent;
    [SerializeField] GameObject itemImage;

    DiabolicInvenSlot[] slots;

    int currentIndex;

    DragUI dragUI;

    int width;
    int height;
    bool[] itemShape;

    int column;
    int row;

    bool canSetImage;

    public DiabolicItemInfo draggingItem;

    private void Start()
    {
        dragUI = DragUI.Instance;

        slots = new DiabolicInvenSlot[slotParent.childCount];

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = slotParent.GetChild(i).GetComponent<DiabolicInvenSlot>();
        }
    }

    public void InstantItemImage(DiabolicItemInfo item)
    {
        OffSlotSetImage();

        if (!canSetImage)
        {
            return;
        }

        Instantiate(itemImage, itemImageParent).GetComponent<DiabolicSlotItem>().ItemSetOnInventory(slots[currentIndex].GetComponent<RectTransform>().localPosition, item);

        SetSlotIsEmpty(false);
    }

    void CheckCanSetImage()
    {
        width = dragUI.DragItem.ItemShape.Width;
        height = dragUI.DragItem.ItemShape.Height;
        itemShape = dragUI.DragItem.ItemShape.Shape;

        column = currentIndex % 4;
        row = currentIndex / 4;

        canSetImage = (column + width) <= 4 && (row + height) <= 4;

        if (!canSetImage)
            return;

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j <  width; j++)
            {
                if (itemShape[i * width + j] && !slots[(i + row) * 4 + j + column].GetComponent<DiabolicInvenSlot>().IsEmpty)
                {

                    canSetImage = false;
                    return;
                }
            }
        }
    }

    public void ChangeSlotsColor(int index)
    {
        currentIndex = index;

        CheckCanSetImage();

        for (int i = row; i < Mathf.Clamp(row + height, row, 4); i++)
        {
            for (int j = column; j < Mathf.Clamp(column + width, column, 4); j++)
            {
                if (itemShape[(i - row) * width + (j - column)])
                {
                    slots[i * 4 + j].GetComponent<DiabolicInvenSlot>().ChangeCanSetImageColor(canSetImage);
                }
            }
        }
    }

    void SetSlotIsEmpty(bool slotEmpty)
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (itemShape[i * width + j])
                {
                    slots[(i + row) * 4 + j + column].GetComponent<DiabolicInvenSlot>().ChagneEmptyState(slotEmpty);
                }
            }
        }
    }

    public void OffSlotSetImage()
    {
        for (int i = row; i < Mathf.Clamp(row + height, row, 4); i++)
        {
            for (int j = column; j < Mathf.Clamp(column + width, column, 4); j++)
            {
                if (itemShape[(i - row) * width + (j - column)])
                {
                    slots[i * 4 + j].GetComponent<DiabolicInvenSlot>().OffCanSetImage();
                }
            }
        }
    }
}
