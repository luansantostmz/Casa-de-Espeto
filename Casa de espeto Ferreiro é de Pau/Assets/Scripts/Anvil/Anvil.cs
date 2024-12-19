using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Anvil : ItemContainer
{
    [Header("Anvil Settings")]
    [SerializeField] Recipes _recipes;
    [SerializeField] GameObject _hammerVFX;

    [SerializeField] TMP_Text _hammerCountText;

    [SerializeField] ItemDisplay _toCraftItemUI;
    [SerializeField] PingPongSlider _bar;
    [SerializeField] Button _startHammerButton;

    bool _hammerEnabled;

    ItemSettings _lastToCraftItem;

    public int _hammerCount;
    public int _qualityPoints;

    protected override void Awake()
    {
        base.Awake();

        _bar.gameObject.SetActive(false);
        _hammerCountText.gameObject.SetActive(false);
        _startHammerButton.gameObject.SetActive(false);

        GameEvents.Anvil.OnHammer += OnHammer;

        _startHammerButton.onClick.AddListener(StartHammer);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        GameEvents.Anvil.OnHammer -= OnHammer;

        _startHammerButton.onClick.AddListener(StartHammer);
    }

    private void StartHammer()
    {
        DropHandler.IsBlocked = true;

        _hammerEnabled = true;
        _startHammerButton.gameObject.SetActive(false);
        _toCraftItemUI.gameObject.SetActive(true);
        _bar.gameObject.SetActive(true);

        foreach (var item in Items)
        {
            Destroy(item.gameObject);
        }

        Items.Clear();
    }

    private void OnHammer(QualitySettings quality)
    {
        DropHandler.IsBlocked = true;
        _qualityPoints += quality.Points;
        _qualityPoints = _qualityPoints / 2;
        _hammerCount++;
        _toCraftItemUI.GetComponent<ScaleDoTween>().PlayTween();
        UpdateToCraftItem(true);
        _bar.RestartBar();
        _hammerVFX.gameObject.SetActive(true);

        _hammerCountText.gameObject.SetActive(true);
        _hammerCountText.text = $"{_hammerCount}/{_lastToCraftItem.HammerCount}";

        StartCoroutine(ReloadHammer());
    }

    IEnumerator ReloadHammer()
    {
        _bar.gameObject.SetActive(false);
        yield return new WaitForSeconds(.5f);
        _bar.gameObject.SetActive(true);
        _hammerVFX.gameObject.SetActive(false);

        if (_hammerCount >= _lastToCraftItem.HammerCount)
        {
            GameEvents.Inventory.OnAddItem?.Invoke(
                _toCraftItemUI.Item, 
                _toCraftItemUI.Quality, 
                _toCraftItemUI.Quantity);

            AchievementsManager.Instance.OnFirstCraft.TryAchieve();

            _hammerCount = 0;
            _qualityPoints = 0;
            DropHandler.IsBlocked = false;
            _hammerCountText.gameObject.SetActive(false);
            UpdateToCraftItem();
            _lastToCraftItem = null;
            _startHammerButton.gameObject.SetActive(false);
            _hammerEnabled = false;
        }
    }

    private void UpdateToCraftItem(bool forceUpdateVisual = false)
    {
        if (_hammerCount > 0)
        {
            var quality = QualityProvider.Instance.GetQualityByPoints(_qualityPoints);
            _toCraftItemUI.UpdateVisual(_lastToCraftItem, quality, 1);
            _toCraftItemUI.gameObject.SetActive(true);

            return;
        }

        var toCraftItem = _recipes.GetToCraftItem(Items);

        if (_lastToCraftItem == toCraftItem && !forceUpdateVisual)
            return;

        if (toCraftItem != null)
        {
            _lastToCraftItem = toCraftItem;

            _bar.SetQTESettings(toCraftItem.AnvilSettings);
            _bar.gameObject.SetActive(true);

            if (_qualityPoints == 0)
            {
                int ignoreCount = 0;
                foreach (var ingredient in Items)
                {
                    if (ingredient.Item.IgnoreQualityOnAnvil)
                    {
                        ignoreCount++;
                        continue;
                    }

                    _qualityPoints += ingredient.Quality.Points;
                }

                _qualityPoints = _qualityPoints / (Items.Count - ignoreCount);
            }

            var quality = QualityProvider.Instance.GetQualityByPoints(_qualityPoints);
            _toCraftItemUI.UpdateVisual(toCraftItem, quality, 1);
            _toCraftItemUI.gameObject.SetActive(true);

            if (!_hammerEnabled)
            {
                _startHammerButton.gameObject.SetActive(true);
                _bar.gameObject.SetActive(false);
            }
            else
            {
            }
        }
        else
        {
            _toCraftItemUI.gameObject.SetActive(false);
            _bar.gameObject.SetActive(false);
            _startHammerButton.gameObject.SetActive(false);
        }
    }

    public override void RemoveItem(UIItem uiItem)
    {
        base.RemoveItem(uiItem);

        DropHandler.IsBlocked = false;
        UpdateToCraftItem();
        _lastToCraftItem = null;
        _qualityPoints = 0;
        _hammerCount = 0;
        _hammerCountText.gameObject.SetActive(false);
        _startHammerButton.gameObject.SetActive(false);
        _hammerEnabled = false;
    }

    public override void AddItem(UIItem uiItem)
    {
        base.AddItem(uiItem);
        UpdateToCraftItem();
    }
}
