
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuickSlot : MonoBehaviour,
    IDropHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] Image itemImage;
    [SerializeField] ItemInventoryDrag dragSlot;
    [SerializeField] private Image slotBackground;
    private static readonly List<QuickSlot> allSlots = new();
    ItemInfo itemInfo;

    // 다른 슬롯에서 현재 아이템을 조회하기 위한 프로퍼티
    public ItemInfo ItemInfo => itemInfo;

    private void Awake()
    {
        allSlots.Add(this);
        // 게임 시작 시 빈 슬롯 상태로 초기화
        SetItemDirect(null);
    }

    private void OnDestroy()
    {
        allSlots.Remove(this);
    }

    // 퀵슬롯에 아이템 설정
    public void SetItem(ItemInfo item)
    {
        // 이미 다른 퀵슬롯에 등록된 동일 아이템 제거
        if (item != null)
        {
            foreach (QuickSlot slot in allSlots)
            {
                if (slot != this &&slot.itemInfo?.itemId == item.itemId)
                {
                    slot.SetItemDirect(null);
                }
            }
        }

        SetItemDirect(item);
    }
    // 아이템 정보와 이미지만 직접 변경
    private void SetItemDirect(ItemInfo item)
    {
        itemInfo = item;

        // 아이템 아이콘 변경
        itemImage.sprite = item == null ? null : Resources.Load<Sprite>($"Item/{item.itemId}");

        itemImage.enabled = item != null;

        // 기존 슬롯 배경 유지, 색상만 변경
        slotBackground.color = item == null ? Color.white : new Color(0.85f, 0.7f, 0.5f, 1f);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        // 1. 장착 슬롯 -> 퀵슬롯
        EquipmentSlotUI fromEquipment = eventData.pointerDrag.GetComponent<EquipmentSlotUI>();

        if (fromEquipment != null)
        {
            EquipmentManager manager = fromEquipment.EquipmentManager;

            ItemInfo equippedItem = manager.GetEquippedItem();
            ItemInfo previousItem = itemInfo;

            // 퀵슬롯에 기존 아이템이 있다면 장착 가능 여부 검사 후 교환
            if (previousItem != null)
            {
                if (!manager.EquipItem(previousItem)) return;
            }
            else
            {
                // 빈 퀵슬롯으로 옮긴 경우 장착 해제
                manager.UnequipItem();
            }

            SetItem(equippedItem);
            return;
        }

        // 2. 퀵슬롯 -> 퀵슬롯
        QuickSlot fromQuick = eventData.pointerDrag.GetComponent<QuickSlot>();

        if (fromQuick != null)
        {
            if (fromQuick == this) return;

            ItemInfo draggedItem = fromQuick.ItemInfo;
            ItemInfo previousItem = itemInfo;

            fromQuick.SetItemDirect(previousItem);
            SetItemDirect(draggedItem);

            return;
        }

        // 3. 일반 인벤토리 -> 퀵슬롯
        ItemInvenSlot fromInventory = eventData.pointerDrag.GetComponent<ItemInvenSlot>();

        if (fromInventory != null)
        {
            ItemInfo draggedItem = fromInventory.inventory.DragSlot.GetComponent<ItemInventoryDrag>().itemInfo;

            SetItem(draggedItem);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemInfo == null) return;

        dragSlot.itemInfo = itemInfo;
        dragSlot.GetComponentInChildren<Image>().sprite = itemImage.sprite;
        dragSlot.gameObject.SetActive(true);

        // 원본 아이템은 드롭 성공 전까지 유지
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (itemInfo == null) return;

        RectTransform dragRect = dragSlot.GetComponent<RectTransform>();

        RectTransform parentRect = dragRect.parent as RectTransform;

        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(parentRect, eventData.position, eventData.pressEventCamera, out Vector3 worldPos))
        {
            dragRect.position = worldPos;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragSlot.gameObject.SetActive(false);
    }
    public static void ClearItemFromAll(int itemId)
    {
        foreach (QuickSlot slot in allSlots)
        {
            if (slot.itemInfo?.itemId == itemId)
            {
                slot.SetItemDirect(null);
            }
        }
    }
}
