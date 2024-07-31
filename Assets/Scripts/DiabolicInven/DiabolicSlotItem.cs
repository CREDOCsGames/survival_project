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

    public void ItemSetOnInventory(Vector3 instantPos)
    {
        SetImage();
        GetComponent<RectTransform>().anchoredPosition = instantPos;
    }

    void SetImage()
    {
        if(dragUI == null)
            dragUI = DragUI.Instance;

        GetComponent<RectTransform>().localScale = dragUI.transform.localScale;
        GetComponent<RectTransform>().pivot = dragUI.GetComponent<RectTransform>().pivot;
        itemImage.sprite = dragUI.DragItem.ItemSprite;
    }
}
