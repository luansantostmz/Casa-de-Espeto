using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class UIAnvil : MonoBehaviour
{
    [SerializeField] UIRecipes _recipes;
    [SerializeField] int _maxItems;

    [SerializeField] UICardItem _toCraftItemUI;

    List<UICardItem> _items =  new List<UICardItem>();
    DropZone _dropZone;

    ItemSettings _lastToCraftItem;

    private void Awake()
    {
        _dropZone = GetComponent<DropZone>();

        GameEvents.Anvil.OnItemAddedToAnvil += AddItem;
        GameEvents.Anvil.OnItemRemovedFromAnvil += RemoveItem;
    }

    private void OnDestroy()
    {
        GameEvents.Anvil.OnItemAddedToAnvil -= AddItem;
        GameEvents.Anvil.OnItemRemovedFromAnvil -= RemoveItem;
    }

    private void UpdateToCraftItem()
    {
        var toCraftItem = GetToCraftItem();

        if (_lastToCraftItem == toCraftItem)
            return;

        _lastToCraftItem = toCraftItem;
        if (toCraftItem != null)
        {
            _toCraftItemUI.SetItem(new InventoryItem(toCraftItem, QualityType.Good));
            _toCraftItemUI.gameObject.SetActive(true);
        }
        else
        {
            _toCraftItemUI.gameObject.SetActive(false);
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
    }
}
