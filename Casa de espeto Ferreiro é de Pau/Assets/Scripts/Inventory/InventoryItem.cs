
[System.Serializable]
public class InventoryItem
{
    public ItemSettings Settings;
    public int Quantity;

    public InventoryItem(ItemSettings item, int quantity = 1)
    {
        Settings = item;
        Quantity = quantity;
    }
}
