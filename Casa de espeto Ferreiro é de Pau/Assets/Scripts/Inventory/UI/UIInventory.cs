using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private UIInventoryItem _itemPrefab;
    [SerializeField] private RectTransform _itemContainer;
    [SerializeField] private List<InventoryItem> _items = new List<InventoryItem>();

    private readonly Dictionary<ItemSettings, UIInventoryItem> _inventoryItems = new();

    private void Awake()
    {
        RegisterEventListeners();
    }

    private void Start()
    {
        InitializeInventoryUI();
        _items = InventoryService.Items;
    }

    private void OnDestroy()
    {
        UnregisterEventListeners();
    }

    #region Initialization

    private void InitializeInventoryUI()
    {
        foreach (var item in InventoryService.Items)
        {
            AddItemToUI(item);
        }
    }

    private void RegisterEventListeners()
    {
        GameEvents.Inventory.OnItemAdded += HandleItemAdded;
        GameEvents.Inventory.OnItemRemoved += HandleItemRemoved;
    }

    private void UnregisterEventListeners()
    {
        GameEvents.Inventory.OnItemAdded -= HandleItemAdded;
        GameEvents.Inventory.OnItemRemoved -= HandleItemRemoved;
    }
        
    #endregion

    #region Inventory Updates

    private void HandleItemAdded(InventoryItem newItem)
    {
        // Check if the item already exists in the inventory and is stackable
        if (_inventoryItems.TryGetValue(newItem.Settings, out var uiItem))
        {
            if (newItem.Settings.IsStackable)
            {
                // Update the existing item quantity
                uiItem.SetItem(uiItem.Item);
            }
            else
            {
                AddItemToUI(newItem);
            }
        }
        else
        {
            AddItemToUI(newItem);
        }
        _items = InventoryService.Items;
    }

    private void HandleItemRemoved(InventoryItem itemToRemove)
    {
        if (_inventoryItems.TryGetValue(itemToRemove.Settings, out var uiItem))
        {
            if (itemToRemove.Quantity <= 0)
            {
                // Remove item from inventory UI
                _inventoryItems.Remove(itemToRemove.Settings);
                Destroy(uiItem.gameObject);
            }
            else
            {
                // Update the item's quantity in the UI
                uiItem.SetItem(itemToRemove);
            }
        }
        _items = InventoryService.Items;
    }

    private void AddItemToUI(InventoryItem item)
    {
        var uiItem = Instantiate(_itemPrefab, _itemContainer);
        uiItem.SetItem(item);
        _inventoryItems[item.Settings] = uiItem; // Use ItemData as the key
    }

    #endregion
}
