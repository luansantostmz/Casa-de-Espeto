using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Initial Items", menuName = "Initial Items")]
public class InitialItemSettings : ScriptableObject
{
    public List<InitialItemData> Items = new List<InitialItemData>();
}

[System.Serializable]
public class InitialItemData
{
    public ItemSettings Item;
    public QualitySettings Quality;
    public int Quantity;
}
