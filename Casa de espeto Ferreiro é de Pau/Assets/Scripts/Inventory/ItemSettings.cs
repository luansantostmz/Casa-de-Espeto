using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item", order = 0)]
public class ItemSettings : ScriptableObject
{
    public string ItemName;
    public int BasePrice;
    public Sprite Sprite;
    public ItemSettings MeltedItem;
    public ForgeSettings ForgeSettings;
    public List<ItemSettings> Ingredients = new List<ItemSettings>();
}

public enum QualityType
{
    Good, Great, Perfect
}