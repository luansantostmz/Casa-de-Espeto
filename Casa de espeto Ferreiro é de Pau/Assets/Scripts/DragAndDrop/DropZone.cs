using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropZone : MonoBehaviour
{
	public bool IsBlocked;
    public GameObject DropZoneOwner;
	public RectTransform Container;

    public int Capacity;

    public List<DragAndDropObject> Items = new List<DragAndDropObject>();

    public Action<CardItem> OnDroppedHere;
    public Action<CardItem> OnMovedOut;

    private void Awake()
    {
        gameObject.SetActive(false);

        GameEvents.DragAndDrop.OnAnyDragStart += OnAnyDragStart;
        GameEvents.DragAndDrop.OnAnyDragEnd += OnAnyDragEnd;
    }

    private void OnDestroy()
    {
        GameEvents.DragAndDrop.OnAnyDragStart -= OnAnyDragStart;
        GameEvents.DragAndDrop.OnAnyDragEnd -= OnAnyDragEnd;
    }

    private void OnAnyDragStart(DragAndDropObject dragObject)
    {
        if (IsBlocked)
            return;

        gameObject.SetActive(dragObject.CurrentDropZone != this);
    }

    private void OnAnyDragEnd(DragAndDropObject dragObject, DropZone dropZone)
    {
        GameManager.Instance.StartCoroutine(DeactivateAfter(.1f));
    }

    IEnumerator DeactivateAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        gameObject.SetActive(false);
    }

    public void OnDropHere(CardItem cardItem)
	{
        OnDroppedHere?.Invoke(cardItem);
    }

    public void OnRemove(CardItem cardItem)
    {
        OnMovedOut?.Invoke(cardItem);
    }
}

