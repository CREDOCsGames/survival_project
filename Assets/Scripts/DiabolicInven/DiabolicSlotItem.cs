using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DiabolicSlotItem : MonoBehaviour
{
    [SerializeField] Image itemImage;

    DragUI dragUI;
    GameManager gameManager;
    DiabolicInven inven;
    ItemManager itemManager;

    DiabolicItemInfo itemData;
    Dictionary<Status, int> itemStatus;
    Dictionary<SpecialStatus, bool> itemSpecialStatus;

    List<int> indexes;
    int firstIndex;

    int pieceSlotIndex;

    private void Start()
    {
        dragUI = DragUI.Instance;
        gameManager = GameManager.Instance;
        inven = DiabolicInven.Instance;
        itemManager = ItemManager.Instance;
    }

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            SubtractItem();
    }

    public void ItemSetOnInventory(Vector3 instantPos, DiabolicItemInfo item, List<int> _indexes, int _firstIndex, int _pieceSlotIndex)
    {
        itemData = item;
        SetImage(item);
        GetComponent<RectTransform>().anchoredPosition = instantPos;
        indexes = _indexes;
        firstIndex = _firstIndex;
        pieceSlotIndex = _pieceSlotIndex;
    }

    void SetImage(DiabolicItemInfo item)
    {
        if(dragUI == null)
            dragUI = DragUI.Instance;

        GetComponent<RectTransform>().localScale = dragUI.transform.localScale;
        GetComponent<RectTransform>().pivot = dragUI.GetComponent<RectTransform>().pivot;
        itemImage.sprite = item.ItemSprite;
    }

    void SubtractItem()
    {
        if (Input.GetMouseButtonUp(1))
        {
            bool canSub = false;

            foreach (var i in indexes)
            {
                if(i == inven.currentIndex)
                {
                    canSub = true; 
                    break;
                }
            }

            if (canSub)
            {
                itemStatus = itemData.Stat();
                itemSpecialStatus = itemData.SpecialStat();

                for (int i = 0; i < gameManager.status.Count; i++)
                {
                    if (itemStatus[(Status)i] == 0)
                        continue;

                    gameManager.status[(Status)i] -= itemStatus[(Status)i] * itemManager.itemQuantity[pieceSlotIndex];
                }

                for (int i = 0; i < gameManager.specialStatus.Count; ++i)
                {
                    if (itemSpecialStatus[(SpecialStatus)i])
                        gameManager.specialStatus[(SpecialStatus)i] = false;
                }

                int row = firstIndex / 4;
                int column = firstIndex % 4;

                inven.SetSlotIsEmpty(itemData.ItemShape.Height, itemData.ItemShape.Width, itemData.ItemShape.Shape, row, column, true);

                Destroy(gameObject);
            }

            Character.Instance.UpdateStat();
            PieceItemInven.Instance.SlotBlockImage(pieceSlotIndex);
        }
    }
}
