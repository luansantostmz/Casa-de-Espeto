using UnityEngine;

public class UIForgeInventoryItem : MonoBehaviour
{
    public UICardItem InventoryItem;
    public UIForgeSlot ForgeSlot;

    DragAndDropObject DragNDrop;

    private void Awake()
    {
        InventoryItem = GetComponent<UICardItem>();
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

        var item = GetComponent<UICardItem>().Item;

        if (dropZone.DropZoneOwner.TryGetComponent(out UIForgeSlot slot) && !dropZone.IsBlocked)
        {
            if (item.Settings.MeltedItem == null)
                return;

            slot.SetItem(InventoryItem);
            ForgeSlot = slot;
            DragNDrop.useSlotId = true;
            InventoryService.RemoveItem(item);
        }
        else if (dropZone.DropZoneOwner.TryGetComponent(out UIInventory inventory))
        {
            if (!ForgeSlot)
                return;

            InventoryService.AddItem(item, false);
            ForgeSlot.RemoveItem();
            DragNDrop.useSlotId = false;
        }
    }
}
