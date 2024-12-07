using System.Collections.Generic;

[System.Serializable]
public class OrderData
{
    public List<InventoryItem> Items = new List<InventoryItem>();
    public int Reward;

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
        EconomyService.AddGold(Reward);

        foreach (var item in Items)
        {
            InventoryService.RemoveItem(item);
        }
    }
}
