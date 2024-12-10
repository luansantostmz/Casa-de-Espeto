using UnityEngine;

public class UIForgeInventoryItem : MonoBehaviour
{
    public CardItem InventoryItem;
    public UIForgeSlot ForgeSlot;

    DragAndDropObject DragNDrop;

    private void Awake()
    {
        InventoryItem = GetComponent<CardItem>();
        DragNDrop = GetComponent<DragAndDropObject>();
        DragNDrop.OnDrop += OnDrop;
    }

    private void OnDestroy()
    {
        DragNDrop.OnDrop -= OnDrop;
    }

    void OnDrop(DropZone dropZone)
    {
        if (dropZone == null)
            return;

        var cardItem = GetComponent<CardItem>();

        if (dropZone.DropZoneOwner.TryGetComponent(out UIForgeSlot slot) && !dropZone.IsBlocked)
        {
            if (cardItem.Item.MeltedItem == null)
                return;

            slot.SetItem(InventoryItem);
            ForgeSlot = slot;
            DragNDrop.useSlotId = true;
            InventoryService.RemoveItem(cardItem);
        }
        else if (dropZone.DropZoneOwner.TryGetComponent(out UIInventory inventory))
        {
            if (!ForgeSlot)
                return;

            //InventoryService.AddItem(cardItem, false);
            ForgeSlot.RemoveItem();
            DragNDrop.useSlotId = false;
        }
    }
}
