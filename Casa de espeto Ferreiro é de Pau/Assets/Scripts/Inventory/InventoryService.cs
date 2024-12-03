using System.Collections.Generic;

public class InventoryService
{
    public static List<InventoryItem> Items { get; private set; } = new List<InventoryItem>();

    public static void AddItem(ItemSettings itemData, int quantity = 1)
    {
        if (itemData.IsStackable)
        {
            InventoryItem existingItem = Items.Find(i => i.Settings == itemData);
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

    public static void RemoveItem(ItemSettings itemData, int quantity = 1)
    {
        InventoryItem existingItem = Items.Find(i => i.Settings == itemData);

        if (existingItem != null)
        {
            if (existingItem.Settings.IsStackable)
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
