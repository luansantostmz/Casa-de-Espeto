using Unity.VisualScripting;
using UnityEngine;

public class UIForgeSlot : MonoBehaviour
{
    [SerializeField] ForgeBar _forgeBar;
    [SerializeField] DropZone _dropZone;

    public UICardItem Item { get; private set; }

    QualitySettings _lastQuality;

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

        if (_lastQuality != quality)
        {
            Item.UpdateVisual();
            _lastQuality = quality;
        }
    }

    public void SetItem(UICardItem item)
    {
        _lastQuality = item.Item.Quality;
        GetComponentInChildren<DropZone>(true).IsBlocked = true;

        var itemSettings = item.Item.Settings;

        Item = item;
        _forgeBar.forgeSettings = itemSettings.ForgeSettings;
        _forgeBar.StartBar();

        InventoryService.RemoveItem(item.Item);
    }

    public void RemoveItem()
    {
        _dropZone.IsBlocked = false;
        _forgeBar.StopBar();
    }
}
