using Unity.VisualScripting;
using UnityEngine;

public class UIForgeSlot : MonoBehaviour
{
    [SerializeField] ForgeBar _forgeBar;
    [SerializeField] DropZone _dropZone;

    public CardItem Item { get; private set; }

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
        Item.Quality = quality;

        if (quality == null)
        {
            Destroy(Item.gameObject);
            RemoveItem();
            Item = null;
            return;
        }

        if (quality != _forgeBar.forgeSettings.valueRanges[0].quality && Item.Item.MeltedItem)
            Item.Item = Item.Item.MeltedItem;

        if (_lastQuality != quality)
        {
            Item.UpdateVisual();
            _lastQuality = quality;
        }
    }

    public void SetItem(CardItem item)
    {
        _lastQuality = item.Quality;
        GetComponentInChildren<DropZone>(true).IsBlocked = true;

        var itemSettings = item.Item;

        Item = item;
        _forgeBar.forgeSettings = itemSettings.ForgeSettings;
        _forgeBar.StartBar();

        InventoryService.RemoveItem(item);
    }

    public void RemoveItem()
    {
        _dropZone.IsBlocked = false;
        _forgeBar.StopBar();
    }
}
