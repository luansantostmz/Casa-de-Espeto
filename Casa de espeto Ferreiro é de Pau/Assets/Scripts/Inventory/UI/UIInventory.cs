using System.Collections.Generic;
using UnityEngine;

public class UIInventory : MonoBehaviour
{
    [SerializeField] private CardItem _itemPrefab;
    [SerializeField] private RectTransform _itemContainer;
    [SerializeField] DropZone _dropZone;

    public List<CardItem> Items = new();

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
        
    }

    private void RegisterEventListeners()
    {
        GameEvents.Inventory.OnNewItemAdded += HandleNewItemAdded;
        GameEvents.Inventory.OnItemRemoved += HandleItemRemoved;

        _dropZone.OnDroppedHere += OnDropHere;

        //GameEvents.Inventory.OnItemDestroyed += HandleItemDestroyed;

        //GameEvents.Inventory.OnCardItemAddedToInventory += HandleCardItemAdded;
        //GameEvents.Inventory.OnCardItemRemovedFromInventory += HandleCardItemRemoved;
    }

    private void UnregisterEventListeners()
    {
        GameEvents.Inventory.OnNewItemAdded -= HandleNewItemAdded;
        GameEvents.Inventory.OnItemRemoved -= HandleItemRemoved;

        _dropZone.OnDroppedHere -= OnDropHere;

        //GameEvents.Inventory.OnItemDestroyed -= HandleItemDestroyed;

        //GameEvents.Inventory.OnCardItemAddedToInventory -= HandleCardItemAdded;
        //GameEvents.Inventory.OnCardItemRemovedFromInventory -= HandleCardItemRemoved;
    }

    #endregion

    #region Inventory Updates

    private void OnDropHere(DragAndDropObject dropObject)
    {
        Items.Add(dropObject.GetComponent<CardItem>());
    }

    private void HandleNewItemAdded(ItemSettings item, QualitySettings quality)
    {
        var card = InventoryService.InstantiateCardItem(_itemPrefab, item, quality, _itemContainer);
        Items.Add(card);
    }

    //private void HandleItemAdded(CardItem newItem)
    //{
    //    //AddItemToUI(newItem);
    //}

    private void HandleItemRemoved(CardItem itemToRemove)
    {
        var uiItem = Items.Find(i => i.Item == itemToRemove);

        if (uiItem != null)
        {
            Items.Remove(uiItem);
        }
    }

    //private void HandleItemDestroyed(InventoryItem itemToRemove)
    //{
    //    var uiItem = _inventoryItems.Find(i => i.Item == itemToRemove);

    //    if (uiItem != null)
    //    {
    //        Destroy(uiItem.gameObject);
    //    }
    //    else
    //    {
    //        foreach(var ui in _inventoryItems)
    //        {
    //            if (ui.Item.Settings == itemToRemove.Settings && ui.Item.Quality == itemToRemove.Quality)
    //            {
    //                Destroy(ui.gameObject);
    //                return;
    //            }
    //        }
    //    }
    //}

    #endregion
}
