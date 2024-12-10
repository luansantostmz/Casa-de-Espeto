using System;

public class GameEvents
{
    public static Action OnGameOver;

    public class Reputation
    {
        public static Action OnReputationChanged;
    }

    public class Inventory
    {
        public static Action<ItemSettings, QualitySettings> OnNewItemAdded;
        //public static Action<CardItem> OnItemMovedToInventory;
        public static Action<CardItem> OnItemRemoved;

        public static Action<CardItem> OnCardItemAddedToInventory;
        public static Action<CardItem> OnCardItemRemovedFromInventory;
    }

    public class Economy
    {
        public static Action OnGoldAdded;
        public static Action<int> OnEarnGold;
        public static Action OnGoldSubtracted;
    }

    public class Order
    {
        public static Action<OrderData> OnOrderAdded;
        public static Action<OrderData> OnOrderComplete;
        public static Action<OrderData> OnOrderFail;
    }

    public class DragAndDrop
    {
        public static Action<DragAndDropObject> OnAnyDragStart;
        public static Action<DragAndDropObject, DropZone> OnAnyDragEnd;
    }

    public class Forge
    {
        public static Action<CardItem> OnItemAddedToForge;
    }

    public class Anvil
    {
        public static Action<CardItem> OnItemAddedToAnvil;
        public static Action<CardItem> OnItemRemovedFromAnvil;
        public static Action<ItemSettings> OnRecipeItemClicked;

        public static Action<QualitySettings> OnHammer;
    }
}
