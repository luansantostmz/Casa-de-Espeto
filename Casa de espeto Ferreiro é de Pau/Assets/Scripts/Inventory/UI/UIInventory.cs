using System.Collections.Generic;
using UnityEngine;

public class UIInventory : MonoBehaviour
{
    [SerializeField] private UICardItem _itemPrefab;
    [SerializeField] private RectTransform _itemContainer;

    [SerializeField] private List<InventoryItem> _items = new List<InventoryItem>();
    private List<UICardItem> _inventoryItems = new();

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

        GameEvents.Inventory.OnCardItemAddedToInventory += HandleCardItemAdded;
        GameEvents.Inventory.OnCardItemRemovedFromInventory += HandleCardItemRemoved;
    }

    private void UnregisterEventListeners()
    {
        GameEvents.Inventory.OnItemAdded -= HandleItemAdded;
        GameEvents.Inventory.OnItemRemoved -= HandleItemRemoved;

        GameEvents.Inventory.OnCardItemAddedToInventory -= HandleCardItemAdded;
        GameEvents.Inventory.OnCardItemRemovedFromInventory -= HandleCardItemRemoved;
    }

    #endregion

    #region Inventory Updates

    private void HandleCardItemAdded(UICardItem item)
    {
        InventoryService.AddItem(item.Item, false);

        if (_inventoryItems.Contains(item))
            return;

        _inventoryItems.Add(item);
    }

    private void HandleCardItemRemoved(UICardItem item)
    {
        InventoryService.RemoveItem(item.Item, false);
        _inventoryItems.Remove(item);
    }

    private void HandleItemAdded(InventoryItem newItem)
    {
        _items = InventoryService.Items;
        AddItemToUI(newItem);
    }

    private void HandleItemRemoved(InventoryItem itemToRemove)
    {
        _items = InventoryService.Items;

        var uiItem = _inventoryItems.Find(i => i.Item == itemToRemove);

        if (uiItem != null)
        {
            _inventoryItems.Remove(uiItem);
        }
    }

    private void AddItemToUI(InventoryItem item)
    {
        var newItemUI = Instantiate(_itemPrefab, _itemContainer);
        newItemUI.SetItem(item);
        _inventoryItems.Add(newItemUI);
    }

    #endregion
}
