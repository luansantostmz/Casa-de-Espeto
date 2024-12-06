using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIRecipeIngredient : MonoBehaviour
{
    [SerializeField] TMP_Text _name;
    [SerializeField] TMP_Text _quantity;
    [SerializeField] Image _icon;

    public void Setup(ItemSettings settings, int quantity)
    {
        _name.text = settings.ItemName;
        _quantity.text = $"{quantity}x";
        _icon.sprite = settings.Sprite;
    }
}
