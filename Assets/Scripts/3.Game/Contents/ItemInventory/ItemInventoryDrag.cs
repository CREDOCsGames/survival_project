using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class ItemInventoryDrag : MonoBehaviour
{
    public RectTransform RectTransform => rectTransform;

    RectTransform rectTransform;

    public ItemInfo itemInfo;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        gameObject.SetActive(false);
    }
}
