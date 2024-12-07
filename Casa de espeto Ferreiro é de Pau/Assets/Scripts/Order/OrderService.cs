using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderService 
{
    public static int OrderCount = 0;

    public static void AddNewOrder(OrderData order)
    {
        OrderCount++;
        order.OrderId = OrderCount;
        GameEvents.Order.OnOrderAdded?.Invoke(order);
    }
}
