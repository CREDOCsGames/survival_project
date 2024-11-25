using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDictionay : MonoBehaviour
{
    [SerializeField] GameObject InventoryView;
    [SerializeField] Transform itemSlotParent;
    [SerializeField] GameObject itemSlotPrefab;
    [SerializeField] GameObject itemMatSlotPrefab;

    private void Start()
    {
        var items = GameManager.itemDatas;

        for (int i = 0; i < items.Count; ++i)
        {
            GameObject itemSlot = Instantiate(itemSlotPrefab, itemSlotParent);

            if (items[i].PieceId == -1)
                itemSlot.transform.Find("ItemImage").GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(items[i].ImageId.ToString());
            else
                itemSlot.transform.Find("ItemImage").GetChild(0).GetComponent<Image>().sprite = ItemManager.Instance.GetPieceInfo(items[i].PieceId).ItemSprite;

            Transform matSlotParent = itemSlot.transform.Find("MatSlots");

            foreach(var mat in items[i].NeedMaterials)
            {
                GameObject matSlot = Instantiate(itemMatSlotPrefab, matSlotParent);
                matSlot.transform.Find("MatImage").GetComponent<Image>().sprite = Resources.Load<Sprite>($"Mat{(int)mat.Key}");
                matSlot.transform.Find("MatCount").GetComponent<Text>().text = $"x{mat.Value}";
            }
        }

        InventoryView.SetActive(false);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
            InventoryView.SetActive(!InventoryView.activeSelf);
    }
}
