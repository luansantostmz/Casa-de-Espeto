using UnityEngine;

public class InventoryService
{
    public static void AddItem(ItemSettings item, QualitySettings quality)
    {
        Debug.Log("Adding new item to inventory:" + item.ItemName);
        GameEvents.Inventory.OnNewItemAdded?.Invoke(item, quality);
    }

    public static void MoveItemToInventory(CardItem item)
    {
        Debug.Log("Moving item to inventory:" + item.Item.ItemName);
        //GameEvents.Inventory.OnItemMovedToInventory?.Invoke(item);
    }

    public static void RemoveItem(CardItem item)
    {
        Debug.Log("Removing item from inventory:" + item.Item);
        GameEvents.Inventory.OnItemRemoved?.Invoke(item);
    }

    public static CardItem InstantiateCardItem(CardItem itemPrefab, ItemSettings item, QualitySettings quality, RectTransform container)
    {
        var newItemUI = GameObject.Instantiate(itemPrefab, container);
        newItemUI.SetItem(item, quality);

        return newItemUI;
    }
}
