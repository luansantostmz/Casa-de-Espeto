using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIItem : MonoBehaviour
{
    [SerializeField] private TMP_Text _name;
    [SerializeField] private Image _image;
    [SerializeField] private TMP_Text _quality;
    [SerializeField] private TMP_Text _amount;

    [field: SerializeField] public InventoryItem InventoryItem { get; private set; }
    public Inventory Inventory => _dragHandler.CurrentDropHandler.ItemContainer.Inventory;

    UIDragHandler _dragHandler;

    private void Awake()
    {
        _dragHandler = GetComponent<UIDragHandler>();
    }

    public void Setup(InventoryItem inventoryItem)
    {
        InventoryItem = inventoryItem;

        // Atualiza os campos visuais
        if (_name) _name.text = InventoryItem.Settings.ItemName;
        if (_image) _image.sprite = InventoryItem.Settings.Sprite;
        if (_quality) _quality.text = InventoryItem.Quality.QualityName;
        if (_amount) _amount.text = InventoryItem.Quantity.ToString();
    }

    public void UpdateText()
    {
        _amount.text = InventoryItem.Quantity.ToString();
    }

    public void AdjustQuantity(int amount)
    {
        InventoryItem.Quantity += amount;
        UpdateText();
    }

    public void SetQuantity(int amount)
    {
        InventoryItem.Quantity = amount;
        UpdateText();
    }

    public void TransferTo(ItemContainer container)
    {
        Inventory.RemoveItem(InventoryItem.Settings, InventoryItem.Quality, InventoryItem.Quantity);
        container.Inventory.AddItem(InventoryItem.Settings, InventoryItem.Quality, InventoryItem.Quantity);
    }
}
