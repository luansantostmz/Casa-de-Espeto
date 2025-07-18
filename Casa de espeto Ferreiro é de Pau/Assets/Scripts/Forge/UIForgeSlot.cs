using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIForgeSlot : ItemContainer
{
    [Header("Forge")]
    [SerializeField] UIFillClock _clock;
    [SerializeField] int _currentLevel;
    [SerializeField] Button _upgradeButton;
    [SerializeField] TMP_Text _upgradePriceText;
    [SerializeField] TMP_Text _currentLevelText;

    public UIFillClock Clock => _clock;
    ForgeController _controller;
    ItemSettings _toForgeItem;

    bool OnMaxLevel => _currentLevel >= GameManager.Instance.GameplaySettings.ForgeUpgradeSettings.Count - 1;
    ForgeUpgradeData CurrentSettings => GameManager.Instance.GameplaySettings.ForgeUpgradeSettings[_currentLevel];

    protected override void Awake()
    {
        base.Awake();
        _clock.StopTime();
        _clock.OnComplete += ForgeItem;
        _upgradeButton.onClick.AddListener(Upgrade);

        GameEvents.Economy.OnGoldChanged += RefreshButton;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _clock.OnComplete -= ForgeItem;
        GameEvents.Economy.OnGoldChanged -= RefreshButton;
        _upgradeButton.onClick.RemoveListener(Upgrade);
    }

    public void Initialize(ForgeController controller)
    {
        _controller = controller;
        RefreshButton();
    }

    private void RefreshButton()
    {
        _upgradeButton.gameObject.SetActive(!OnMaxLevel);
        _upgradePriceText.text = CurrentSettings.UpgradePrice.ToString();
        _upgradeButton.interactable = EconomyService.CurrentGold >= CurrentSettings.UpgradePrice;
    }

    private void Upgrade()
    {
        _currentLevel++;
        var level = OnMaxLevel ? "max" : (_currentLevel + 1).ToString();
        _currentLevelText.text = $"Level {level}";

        EconomyService.SubtractGold(CurrentSettings.UpgradePrice);

        if (_toForgeItem)
            _clock.TotalTime = _toForgeItem.ForgeTime * CurrentSettings.LevelModifier;
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
            _clock.StartTime(_toForgeItem.ForgeTime * CurrentSettings.LevelModifier);
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
