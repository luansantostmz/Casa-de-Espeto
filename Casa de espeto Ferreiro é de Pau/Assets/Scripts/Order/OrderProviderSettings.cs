using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Order Provider", menuName = "OrderProvider")]
public class OrderProviderSettings : ScriptableObject
{
    public int MinOrdersToInitialize;
    public int MaxOrdersToInitialize;
    public List<OrderProviderInterval> OrderIntervals = new List<OrderProviderInterval>();
}

[System.Serializable]
public class OrderProviderInterval
{
    public int Threshold;
    public int MinDelay;
    public int MaxDelay;
    public int MinOrderDeliveryTime;
    public int MaxOrderDeliveryTime;
    public bool CanRepeatItem;
    public List<ItemSettings> Items = new List<ItemSettings>();
    public int MinItemQuantity;
    public int MaxItemQuantity;
}
