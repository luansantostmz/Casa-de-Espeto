using System.Collections.Generic;
using System.Linq;

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

    public void Complete()
    {
        if (OrderState != OrderState.WaitingReward) return;

        OrderState = OrderState.Completed;

        GameManager.Instance.GainReputation();
        EconomyService.AddGold(Reward);

        GameEvents.Order.OnOrderComplete?.Invoke(this);
    }

    public void Fail()
    {
        if (OrderState == OrderState.Failed || OrderState == OrderState.Completed) return;

        OrderState = OrderState.Failed;
        GameManager.Instance.LoseReputation();
        GameEvents.Order.OnOrderFail?.Invoke(this);
    }
}

public enum OrderState
{
    Uncomplete, WaitingReward, Completed, Failed
}