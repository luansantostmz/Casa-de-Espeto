using TMPro;
using UnityEngine;

public class UIForgeSlot : ItemContainer
{
    [Header("Forge")]
    [SerializeField] UIFillClock _clock;
    [SerializeField] TMP_Text _text;

    ForgeController _controller;
    ItemSettings _toForgeItem;

    protected override void Awake()
    {
        base.Awake();
        _clock.StopTime();
        _clock.OnComplete += ForgeItem;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _clock.OnComplete -= ForgeItem;
    }

    public void Initialize(ForgeController controller)
    {
        _controller = controller;
    }

    private void ForgeItem()
    {
        foreach (var item in Items)
        {
            Destroy(item.gameObject);
        }

        Items.Clear();

        InstantiateNewItem(_toForgeItem, QualityProvider.Instance.GetFirstQuality(), 1);
        _clock.StopTime();
    }

    private void CheckItem()
    {
        _toForgeItem = _controller.Recipes.GetToCraftItem(Items);

        if (_toForgeItem)
        {
            _clock.StartTime(_toForgeItem.ForgeTime);
            return;
        }

        _clock.StopTime();
    }

    public override void AddItem(UIItem uiItem)
    {
        base.AddItem(uiItem);
        CheckItem();
    }

    public override void RemoveItem(UIItem item)
    {
        base.RemoveItem(item);
        CheckItem();

        //if (AchievementsManager.Instance.OnFirstCopperForge.CompareAuxObject(item.Item) && _itemOnDropped != item.Item)
        //    AchievementsManager.Instance.OnFirstCopperForge.TryAchieve();
        //if (AchievementsManager.Instance.OnFirstIronForged.CompareAuxObject(item.Item) && _itemOnDropped != item.Item)
        //    AchievementsManager.Instance.OnFirstIronForged.TryAchieve();
    }
}
