using UnityEngine;

public class UIInventoryDraggableItem : MonoBehaviour
{
    public UICardItem InventoryItem;
    private DragAndDropObject DragNDrop;

    private DropZone LastDropZone;
    private DropZone CurrentDropZone;

    private void Awake()
    {
        DragNDrop = GetComponent<DragAndDropObject>();
        InventoryItem = GetComponent<UICardItem>();

        DragNDrop.OnDrop += OnDrop;
    }

    private void OnDestroy()
    {
        DragNDrop.OnDrop -= OnDrop;
    }

    private void OnDrop(DropZone dropZone)
    {
        if (dropZone == null)
            return;

        CurrentDropZone = dropZone;

        if (dropZone.DropZoneOwner.TryGetComponent(out UIForgeSlot forgeSlot))
        {
            HandleForgeSlotDrop(forgeSlot, dropZone);
        }
        else if (dropZone.DropZoneOwner.TryGetComponent(out Anvil anvil))
        {
            HandleAnvilDrop(anvil);
        }
        else if (dropZone.DropZoneOwner.TryGetComponent(out UIInventory inventory))
        {
            HandleInventoryDrop();
        }
        LastDropZone = CurrentDropZone;
    }

    private void HandleForgeSlotDrop(UIForgeSlot forgeSlot, DropZone dropZone)
    {
        if (InventoryItem.Item.Settings.MeltedItem == null || dropZone.IsBlocked)
            return;

        forgeSlot.SetItem(InventoryItem);
        DragNDrop.useSlotId = true;
        OldInventoryService.RemoveItem(InventoryItem.Item);
    }

    private void HandleAnvilDrop(Anvil anvil)
    {
        GameEvents.Inventory.OnCardItemRemovedFromInventory?.Invoke(InventoryItem);
        GameEvents.Anvil.OnItemAddedToAnvil?.Invoke(InventoryItem);
    }

    private void HandleInventoryDrop()
    {
        if (LastDropZone.DropZoneOwner.TryGetComponent(out UIForgeSlot forgeSlot))
        {
            OldInventoryService.AddItem(InventoryItem.Item, false);
            forgeSlot.RemoveItem();
            DragNDrop.useSlotId = false;
        }
        else if (LastDropZone.DropZoneOwner.TryGetComponent(out Anvil anvil))
        {
            GameEvents.Inventory.OnCardItemAddedToInventory?.Invoke(InventoryItem);
            GameEvents.Anvil.OnItemRemovedFromAnvil?.Invoke(InventoryItem);
        }
    }
}
