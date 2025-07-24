using PirateSheep.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIRecipeItem : MonoBehaviour
{
    public Image ItemImage;
    public TMP_Text ItemName;
    public GameObject Background;
    public Transform IngredientsContainer;
    public UIRecipeIngredient IngredientPrefab;

    ItemSettings _item;

    void Start()
    {
        LocalizationService.OnLanguageChanged += OnLanguageChanged;
    }

    void OnDestroy()
    {
        LocalizationService.OnLanguageChanged -= OnLanguageChanged;
    }

    void OnLanguageChanged()
    {
        ItemName.text = LocalizationService.GetLocalizedText(_item.ItemName);
    }

    public void Setup(ItemSettings item, bool enableBackground)
    {
        _item = item;

        ItemImage.sprite = item.Sprite;
        ItemName.text = item.ItemName;
        Background.SetActive(enableBackground);

        foreach (var ingredient in item.GetIngredientsDictionary())
        {
            var ui = Instantiate(IngredientPrefab, IngredientsContainer);
            ui.Setup(ingredient.Key, ingredient.Value);
        }
    }
}
