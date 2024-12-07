using System.Collections.Generic;
using UnityEngine;

public class InventoryService
{
    public static List<InventoryItem> Items { get; private set; } = new List<InventoryItem>();

    public static void AddItem(InventoryItem newItem, bool callAction = true)
    {
        if (Items.Contains(newItem)) 
            return;

        Debug.Log("Adding new item to inventory:" + newItem.Settings);

        Items.Add(newItem);

        if (callAction)
            GameEvents.Inventory.OnItemAdded?.Invoke(newItem);
    }

    public static void RemoveItem(InventoryItem item, bool callAction = true)
    {
        if (!Items.Contains(item))
            return;

        Debug.Log("Removing item from inventory:" + item.Settings);

        Items.Remove(item);

        if (callAction) 
            GameEvents.Inventory.OnItemRemoved?.Invoke(item);
    }

    public static int GetEqualsAmountInStock(InventoryItem item)
    {
        int count = 0;

        foreach(var i in Items)
        {
            if (i.Settings ==  item.Settings && i.Quality == item.Quality) 
                count++;
        }

        return count;
    }
}
