using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class UIItem : MonoBehaviour
{
    public ItemSettings Item;
    public QualitySettings Quality;
    public int Quantity;

    [Header("UI")]
    [SerializeField] bool _hideBgOnDrag;
    [SerializeField] TMP_Text _nameText;
    [SerializeField] Image _imageImage;
    [SerializeField] TMP_Text _qualityText;
    [SerializeField] TMP_Text _amountText;
    [SerializeField] GameObject _background;

    public ScaleDoTween TweenScale;
    UIDragHandler _dragHandler;
    ItemContainer CurrentItemContainer => _dragHandler.CurrentDropHandler.ItemContainer;

    private void Awake()
    {
        TweenScale = GetComponent<ScaleDoTween>();
        _dragHandler = GetComponent<UIDragHandler>();

        _dragHandler.OnDragStart += HideBackground;
        _dragHandler.OnDragEnd += ShowBackground;
    }

    private void OnDestroy()
    {
        _dragHandler.OnDragStart -= HideBackground;
        _dragHandler.OnDragEnd -= ShowBackground;
    }

    public bool IsIdentical(UIItem other)
    {
        return Item == other.Item && Quality == other.Quality;
    }

    public void Setup(ItemSettings item, QualitySettings quality, int quantity)
    {
        Item = item;
        Quality = quality;
        Quantity = quantity;

        UpdateVisual();
    }

    public void UpdateVisual()
    {
        if (_nameText) _nameText.text = Item.ItemName;
        if (_imageImage) _imageImage.sprite = Item.Sprite;
        if (_qualityText) _qualityText.text = Quality.QualityName;
        if (_amountText) _amountText.text = Quantity > 1 ? Quantity.ToString() : "";
    }

    public void AdjustQuantity(int value)
    {
        Quantity += value;
        UpdateVisual();
    }

    public void SetQuantity(int value)
    {
        Quantity = value;
        UpdateVisual();
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
