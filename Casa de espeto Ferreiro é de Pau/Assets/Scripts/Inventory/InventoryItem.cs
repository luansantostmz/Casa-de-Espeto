
[System.Serializable]
public class InventoryItem
{
    public ItemSettings Settings;
    public QualitySettings Quality;
    
    public InventoryItem(ItemSettings item, QualitySettings quality)
    {
        Settings = item;
        Quality = quality;
    }
}
