using UnityEngine;

public class UIItem : MonoBehaviour
{
    public ItemSettings Item;
    public QualitySettings Quality;
    public int Quantity;

    [Header("UI")]
    [SerializeField] bool _hideBgOnDrag;
    
    ItemDisplay _itemDisplay;
    [HideInInspector] public ScaleDoTween TweenScale;
    UIDragHandler _dragHandler;

    ItemContainer CurrentItemContainer => _dragHandler.CurrentDropHandler.ItemContainer;

    private void Awake()
    {
        _itemDisplay = GetComponent<ItemDisplay>();
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
        _itemDisplay.UpdateVisual(Item, Quality, Quantity);
        TweenScale.PlayTween();
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
        _itemDisplay.ShowBackground();
    }

    public void HideBackground()
    {
        _itemDisplay.HideBackground();
    }
}
