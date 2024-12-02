
[System.Serializable]
public class InventoryItem
{
    public ItemData Data;
    public int Quantity;

    public InventoryItem(ItemData item, int quantity = 1)
    {
        Data = item;
        Quantity = quantity;
    }
}
