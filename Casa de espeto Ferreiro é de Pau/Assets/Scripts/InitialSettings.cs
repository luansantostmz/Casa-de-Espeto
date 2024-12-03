using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Initial Settings", menuName = "Initial Settings", order = 0)]
public class InitialSettings : ScriptableObject
{
    public int Gold;
    public List<InventoryItem> Items = new List<InventoryItem>();
}
