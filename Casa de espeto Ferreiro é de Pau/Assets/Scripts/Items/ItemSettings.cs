using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Analytics;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item", order = 0)]
public class ItemSettings : ScriptableObject
{
    public string ItemName;
    public bool IgnoreQualityOnAnvil;
    public int HammerCount = 3;
    public int BasePrice;
    public Sprite Sprite;
    public float ForgeTime;
    public QTEAnvilSettings AnvilSettings;
    public List<ItemSettings> Ingredients = new List<ItemSettings>();

    public Dictionary<ItemSettings, int> GetIngredientsDictionary()
    {
        Dictionary<ItemSettings, int> ingredientsList = new Dictionary<ItemSettings, int>();
        foreach (var ingredient in Ingredients)
        {
            if (ingredientsList.TryGetValue(ingredient, out int value))
            {
                ingredientsList[ingredient] += 1;
                continue;
            }

            ingredientsList[ingredient] = 1;
        }
        return ingredientsList;
    }
}