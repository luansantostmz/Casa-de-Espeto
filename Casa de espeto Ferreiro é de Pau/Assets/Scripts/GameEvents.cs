using System;

public class GameEvents
{
    public static Action OnGameOver;
    public static Action<AchievementSettings> OnTryGetAchievement;

    public class Reputation
    {
        public static Action OnReputationChanged;
    }

    public class Economy
    {
        public static Action OnGoldChanged;
        public static Action<int> OnEarnGold;
    }

    public class Inventory
    {
        public static Action<ItemSettings, QualitySettings, int> OnAddItem;
    }

    public class Order
    {
        public static Action<OrderData> OnOrderAdded;
        public static Action<OrderData> OnOrderComplete;
        public static Action<OrderData> OnOrderFail;
    }

    public class DragAndDrop
    {
        public static Action<UIDragHandler> OnAnyDragStart;
        public static Action<UIDragHandler, UIDropHandler> OnAnyDragEnd;
    }

    public class Anvil
    {
        public static Action<ItemSettings> OnRecipeItemClicked;

        public static Action<QualitySettings> OnHammer;
    }

    public class Audio 
    {
        public static Action<float> OnMasterChanged;
        public static Action<float> OnMusicChanged;
        public static Action<float> OnSFXChanged;
    }

    public class Cursor
    {
        public static Action OnCursorToIdle;
        public static Action OnCursorToDrag;
        public static Action OnCursorDragging;
    }
}
