using System;

[System.Serializable]
public class InventoryItem
{
    public ItemSettings Settings;
    public QualitySettings Quality;
    public int Quantity;

    public InventoryItem(ItemSettings item, QualitySettings quality, int quantity = 1)
    {
        Settings = item;
        Quality = quality;
        Quantity = quantity;
    }

    public InventoryItem GetClone()
    {
        return new InventoryItem(Settings, Quality, Quantity);
    }

    // Verifica se outro item é idêntico (para empilhamento)
    public bool IsIdentical(InventoryItem other)
    {
        return Settings == other.Settings && Quality == other.Quality;
    }
}