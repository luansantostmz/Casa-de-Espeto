using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Recipes : MonoBehaviour
{
    [SerializeField] ItemDisplay _itemPrefab;
    [SerializeField] RectTransform _recipesContainer;
    [SerializeField] List<ItemSettings> _items = new List<ItemSettings>();

    [SerializeField] TMP_Text _recipeTitleText;
    [SerializeField] UIRecipeIngredient _ingredientPrefab;
    [SerializeField] RectTransform _ingredientsContainer;

    public List<ItemSettings> Items => _items;

    List<ItemDisplay> _uiItems = new List<ItemDisplay>();
    List<UIRecipeIngredient> _uiIngredients = new List<UIRecipeIngredient>();

    private void Awake()
    {
        GameEvents.Anvil.OnRecipeItemClicked += ShowRecipe;
    }

    private void OnDestroy()
    {
        GameEvents.Anvil.OnRecipeItemClicked -= ShowRecipe;
    }

    private void Start()
    {
        foreach (var item in _items)
        {
            var newItemUI = Instantiate(_itemPrefab, _recipesContainer);
            newItemUI.UpdateVisual(item, QualityProvider.Instance.GetFirstQuality());
            _uiItems.Add(newItemUI);
        }

        GameEvents.Anvil.OnRecipeItemClicked?.Invoke(_uiItems[0].Item);
    }

    private void ShowRecipe(ItemSettings itemSettings)
    {
        Dictionary<ItemSettings, int> ingredients = new Dictionary<ItemSettings, int>();

        foreach (var item in itemSettings.Ingredients)
        {
            if (ingredients.ContainsKey(item))
            {
                ingredients[item]++;
            }
            else
            {
                ingredients.Add(item, 1);
            }
        }

        foreach(var ingredient in _uiIngredients)
        {
            Destroy(ingredient.gameObject);
        }

        _uiIngredients.Clear();

        _recipeTitleText.text = itemSettings.ItemName;
        foreach (var item in ingredients)
        {
            var uiIngredient = Instantiate(_ingredientPrefab, _ingredientsContainer);
            uiIngredient.Setup(item.Key, item.Value);
            _uiIngredients.Add(uiIngredient);
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
}
