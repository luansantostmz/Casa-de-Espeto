using System;
using System.Collections;
using UnityEngine;

public class UIDropHandler : MonoBehaviour
{
    public bool IsBlocked;
    public ItemContainer ItemContainer;
    public RectTransform Container;

    public Action<UIDragHandler> OnDropHere;

    private void Awake()
    {
        gameObject.SetActive(false);

        GameEvents.DragAndDrop.OnDragStarted += OnAnyDragStart;
        GameEvents.DragAndDrop.OnDragEnded += OnAnyDragEnd;

        OnDropHere += AddItem;
    }

    private void OnDestroy()
    {
        GameEvents.DragAndDrop.OnDragStarted -= OnAnyDragStart;
        GameEvents.DragAndDrop.OnDragEnded -= OnAnyDragEnd;

        OnDropHere -= AddItem;
    }

    private void OnAnyDragStart(UIDragHandler dragHandler)
    {
        if (IsBlocked)
            return;

        gameObject.SetActive(dragHandler.CurrentDropHandler != this);
    }

    private void OnAnyDragEnd(UIDragHandler dragHandler, UIDropHandler dropHandler)
    {
        GameManager.Instance.StartCoroutine(DeactivateAfter(.1f));
        
        //var droppedItem = dragHandler.GetComponent<UIItem>().InventoryItem;
        //var addResult = ItemContainer.Inventory.AddItem(droppedItem.Settings, droppedItem.Quality, droppedItem.Quantity);
        //dragHandler.CurrentDropHandler.ItemContainer.TryRemoveItem(droppedItem);
        //Destroy(dragHandler.gameObject);
    }

    IEnumerator DeactivateAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        gameObject.SetActive(false);
    }

    private void AddItem(UIDragHandler dragHandler)
    {
    }
}

