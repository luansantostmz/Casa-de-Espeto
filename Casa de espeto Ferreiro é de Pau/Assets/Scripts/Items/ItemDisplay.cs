using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDisplay : MonoBehaviour
{
    public ItemSettings Item;
    public QualitySettings Quality;
    public int Quantity;

    [Header("UI")]
    public TMP_Text _nameText;
    public Image _imageImage;
    public TMP_Text _qualityText;
    public TMP_Text _amountText;
    public GameObject _background;
    public Image _qualityBackground;
    public GameObject _qualityVFX;

    public virtual void UpdateVisual(ItemSettings item, QualitySettings quality, int quantity = 1)
    {
        Item = item;
        Quality = quality;
        Quantity = quantity;

        if (_nameText) _nameText.text = Item.ItemName;
        if (_imageImage) _imageImage.sprite = Item.Sprite;
        if (_qualityText) _qualityText.text = Quality.QualityName;
        if (_amountText) _amountText.text = Quantity > 1 ? Quantity.ToString() : "";
        if (_qualityBackground) _qualityBackground.color = Quality.Color;
        if (_qualityVFX) _qualityVFX.SetActive(Quality.IsSpecial);
    }

    public void ShowBackground()
    {
        _background.SetActive(true);
    }

    public void HideBackground()
    {
        _background.SetActive(false);
    }
}
