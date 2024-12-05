using System;

public class GameEvents
{
    public class Inventory
    {
        public static Action<InventoryItem> OnItemAdded;
        public static Action<InventoryItem> OnItemRemoved;
    }

    public class Economy
    {
        public static Action OnGoldAdded;
        public static Action OnGoldSubtracted;
    }

    public class Forge
    {
        public static Action<UIInventoryItem> OnItemAddedToForge;
    }
}
