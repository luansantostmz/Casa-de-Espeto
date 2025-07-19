using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OrderData
{
    public int OrderId;
    public List<ItemSettings> Items = new List<ItemSettings>();
    public int DeliveryTime;
    public float RemainingTime;
    public int Reward;

    public List<InventoryItem> DeliveredItems = new List<InventoryItem>();

    public OrderState OrderState;

    public void GiveRewards()
    {
        GameManager.Instance.AddReputation(GetReputationBasedOnDeliveredItems());
        EconomyService.AddGold(GetGoldBasedOnDeliveredItems());
    }

    public void LoseReputation()
    {
        GameManager.Instance.LoseReputation();
    }

    public void Complete()
    {
        if (OrderState != OrderState.WaitingReward) return;
        OrderState = OrderState.Completed;
        GameEvents.Order.OnOrderComplete?.Invoke(this);
    }

    public void Fail()
    {
        if (OrderState == OrderState.Failed || OrderState == OrderState.Completed) return;
        OrderState = OrderState.Failed;
        GameEvents.Order.OnOrderFail?.Invoke(this);
    }

    public int GetReputationBasedOnDeliveredItems()
    {
        var value = 0;

        DeliveredItems.ForEach(item =>
        {
            var qualityModifier = item.Quality.DeliverReputationModifier;
            value += Mathf.RoundToInt(qualityModifier * GameManager.Instance.GameplaySettings.ReputationToAddOnDeliver);
        });

        value = value / DeliveredItems.Count;
        return value;
    }

    public int GetGoldBasedOnDeliveredItems()
    {
        var value = 0;

        DeliveredItems.ForEach(item =>
        {
            var qualityModifier = item.Quality.DeliverGoldModifier;
            value += Mathf.RoundToInt(qualityModifier * item.Settings.BasePrice);
        });

        value = value / DeliveredItems.Count;
        return value;
    }
}

public enum OrderState
{
    Uncomplete, WaitingReward, Completed, Failed
}