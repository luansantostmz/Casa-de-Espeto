
[System.Serializable]
public class InventoryItem
{
    public ItemSettings Settings;
    public QualityType Quality;

    public InventoryItem(ItemSettings item, QualityType quality = QualityType.Bom)
    {
        Settings = item;
        Quality = quality;
    }
}
