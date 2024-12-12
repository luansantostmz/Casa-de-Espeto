using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Anvil : ItemContainer
{
    [Header("Anvil Settings")]
    [SerializeField] UIRecipes _recipes;
    [SerializeField] GameObject _hammerVFX;

    [SerializeField] TMP_Text _hammerCountText;

    [SerializeField] ItemDisplay _toCraftItemUI;
    [SerializeField] PingPongSlider _bar;

    ItemSettings _lastToCraftItem;

    public int _hammerCount;
    public int _qualityPoints;

    protected override void Awake()
    {
        base.Awake();

        _bar.gameObject.SetActive(false);
        _hammerCountText.gameObject.SetActive(false);

        GameEvents.Anvil.OnHammer += OnHammer;
    }

    private void OnDestroy()
    {
        GameEvents.Anvil.OnHammer -= OnHammer;
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

        StartCoroutine(FinishHammer());
    }

    IEnumerator FinishHammer()
    {
        _bar.gameObject.SetActive(false);
        yield return new WaitForSeconds(.5f);
        _bar.gameObject.SetActive(true);
        _hammerVFX.gameObject.SetActive(false);

        if (Items.Count > 0)
        {
            foreach (var item in Items)
            {
                Destroy(item.gameObject);
            }

            Items.Clear();
        }

        if (_hammerCount >= _lastToCraftItem.HammerCount)
        {
            GameEvents.Inventory.OnAddItem?.Invoke(
                _toCraftItemUI.Item, 
                _toCraftItemUI.Quality, 
                _toCraftItemUI.Quantity);

            _hammerCount = 0;
            _qualityPoints = 0;
            DropHandler.IsBlocked = false;
            _hammerCountText.gameObject.SetActive(false);
            UpdateToCraftItem();
            _lastToCraftItem = null;
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

        var toCraftItem = GetToCraftItem();

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
        }
        else
        {
            _toCraftItemUI.gameObject.SetActive(false);
            _bar.gameObject.SetActive(false);
        }
    }

    private ItemSettings GetToCraftItem()
    {
        List<ItemSettings> items = new List<ItemSettings>();

        foreach (var item in Items)
        {
            for (int i = 0; i < item.Quantity; i++)
            {
                items.Add(item.Item);
            }
        }

        foreach (var toCraftItem in _recipes.Items)
        {
            if (AreListsEqualIgnoringOrder(items, toCraftItem.Ingredients))
                return toCraftItem;
        }

        return null;
    }

    private bool AreListsEqualIgnoringOrder(List<ItemSettings> firstList, List<ItemSettings> secondList)
    {
        if (firstList.Count != secondList.Count)
            return false;

        // Cria dicionários para contar as ocorrências de cada item
        Dictionary<ItemSettings, int> firstListCounts = GetItemCounts(firstList);
        Dictionary<ItemSettings, int> secondListCounts = GetItemCounts(secondList);

        // Compara os dicionários
        foreach (var kvp in firstListCounts)
        {
            if (!secondListCounts.TryGetValue(kvp.Key, out int secondCount) || kvp.Value != secondCount)
            {
                return false;
            }
        }

        return true;
    }

    private Dictionary<ItemSettings, int> GetItemCounts(List<ItemSettings> list)
    {
        Dictionary<ItemSettings, int> itemCounts = new Dictionary<ItemSettings, int>();

        foreach (var item in list)
        {
            if (itemCounts.ContainsKey(item))
            {
                itemCounts[item]++;
            }
            else
            {
                itemCounts[item] = 1;
            }
        }

        return itemCounts;
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
    }

    public override void AddItem(UIItem uiItem)
    {
        base.AddItem(uiItem);
        UpdateToCraftItem();
    }
}
