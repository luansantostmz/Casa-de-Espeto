using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderProvider : MonoBehaviour
{
    [Expandable]
    public OrderProviderSettings OrderSettings;

    public static OrderProvider Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
            return;
        }

        Destroy(gameObject);
    }

    private void Start()
    {
        int initialOrders = Random.Range(OrderSettings.MinOrdersToInitialize, OrderSettings.MaxOrdersToInitialize + 1);
        for (int i = 0; i < initialOrders; i++)
        {
            AddRandomOrder();
        }

        StartCoroutine(AddOrderRoutine());
    }

    private IEnumerator AddOrderRoutine()
    {
        var settings = GetCurrentOrderSettings();

        float delay = Random.Range(settings.MinDelay, settings.MaxDelay);

        yield return new WaitForSeconds(delay);

        AddRandomOrder();

        StartCoroutine(AddOrderRoutine());
    }

    private void AddRandomOrder()
    {
        OrderService.AddNewOrder(GetRandomOrderData());
    }

    private OrderData GetRandomOrderData()
    {
        var settings = GetCurrentOrderSettings();

        int itemQuantity = Random.Range(settings.MinItemQuantity, settings.MaxItemQuantity + 1);
        List<ItemSettings> items = new List<ItemSettings>();

        if (settings.CanRepeatItem)
        {
            List<ItemSettings> poolItems = new List<ItemSettings>(settings.Items);
            poolItems.Shuffle();

            for (int i = 0; i < itemQuantity; i++)
            {
                items.Add(poolItems[Random.Range(0, poolItems.Count)]);
            }
        }
        else
        {
            Queue<ItemSettings> poolItems = new Queue<ItemSettings>(settings.Items);
            poolItems.Shuffle();

            for (int i = 0; i < itemQuantity; i++)
            {
                items.Add(poolItems.Dequeue());
            }
        }

        int deliveryTime = Random.Range(settings.MinOrderDeliveryTime, settings.MaxOrderDeliveryTime + 1);
        
        int reward = 0;
        foreach (var item in items)
        {
            reward += item.BasePrice;
        }

        return new OrderData()
        {
            Items = items,
            Reward = reward,
            DeliveryTime = deliveryTime,
            RemainingTime = deliveryTime
        };
    }

    private OrderProviderInterval GetCurrentOrderSettings()
    {
        var list = new List<OrderProviderInterval>(OrderSettings.OrderIntervals);
        list.Reverse();

        foreach (var settings in list)
        {
            if (OrderService.OrderCount >= settings.Threshold)
            {
                return settings;
            }
        }

        return list[0];
    }
}
