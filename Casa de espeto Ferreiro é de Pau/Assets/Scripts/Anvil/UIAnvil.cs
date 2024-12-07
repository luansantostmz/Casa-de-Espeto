using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIAnvil : MonoBehaviour
{
    [SerializeField] UIRecipes _recipes;
    [SerializeField] int _maxItems;
    [SerializeField] GameObject _hammerVFX;

    [SerializeField] TMP_Text _hammerCountText;

    [SerializeField] UICardItem _toCraftItemUI;
    [SerializeField] PingPongSlider _bar;

    List<UICardItem> _items =  new List<UICardItem>();
    DropZone _dropZone;

    ItemSettings _lastToCraftItem;

    public int _hammerCount;
    public int _qualityPoints;

    private void Awake()
    {
        _dropZone = GetComponentInChildren<DropZone>(true);
        _bar.gameObject.SetActive(false);
        _hammerCountText.gameObject.SetActive(false);

        GameEvents.Anvil.OnItemAddedToAnvil += AddItem;
        GameEvents.Anvil.OnItemRemovedFromAnvil += RemoveItem;
        GameEvents.Anvil.OnHammer += OnHammer;

    }

    private void OnDestroy()
    {
        GameEvents.Anvil.OnItemAddedToAnvil -= AddItem;
        GameEvents.Anvil.OnItemRemovedFromAnvil -= RemoveItem;
        GameEvents.Anvil.OnHammer -= OnHammer;
    }

    private void OnHammer(QualitySettings quality)
    {
        _qualityPoints += quality.Points;
        _qualityPoints = _qualityPoints / 2;
        UpdateToCraftItem(true);
        _bar.RestartBar();
        _hammerVFX.gameObject.SetActive(true);

        _hammerCount++;

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

        if (_hammerCount >= _lastToCraftItem.HammerCount)
        {
            InventoryService.AddItem(_toCraftItemUI.Item);
            
            foreach(var item in _items)
            {
                Destroy(item.gameObject);
            }

            _dropZone.IsBlocked = false;
            _qualityPoints = 0;
            _hammerCount = 0;
            _hammerCountText.gameObject.SetActive(false);

            _items.Clear();

            UpdateToCraftItem();
        }
    }

    private void UpdateToCraftItem(bool forceUpdateVisual = false)
    {
        var toCraftItem = GetToCraftItem();

        if (_lastToCraftItem == toCraftItem && !forceUpdateVisual)
            return;

        _lastToCraftItem = toCraftItem;
        if (toCraftItem != null)
        {
            _bar.SetQTESettings(toCraftItem.AnvilSettings);
            _bar.gameObject.SetActive(true);

            if (_qualityPoints == 0)
            {
                int ignoreCount = 0;
                foreach (var ingredient in _items)
                {
                    if (ingredient.Item.Settings.IgnoreQualityOnAnvil)
                    {
                        ignoreCount++;
                        continue;
                    }

                    _qualityPoints += ingredient.Item.Quality.Points;
                }

                _qualityPoints = _qualityPoints / (_items.Count - ignoreCount);
            }

            var quality = QualityProvider.Instance.GetQualityByPoints(_qualityPoints);
            _toCraftItemUI.SetItem(new InventoryItem(toCraftItem, quality));
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

        foreach (var item in _items)
        {
            items.Add(item.Item.Settings);
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

        HashSet<ItemSettings> firstSet = new HashSet<ItemSettings>(firstList);
        HashSet<ItemSettings> secondSet = new HashSet<ItemSettings>(secondList);

        return firstSet.SetEquals(secondSet);
    }

    public void AddItem(UICardItem item)
    {
        if (_items.Contains(item)) 
            return;

        _items.Add(item);

        if (_items.Count >= _maxItems)
            _dropZone.IsBlocked = true;

        UpdateToCraftItem();
    }

    public void RemoveItem(UICardItem item)
    {
        _items.Remove(item);

        _dropZone.IsBlocked = false;
        UpdateToCraftItem();
        _qualityPoints = 0;
        _hammerCount = 0;
        _hammerCountText.gameObject.SetActive(false);
    }
}
