
public class UIInventory : ItemContainer
{
    protected override void Awake()
    {
        base.Awake();
        GameEvents.Inventory.OnAddItem += InstantiateNewItem;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        GameEvents.Inventory.OnAddItem -= InstantiateNewItem;
    }
}
