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

    public void Setup(ItemSettings item, bool enableBackground)
    {
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
