using System.Linq;
using UnityEngine;

public class UIForgeSlot : ItemContainer
{
    [Header("Forge")]
    [SerializeField] ForgeBar _forgeBar;
    public UIItem ToForgeItem
    {
        get
        {
            if (Items.Any()) return Items[0];
            return null;
        }
    }

    QualitySettings _lastQuality;

    protected override void Awake()
    {
        base.Awake();
        _forgeBar.StopBar();
    }


    private void Update()
    {
        if (ToForgeItem == null)
            return;
     
        var quality = _forgeBar.GetCurrentQuality();
        ToForgeItem.Quality = quality;

        //if (quality == null)
        //{
        //    Destroy(ToForgeItem.gameObject);
        //    RemoveItem(ToForgeItem);
        //    return;
        //}

        if (quality != _forgeBar.forgeSettings.valueRanges[0].quality && ToForgeItem.Item.MeltedItem)
            ToForgeItem.Item = ToForgeItem.Item.MeltedItem;

        if (quality && _lastQuality != quality)
        {
            ToForgeItem.UpdateVisual();
            _lastQuality = quality;
        }
    }

    public void SetItem(UIItem item)
    {
        DropHandler.IsBlocked = true;
        _lastQuality = item.Quality;

        var itemSettings = item.Item;

        _forgeBar.forgeSettings = itemSettings.ForgeSettings;

        if (item.Item.ForgeSettings) 
            _forgeBar.StartBar();
    }

    public override void AddItem(UIItem uiItem)
    {
        base.AddItem(uiItem);
        SetItem(uiItem);
    }

    public override void RemoveItem(UIItem item)
    {
        base.RemoveItem(item);
        DropHandler.IsBlocked = false;
        _forgeBar.StopBar();
    }
}
