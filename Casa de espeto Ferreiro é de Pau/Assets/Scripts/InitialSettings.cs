using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Initial Settings", menuName = "Initial Settings", order = 0)]
public class InitialSettings : ScriptableObject
{
    public int Gold;
    public List<InitialItemData> Items = new List<InitialItemData>();
}

[System.Serializable]
public class InitialItemData
{
    public ItemSettings Item;
    public QualitySettings Quality;
    public int Quantity;
}