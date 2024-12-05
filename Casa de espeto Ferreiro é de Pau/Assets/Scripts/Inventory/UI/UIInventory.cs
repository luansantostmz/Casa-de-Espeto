using System.Collections.Generic;
using UnityEngine;

public class UIInventory : MonoBehaviour
{
    [SerializeField] private UIInventoryItem _itemPrefab;
    [SerializeField] private RectTransform _itemContainer;

    [SerializeField] private List<InventoryItem> _items = new List<InventoryItem>();
    private List<UIInventoryItem> _inventoryItems = new();

    private void Awake()
    {
        RegisterEventListeners();
    }

    private void Start()
    {
        _items = InventoryService.Items;
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
        _items = InventoryService.Items;
        AddItemToUI(newItem);
    }

    private void HandleItemRemoved(InventoryItem itemToRemove)
    {
        _items = InventoryService.Items;
        // Find the exact UI item for this inventory item
        var uiItem = _inventoryItems.Find(i => i.Item == itemToRemove);

        if (uiItem != null)
        {
            // Remove the item from UI
            _inventoryItems.Remove(uiItem);
            //Destroy(uiItem.gameObject);
        }
    }

    private void AddItemToUI(InventoryItem item)
    {
        // Always create a new UI element for each inventory item
        var newItemUI = Instantiate(_itemPrefab, _itemContainer);
        newItemUI.SetItem(item);
        _inventoryItems.Add(newItemUI);
    }

    #endregion
}
