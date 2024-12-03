

public class UIForgeInventoryItem : BaseButton
{
    protected override void OnClick()
    {
        base.OnClick();

        var itemSettings = GetComponent<UIInventoryItem>().Item.Settings;
        if (itemSettings.MeltedItem == null)
            return;

        GameEvents.Forge.OnItemAddedToForge?.Invoke(itemSettings);
    }
}
