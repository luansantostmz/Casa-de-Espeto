using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIRecipes : MonoBehaviour
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
}
