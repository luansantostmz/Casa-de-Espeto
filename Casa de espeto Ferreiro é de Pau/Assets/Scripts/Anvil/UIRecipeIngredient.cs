using System.Collections;
using System.Collections.Generic;
using PirateSheep.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIRecipeIngredient : MonoBehaviour
{
    [SerializeField] TMP_Text _name;
    [SerializeField] TMP_Text _quantity;
    [SerializeField] Image _icon;

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
        _name.text = LocalizationService.GetLocalizedText(_item.ItemName);
    }

    public void Setup(ItemSettings settings, int quantity)
    {
        _item = settings;

        _name.text = settings.ItemName;
        _quantity.text = $"{quantity}x";
        _icon.sprite = settings.Sprite;
    }
}
