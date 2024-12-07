using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item", order = 0)]
public class ItemSettings : ScriptableObject
{
    public string ItemName;
    public bool IgnoreQualityOnAnvil;
    public int BasePrice;
    public Sprite Sprite;
    public ItemSettings MeltedItem;
    public QTESettings ForgeSettings;
    public QTEAnvilSettings AnvilSettings;
    public List<ItemSettings> Ingredients = new List<ItemSettings>();
}