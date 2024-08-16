using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PieceItemInvenSlot : MonoBehaviour
{
    [SerializeField] Image itemImage;
    [SerializeField] Image GradeColorImage;
    [SerializeField] Image blockImage;
    
    Text itemDiscriptionText;
    PieceItemInven pieceInven;
    DragUI dragUI;

    int indexNum;

    bool isDragging = false;

    Dictionary<Status,int> status = new Dictionary<Status,int>();

    private void Awake()
    {
        pieceInven = PieceItemInven.Instance;
        dragUI = DragUI.Instance;

        indexNum = transform.GetSiblingIndex();
        itemDiscriptionText = pieceInven.DiscriptionPanel.GetComponent<Text>();
        itemDiscriptionText.text = "";
        blockImage.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        UpdateSlotData();
    }

    private void Update()
    {
        ChangeSlotColor();
    }

    void ChangeSlotColor()
    {
        switch(pieceInven.itemQuantity[indexNum])
        {
            case 0:
                GradeColorImage.gameObject.SetActive(false);
                break;

            case 1:
                GradeColorImage.gameObject.SetActive(true);
                GradeColorImage.color = Color.gray;
                break;

            case 2:
                GradeColorImage.gameObject.SetActive(true);
                GradeColorImage.color = Color.yellow;
                break;

            case 3:
                GradeColorImage.gameObject.SetActive(true);
                GradeColorImage.color = Color.green;
                break;
        }
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
        itemDiscriptionText.text = "";

        if (pieceInven.items[indexNum] == null)
        {
            return;
        }

        status = pieceInven.items[indexNum].Stat();

        for (int i = 0; i < status.Count; i++)
        {
            if (status[(Status)i] == 0)
                continue;

            itemDiscriptionText.text += GameManager.statNames[i] + ": " + status[(Status)i] + "\n";
        }

        itemDiscriptionText.text += "\n" + pieceInven.items[indexNum].Description;
    }

    public void DragBeginItem()
    {
        if (pieceInven.items[indexNum] == null || isDragging || blockImage.gameObject.activeSelf)
            return;

        if(dragUI == null)
            dragUI = DragUI.Instance;

        dragUI.SettingDragUI(pieceInven.items[indexNum]);
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

        DiabolicInven.Instance.InstantItemImage(dragUI.DragItem, blockImage.gameObject, indexNum);

        dragUI.OffDragUI();
        isDragging = false;
    }

    public void OffBlockImage()
    {
        blockImage.gameObject.SetActive(false);
    }
}
