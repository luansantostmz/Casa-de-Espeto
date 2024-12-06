using System;

public class GameEvents
{
    public class Inventory
    {
        public static Action<InventoryItem> OnItemAdded;
        public static Action<InventoryItem> OnItemRemoved;

        public static Action<UICardItem> OnCardItemAddedToInventory;
        public static Action<UICardItem> OnCardItemRemovedFromInventory;
    }

    public class Economy
    {
        public static Action OnGoldAdded;
        public static Action OnGoldSubtracted;
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
    }
}
