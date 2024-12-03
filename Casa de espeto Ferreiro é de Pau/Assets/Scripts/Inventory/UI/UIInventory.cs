using System.Collections.Generic;
using UnityEngine;

public class UIInventory : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private UIInventoryItem _itemPrefab;
    [SerializeField] private RectTransform _itemContainer;

    private readonly Dictionary<(ItemSettings, QualityType), UIInventoryItem> _inventoryItems = new();

    private void Awake()
    {
        RegisterEventListeners();
    }

    private void Start()
    {
        InitializeInventoryUI();
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
        var key = (newItem.Settings, newItem.Quality);

        // Check if the item already exists in the inventory
        if (_inventoryItems.TryGetValue(key, out var uiItem))
        {
            if (newItem.Settings.IsStackable)
            {
                // Update the existing item's UI for stackable items
                uiItem.SetItem(newItem);
            }
            else
            {
                // Non-stackable items need a new UI entry
                AddItemToUI(newItem);
            }
        }
        else
        {
            AddItemToUI(newItem);
        }
    }

    private void HandleItemRemoved(InventoryItem itemToRemove)
    {
        var key = (itemToRemove.Settings, itemToRemove.Quality);

        if (_inventoryItems.TryGetValue(key, out var uiItem))
        {
            if (itemToRemove.Quantity <= 0)
            {
                // Remove the item UI if quantity reaches 0
                _inventoryItems.Remove(key);
                Destroy(uiItem.gameObject);
            }
            else
            {
                // Update the item's UI for reduced quantity
                uiItem.SetItem(itemToRemove);
            }
        }
    }

    private void AddItemToUI(InventoryItem item)
    {
        var key = (item.Settings, item.Quality);

        var uiItem = Instantiate(_itemPrefab, _itemContainer);
        uiItem.SetItem(item);
        _inventoryItems[key] = uiItem;
    }

    #endregion
}
