using UnityEngine;
using UnityEngine.UI;

public class WeaponSlot : MonoBehaviour
{
    [SerializeField] GameObject sellUI;
    [SerializeField] Image back;

    [HideInInspector] public int slotNum;

    ItemManager itemManager;

    private void Start()
    {
        itemManager = ItemManager.Instance;
        slotNum = transform.GetSiblingIndex();
    }

    private void Update()
    {
        SlotColor();
    }

    void SlotColor()
    {
        if (itemManager.weaponGrade[slotNum] == Grade.¿œπ›)
            back.color = new Color(0.53f, 0.53f, 0.53f, 0.8235f);

        else if (itemManager.weaponGrade[slotNum] == Grade.»Ò±Õ)
            back.color = new Color(0, 0.77f, 1, 0.8235f);

        else if (itemManager.weaponGrade[slotNum] == Grade.¿¸º≥)
            back.color = new Color(0.5f, 0.2f, 0.4f, 0.8235f);

        else if (itemManager.weaponGrade[slotNum] == Grade.Ω≈»≠)
            back.color = new Color(1, 0.31f, 0.31f, 0.8235f);
    }

    public void ShowClickUI()
    {
        if (ItemManager.Instance.storedWeapon[slotNum] != null)
        {
            sellUI.GetComponent<CardClick>().ShowPos();
            sellUI.GetComponent<CardClick>().Setting(slotNum);
            sellUI.GetComponent<CardClick>().CardImage(slotNum);
            sellUI.gameObject.SetActive(true);
        }
    }
}
