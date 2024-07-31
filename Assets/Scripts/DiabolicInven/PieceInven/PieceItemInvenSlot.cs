using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PieceItemInvenSlot : MonoBehaviour
{
    [SerializeField] Image itemImage;
    
    Text itemDiscriptionText;
    PieceItemInven pieceInven;
    DragUI dragUI;

    int indexNum;

    bool isDragging = false;

    private void Start()
    {
        pieceInven = PieceItemInven.Instance;
        dragUI = DragUI.Instance;

        indexNum = transform.GetSiblingIndex();
        itemDiscriptionText = pieceInven.DiscriptionPanel.GetComponent<Text>();

        UpdateSlotData();
    }

    public void UpdateSlotData()
    {
        if (pieceInven.items[indexNum] == null)
        {
            itemImage.gameObject.SetActive(false);
        }

        else
        {
            itemImage.sprite = pieceInven.items[indexNum].ItemSprite;
            itemImage.gameObject.SetActive(true);
        }
    }

    public void UpdateDiscription()
    {
        itemDiscriptionText.text = pieceInven.items[indexNum] == null ? "" : pieceInven.items[indexNum].Discription;
    }

    public void DragBeginItem()
    {
        if (pieceInven.items[indexNum] == null || isDragging)
            return;

        dragUI.OnDragUI(pieceInven.items[indexNum]);
        isDragging = true;
    }

    public void DraggingItem()
    {
        if (!isDragging)
            return;

        dragUI.MoveDragUI();
    }

    public void DragEndItem()
    {
        if (!isDragging)
            return;

        dragUI.OffDragUI();
        isDragging = false;
    }
}
