using System;

public class GameEvents
{
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
        public static Action OnGoldSubtracted;
    }

    public class Order
    {
        public static Action<OrderData> OnOrderAdded;
        public static Action<OrderData> OnOrderComplete;
    }

    public class DragAndDrop
    {
        public static Action<DragAndDropObject> OnAnyDragStart;
        public static Action<DragAndDropObject, DropZone> OnAnyDragEnd;
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
}
