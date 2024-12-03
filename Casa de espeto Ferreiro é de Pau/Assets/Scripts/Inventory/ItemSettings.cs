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
}

public enum QualityType
{
    Common, Uncommon, Rare, SuperRare, Legendary
}