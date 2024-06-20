using UnityEngine;
using UnityEngine.UI;

public class EndPassSlot : MonoBehaviour
{
    [SerializeField] Image back;
    [SerializeField] Image itemImage;
    [SerializeField] Text itemCount;

    int slotNum;

    ItemManager itemManager;

    void Start()
    {
        itemManager = ItemManager.Instance;
        slotNum = transform.parent.GetSiblingIndex();

        itemImage.sprite = itemManager.storedPassive[slotNum].ItemSprite;
        itemCount.text = itemManager.storedPassiveCount[slotNum].ToString();
        SlotColor();
    }

    void SlotColor()
    {
        if (itemManager.storedPassive[slotNum].ItemGrade == Grade.¿œπ›)
            back.color = new Color(0.53f, 0.53f, 0.53f, 0.8235f);

        else if (itemManager.storedPassive[slotNum].ItemGrade == Grade.»Ò±Õ)
            back.color = new Color(0, 0.77f, 1, 0.8235f);

        else if (itemManager.storedPassive[slotNum].ItemGrade == Grade.¿¸º≥)
            back.color = new Color(0.5f, 0.2f, 0.4f, 0.8235f);

        else if (itemManager.storedPassive[slotNum].ItemGrade == Grade.Ω≈»≠)
            back.color = new Color(1, 0.31f, 0.31f, 0.8235f);
    }
}
