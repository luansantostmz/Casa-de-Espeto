using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item", order = 0)]
public class ItemData : ScriptableObject
{
    public string ItemName;
    public int BasePrice;
    public Sprite Sprite;
    public bool IsStackable;
}
