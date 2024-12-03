
[System.Serializable]
public class InventoryItem
{
    public ItemSettings Settings;
    public int Quantity;
    public QualityType Quality;

    public InventoryItem(ItemSettings item, int quantity = 1, QualityType quality = QualityType.Bom)
    {
        Settings = item;
        Quantity = quantity;
        Quality = quality;
    }
}
