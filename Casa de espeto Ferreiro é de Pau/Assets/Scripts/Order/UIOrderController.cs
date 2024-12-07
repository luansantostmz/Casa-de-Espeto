using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIOrderController : MonoBehaviour
{
    [SerializeField] UIOrder _orderPrefab;
    [SerializeField] RectTransform _container;

    Dictionary<OrderData, UIOrder> _orders = new Dictionary<OrderData, UIOrder>();

    public void Awake()
    {
        GameEvents.Order.OnOrderAdded += OnAddOrder;
        GameEvents.Order.OnOrderComplete += OnOrderComplete;
    }

    private void OnDestroy()
    {
        GameEvents.Order.OnOrderAdded -= OnAddOrder;
        GameEvents.Order.OnOrderComplete -= OnOrderComplete;
    }

    private void OnAddOrder(OrderData order)
    {
        var ui = Instantiate(_orderPrefab, _container);
        ui.Initialize(order);
        _orders.Add(order, ui);
    }

    private void OnOrderComplete(OrderData order)
    {
        _orders.TryGetValue(order, out UIOrder ui);
        _orders.Remove(order);
        Destroy(ui.gameObject);
    }
}
