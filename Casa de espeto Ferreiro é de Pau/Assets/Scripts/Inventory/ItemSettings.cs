using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item", order = 0)]
public class ItemSettings : ScriptableObject
{
    public string ItemName;
    public int BasePrice;
    public Sprite Sprite;
    public bool IsStackable;
    public ItemSettings MeltedItem;
    public ForgeSettings ForgeSettings;
}

public enum QualityType
{
    Bom, Otimo, Perfeito
}