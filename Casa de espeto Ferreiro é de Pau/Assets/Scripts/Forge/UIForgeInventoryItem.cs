using UnityEngine;

public class UIForgeInventoryItem : MonoBehaviour
{
    public UIInventoryItem InventoryItem;
    public UIForgeSlot ForgeSlot;

    DragAndDrop DragNDrop;

    private void Awake()
    {
        InventoryItem = GetComponent<UIInventoryItem>();
        DragNDrop = GetComponent<DragAndDrop>();
        DragNDrop.OnDrop += AddToSlot;
    }

    private void OnDestroy()
    {
        DragNDrop.OnDrop -= AddToSlot;
    }

    void AddToSlot(DropZone dropZone)
    {
        if (dropZone == null)
            return;

        var item = GetComponent<UIInventoryItem>().Item;

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

            InventoryService.AddItem(item.Settings, item.Quality);
            ForgeSlot.RemoveItem();
            DragNDrop.useSlotId = false;
            Destroy(gameObject);
        }
    }
}
