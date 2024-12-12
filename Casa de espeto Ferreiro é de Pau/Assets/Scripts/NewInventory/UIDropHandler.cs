using System;
using System.Collections;
using UnityEngine;

public class UIDropHandler : MonoBehaviour
{
    public bool IsBlocked;
    public ItemContainer ItemContainer { get; private set; }

    public Action<UIDragHandler> OnDropHere;

    private void Awake()
    {
        gameObject.SetActive(false);

        GameEvents.DragAndDrop.OnAnyDragStart += OnAnyDragStart;
        GameEvents.DragAndDrop.OnAnyDragEnd += OnAnyDragEnd;

        OnDropHere += AddItem;
    }

    public void Initialize(ItemContainer itemContainer)
    {
        ItemContainer = itemContainer;
    }

    private void OnDestroy()
    {
        GameEvents.DragAndDrop.OnAnyDragStart -= OnAnyDragStart;
        GameEvents.DragAndDrop.OnAnyDragEnd -= OnAnyDragEnd;

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

