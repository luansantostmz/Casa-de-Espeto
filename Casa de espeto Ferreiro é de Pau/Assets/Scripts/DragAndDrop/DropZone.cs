using System.Collections;
using UnityEngine;

public class DropZone : MonoBehaviour
{
	public int zoneID; 
	public bool IsBlocked;
    public GameObject DropZoneOwner;
	public RectTransform Container;

    private void Awake()
    {
        gameObject.SetActive(false);

        GameEvents.DragAndDrop.OnDragStart += OnAnyDragStart;
        GameEvents.DragAndDrop.OnDragEnd += OnAnyDragEnd;
    }

    private void OnDestroy()
    {
        GameEvents.DragAndDrop.OnDragStart -= OnAnyDragStart;
        GameEvents.DragAndDrop.OnDragEnd -= OnAnyDragEnd;
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

    public void OnCorrectDrop()
	{

	}

	public void OnWrongDrop()
	{

	}
}

