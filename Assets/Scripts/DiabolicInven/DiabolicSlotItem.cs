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

    DiabolicItemInfo itemData;
    Dictionary<Status, int> itemStatus;

    List<int> indexes;

    private void Start()
    {
        dragUI = DragUI.Instance;
        gameManager = GameManager.Instance;
        inven = DiabolicInven.Instance;
    }

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            SubtractItem();
    }

    public void ItemSetOnInventory(Vector3 instantPos, DiabolicItemInfo item, List<int> _indexes)
    {
        itemData = item;
        SetImage(item);
        GetComponent<RectTransform>().anchoredPosition = instantPos;
        indexes = _indexes;
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
                    Debug.Log(i + " " + inven.currentIndex);
                    canSub = true; 
                    break;
                }
            }

            if (canSub)
            {
                itemStatus = itemData.Stat();

                for (int i = 0; i < gameManager.status.Count; i++)
                {
                    if (itemStatus[(Status)i] == 0)
                        continue;

                    gameManager.status[(Status)i] -= itemStatus[(Status)i];
                    Debug.Log((Status)i + " " + gameManager.status[(Status)i]);
                }

                inven.SetSlotIsEmpty(true);

                Destroy(gameObject);
            }
        }
    }
}
