using UnityEngine;

public class UIForgeInventoryItem : MonoBehaviour
{
    public UICardItem InventoryItem;
    public UIForgeSlot ForgeSlot;

    DragAndDrop DragNDrop;

    private void Awake()
    {
        InventoryItem = GetComponent<UICardItem>();
        DragNDrop = GetComponent<DragAndDrop>();
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

        if (dropZone.TryGetComponent(out UIForgeSlot slot) && !dropZone.IsBlocked)
        {
            if (item.Settings.MeltedItem == null)
                return;

            slot.SetItem(InventoryItem);
            ForgeSlot = slot;
            DragNDrop.useSlotId = true;
            InventoryService.RemoveItem(item);
        }
        else if (dropZone.TryGetComponent(out UIInventory inventory))
        {
            if (!ForgeSlot)
                return;

            InventoryService.AddItem(item, false);
            ForgeSlot.RemoveItem();
            DragNDrop.useSlotId = false;
        }
    }
}
