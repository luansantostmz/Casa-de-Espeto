using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            DataService.LoadData();
            Instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }

        Destroy(gameObject);
    }

    private void Start()
    {
        DataService.InitializeNewRun();

        GameEvents.OnGameOver += OnGameOver;
        GameEvents.Economy.OnEarnGold += OnEarnGold;
        GameEvents.Order.OnOrderComplete += OnOrderComplete;
        GameEvents.Order.OnOrderFail += OnOrderFail;
    }

    private void OnDestroy()
    {
        GameEvents.OnGameOver -= OnGameOver;
        GameEvents.Economy.OnEarnGold -= OnEarnGold;
        GameEvents.Order.OnOrderComplete -= OnOrderComplete;
        GameEvents.Order.OnOrderFail -= OnOrderFail;
    }

    private void OnGameOver()
    {
        DataService.EndRun();
    }

    private void OnEarnGold(int value)
    {
        DataService.CurrentRunData.GoldEarned += value;
    }

    private void OnOrderComplete(OrderData order)
    {
        if (GameManager.Instance.IsGameOver)
            return;

        var items = new List<ItemData>();
        foreach (var item in order.Items)
        {
            items.Add(new ItemData()
            {
                ItemName = item.ItemName,
            });
        }

        DataService.CachedData.TotalDeliveredOrders++;
        DataService.CurrentRunData.DeliveredOrders.Add(new SavedOrderData()
        {
            Reward = order.Reward,
            Items = items
        });
    }

    private void OnOrderFail(OrderData order)
    {
        if (GameManager.Instance.IsGameOver)
            return;

        var items = new List<ItemData>();
        foreach (var item in order.Items)
        {
            items.Add(new ItemData()
            {
                ItemName = item.ItemName,
            });
        }

        DataService.CachedData.TotalFailedOrders++;
        DataService.CurrentRunData.FailedOrders.Add(new SavedOrderData()
        {
            Reward = order.Reward,
            Items = items
        });
    }
}
