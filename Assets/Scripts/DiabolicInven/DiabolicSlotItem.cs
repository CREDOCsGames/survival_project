using UnityEngine;
using UnityEngine.UI;

public class DiabolicSlotItem : MonoBehaviour
{
    [SerializeField] Image itemImage;

    DragUI dragUI;

    private void Start()
    {
        dragUI = DragUI.Instance;
    }

    public void ItemSetOnInventory(Vector3 instantPos, DiabolicItemInfo item)
    {
        SetImage(item);
        GetComponent<RectTransform>().anchoredPosition = instantPos;
    }

    void SetImage(DiabolicItemInfo item)
    {
        if(dragUI == null)
            dragUI = DragUI.Instance;

        GetComponent<RectTransform>().localScale = dragUI.transform.localScale;
        GetComponent<RectTransform>().pivot = dragUI.GetComponent<RectTransform>().pivot;
        itemImage.sprite = item.ItemSprite;
    }
}
