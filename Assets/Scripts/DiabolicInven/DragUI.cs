using UnityEngine;
using UnityEngine.UI;

public class DragUI : Singleton<DragUI>
{
    [SerializeField] Image dragItemImage;

    DiabolicItemInfo dragItem;

    public DiabolicItemInfo DragItem => dragItem;

    private void Start()
    {
        dragItemImage.gameObject.SetActive(false);
    }

    public void OnDragUI(DiabolicItemInfo selectedItem)
    {
        dragItem = selectedItem;
        dragItemImage.sprite = selectedItem.ItemSprite;
        transform.localScale = new Vector3(selectedItem.ItemShape.Width, selectedItem.ItemShape.Height, 0);
        GetComponent<RectTransform>().pivot = new Vector2(1 / (selectedItem.ItemShape.Width * 2f), 0.9f);
        dragItemImage.gameObject.SetActive(true);

        Cursor.visible = false;
    }

    public void MoveDragUI()
    {
        GetComponent<RectTransform>().anchoredPosition = Input.mousePosition;
    }

    public void OffDragUI()
    {
        dragItemImage.gameObject.SetActive(false);

        Cursor.visible = true;
    }
}
