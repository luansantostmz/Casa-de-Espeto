using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Recipes : MonoBehaviour
{
    [SerializeField] UIRecipeItem _itemPrefab;
    [SerializeField] RectTransform _recipesContainer;
    [SerializeField] List<ItemSettings> _items = new List<ItemSettings>();

    public List<ItemSettings> Items => _items;

    private void Start()
    {
        for (int i = 0; i < _items.Count; i++)
        {
            var item = Items[i];
            var newItemUI = Instantiate(_itemPrefab, _recipesContainer);
            newItemUI.Setup(item, i % 2 == 0);
        }
    }

    public ItemSettings GetToCraftItem(List<UIItem> currentIngredients)
    {
        List<ItemSettings> items = new List<ItemSettings>();

        foreach (var item in currentIngredients)
        {
            for (int i = 0; i < item.Quantity; i++)
            {
                items.Add(item.Item);
            }
        }

        foreach (var toCraftItem in Items)
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

        // Cria dicion�rios para contar as ocorr�ncias de cada item
        Dictionary<ItemSettings, int> firstListCounts = GetItemCounts(firstList);
        Dictionary<ItemSettings, int> secondListCounts = GetItemCounts(secondList);

        // Compara os dicion�rios
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
}
