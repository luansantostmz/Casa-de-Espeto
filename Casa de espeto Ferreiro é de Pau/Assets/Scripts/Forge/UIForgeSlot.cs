using UnityEngine;
using UnityEngine.UI;

public class UIForgeSlot : MonoBehaviour
{
    [SerializeField] ForgeBar _forgeBar;

    public UICardItem Item { get; private set; }

    private void Awake()
    {
        _forgeBar.StopBar();
    }

    private void Update()
    {
        if (Item == null)
            return;
     
        var quality = _forgeBar.GetCurrentQuality();
        Item.Item.Quality = quality;

        if (quality > 0 && Item.Item.Settings.MeltedItem)
            Item.Item.Settings = Item.Item.Settings.MeltedItem;

        Item.UpdateVisual();
    }

    public void SetItem(UICardItem item)
    {
        GetComponent<DropZone>().IsBlocked = true;

        var itemSettings = item.Item.Settings;

        Item = item;
        _forgeBar.forgeSettings = itemSettings.ForgeSettings;
        _forgeBar.StartBar();

        InventoryService.RemoveItem(item.Item);
    }

    public void RemoveItem()
    {
        GetComponent<DropZone>().IsBlocked = false;
        _forgeBar.StopBar();
    }
}
