using UnityEngine;

public class PassiveSlot : MonoBehaviour
{
    [SerializeField] ShowPassiveSlotCard infoUIPrefab;
    [HideInInspector] public int slotNum;

    RectTransform rectTransform;

    ItemManager itemManager;

    void Start()
    {
        slotNum = transform.GetSiblingIndex();
        rectTransform = GetComponent<RectTransform>();
        itemManager = ItemManager.Instance;
    }

    public void OnInfoUI()
    {
        if (itemManager.storedPassive[slotNum] != null)
        {
            infoUIPrefab.selectedNum = slotNum;
            infoUIPrefab.rect.position = rectTransform.position;
            infoUIPrefab.infoChange = true;
            infoUIPrefab.gameObject.SetActive(true);
        }
    }

    public void OffInfoUI()
    {
        if (itemManager.storedPassive[slotNum] != null)
            infoUIPrefab.gameObject.SetActive(false);
    }
}
