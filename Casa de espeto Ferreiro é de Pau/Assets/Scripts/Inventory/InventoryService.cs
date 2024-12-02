using System.Collections.Generic;

public class InventoryService
{
    public static List<InventoryItem> Items { get; private set; } = new List<InventoryItem>();

    public void AddItem(ItemData itemData, int quantity = 1)
    {
        if (itemData.IsStackable)
        {
            InventoryItem existingItem = Items.Find(i => i.Data == itemData);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                GameEvents.Inventory.OnItemAdded?.Invoke(existingItem);
                return;
            }
        }

        InventoryItem newItem = new InventoryItem(itemData, quantity);
        Items.Add(newItem);
        GameEvents.Inventory.OnItemAdded?.Invoke(newItem);
    }

    public void RemoveItem(ItemData itemData, int quantity = 1)
    {
        InventoryItem existingItem = Items.Find(i => i.Data == itemData);

        if (existingItem != null)
        {
            if (existingItem.Data.IsStackable)
            {
                existingItem.Quantity -= quantity;
                if (existingItem.Quantity <= 0)
                {
                    Items.Remove(existingItem);
                }
            }
            else
            {
                Items.Remove(existingItem);
            }

            GameEvents.Inventory.OnItemRemoved?.Invoke(existingItem);
        }
    }
}
