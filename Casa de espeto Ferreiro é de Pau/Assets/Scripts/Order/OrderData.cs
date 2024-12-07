using System.Collections.Generic;

[System.Serializable]
public class OrderData
{
    public int OrderId;
    public List<InventoryItem> Items = new List<InventoryItem>();
    public int DeliveryTime;
    public float RemainingTime;
    public int Reward;

    public bool IsCompleted;
    public bool IsFailed;

    public List<InventoryItem> GetItemsInStock()
    {
        List<InventoryItem> inStock = new List<InventoryItem>();

        foreach (var orderItem in Items)
        {
            foreach (InventoryItem inventoryItem in InventoryService.Items)
            {
                if (inventoryItem.Settings == orderItem.Settings && 
                    inventoryItem.Quality == orderItem.Quality &&
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
