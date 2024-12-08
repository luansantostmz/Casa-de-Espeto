using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class OrderData
{
    public int OrderId;
    public List<InventoryItem> Items = new List<InventoryItem>();
    public int DeliveryTime;
    public float RemainingTime;
    public int Reward;

    public List<InventoryItem> DeliveredItems = new List<InventoryItem>();

    public bool IsCompleted;
    public bool IsFailed;

    public List<InventoryItem> GetItemsInStock()
    {
        var organizedList = new List<InventoryItem>(Items);
        organizedList.OrderByDescending(item => item.Quality.Points).ToList();

        List<InventoryItem> inStock = new List<InventoryItem>();

        foreach (var orderItem in organizedList)
        {
            foreach (InventoryItem inventoryItem in InventoryService.Items)
            {
                if (inventoryItem.Settings == orderItem.Settings && 
                    (inventoryItem.Quality.Points >= orderItem.Quality.Points) &&
                    !inStock.Contains(inventoryItem))
                {
                    inStock.Add(inventoryItem);
                    break;
                }
            }
        }

        return inStock;
    }

    public bool HaveAllItems()
    {
        return GetItemsInStock().Count >= Items.Count;
    }

    public void DestroyItems()
    {
        DeliveredItems = GetItemsInStock();

        foreach (var item in DeliveredItems)
        {
            GameEvents.Inventory.OnItemDestroyed?.Invoke(item);
        }
    }

    public void Complete()
    {
        if (IsFailed || IsCompleted) return;

        IsCompleted = true;

        GameManager.Instance.GainReputation();
        EconomyService.AddGold(Reward);

        foreach (var item in GetItemsInStock())
        {
            InventoryService.RemoveItem(item);
        }

        GameEvents.Order.OnOrderComplete?.Invoke(this);
    }

    public void Fail()
    {
        if (IsFailed || IsCompleted) return;

        IsFailed = true;
        GameManager.Instance.LoseReputation();
        GameEvents.Order.OnOrderFail?.Invoke(this);
    }
}
