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
        public static Action<InventoryItem> OnItemAdded;
        public static Action<InventoryItem> OnItemRemoved;
        public static Action<InventoryItem> OnItemDestroyed;

        public static Action<UICardItem> OnCardItemAddedToInventory;
        public static Action<UICardItem> OnCardItemRemovedFromInventory;
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
        public static Action<UIDragHandler> OnDragStarted;
        public static Action<UIDragHandler, UIDropHandler> OnDragEnded;

        public static Action<DragAndDropObject> OnDragStart;
        public static Action<DragAndDropObject, DropZone> OnDragEnd;
    }

    public class Forge
    {
        public static Action<UICardItem> OnItemAddedToForge;
    }

    public class Anvil
    {
        public static Action<UICardItem> OnItemAddedToAnvil;
        public static Action<UICardItem> OnItemRemovedFromAnvil;
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
