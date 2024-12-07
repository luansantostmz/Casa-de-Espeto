using UnityEngine;

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

        if (quality != _forgeBar.forgeSettings.valueRanges[0].quality && Item.Item.Settings.MeltedItem)
            Item.Item.Settings = Item.Item.Settings.MeltedItem;

        Item.UpdateVisual();
    }

    public void SetItem(UICardItem item)
    {
        GetComponentInChildren<DropZone>(true).IsBlocked = true;

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
