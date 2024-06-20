using UnityEngine;
using UnityEngine.UI;

public class EndWeaponSlot : MonoBehaviour
{
    [SerializeField] Image back;
    [SerializeField] Image itemImage;

    int slotNum;

    ItemManager itemManager;
    EndUI endUI;

    void Start()
    {
        itemManager = ItemManager.Instance;
        endUI = EndUI.Instance;
        slotNum = transform.parent.GetSiblingIndex();

        itemImage.sprite = itemManager.storedWeapon[endUI.weaponCount[slotNum]].ItemSprite;
        SlotColor();
    }

    void SlotColor()
    {
        if (itemManager.weaponGrade[endUI.weaponCount[slotNum]] == Grade.¿œπ›)
            back.color = new Color(0.53f, 0.53f, 0.53f, 0.8235f);

        else if (itemManager.weaponGrade[endUI.weaponCount[slotNum]] == Grade.»Ò±Õ)
            back.color = new Color(0, 0.77f, 1, 0.8235f);

        else if (itemManager.weaponGrade[endUI.weaponCount[slotNum]] == Grade.¿¸º≥)
            back.color = new Color(0.5f, 0.2f, 0.4f, 0.8235f);

        else if (itemManager.weaponGrade[endUI.weaponCount[slotNum]] == Grade.Ω≈»≠)
            back.color = new Color(1, 0.31f, 0.31f, 0.8235f);
    }
}
