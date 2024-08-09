using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiabolicInven : Singleton<DiabolicInven>
{
    [SerializeField] Transform slotParent;
    [SerializeField] Transform itemImageParent;
    [SerializeField] GameObject itemImage;

    DiabolicInvenSlot[] slots;

    public int currentIndex;

    DragUI dragUI;
    GameManager gameManager;

    int width;
    int height;
    bool[] itemShape;
    

    int column;
    int row;

    bool canSetImage;

    Dictionary<Status,int> itemStatus = new Dictionary<Status,int>();

    private void Start()
    {
        gameManager = GameManager.Instance;
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

        Instantiate(itemImage, itemImageParent).GetComponent<DiabolicSlotItem>().ItemSetOnInventory(slots[currentIndex].GetComponent<RectTransform>().localPosition, item, TransferIndexesNum());

        AddStatus();

        SetSlotIsEmpty(false);
    }

    void AddStatus()
    {
        itemStatus = dragUI.DragItem.Stat();

        for (int i = 0; i < gameManager.status.Count; i++)
        {
            if (itemStatus[(Status)i] == 0)
                continue;

            gameManager.status[(Status)i] += itemStatus[(Status)i];

            Debug.Log((Status)i + " " + gameManager.status[(Status)i]);
        }
    }

    public void ChagneCanSetImage(bool _canSetImage)
    {
        canSetImage = _canSetImage;
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

    public void ChangeSlotsColor()
    {
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

    public void SetSlotIsEmpty(bool slotEmpty)
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

    List<int> TransferIndexesNum()
    {
        List<int> indexes = new List<int>();

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (itemShape[i * width + j])
                {
                    indexes.Add((i + row) * 4 + j + column);
                }
            }
        }

        return indexes;
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
