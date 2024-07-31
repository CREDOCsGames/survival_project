using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DiabolicInvenSlot : MonoBehaviour
{
    [SerializeField] Image canSetImage;

    DiabolicInven inven;

    bool isEmpty = true;

    bool inPointer = false;

    public bool IsEmpty => isEmpty;

    private void Start()
    {
        inven = DiabolicInven.Instance;
        canSetImage.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!inPointer)
            return;

        if (Input.GetMouseButtonUp(0))
        {
            DropDragItem();
        }
    }

    public void PointerEnter()
    {
        inPointer = true;
        ChangeCanSetImageColor();
    }

    public void PointerExit()
    {
        inPointer = false;
        OffCanSetImage();
    }

    public void ChangeSlotEmpty(bool _isEmpty)
    {
        isEmpty = _isEmpty;
    }

    public void ChangeCanSetImageColor()
    {
        canSetImage.color = isEmpty ? Color.blue : Color.red;
        canSetImage.gameObject.SetActive(true);
    }

    public void OffCanSetImage()
    {
        canSetImage.gameObject.SetActive(false);
    }

    public void DropDragItem()
    {
        if (!isEmpty)
            return;

        inven.InstantItemImage(GetComponent<RectTransform>().localPosition);
        isEmpty = false;
    }
}
