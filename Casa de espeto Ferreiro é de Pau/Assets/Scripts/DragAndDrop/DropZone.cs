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
        gameObject.SetActive(true);
    }

    private void OnAnyDragEnd(DragAndDropObject dragObject, DropZone dropZone)
    {
        StartCoroutine(DeactivateAfter(.1f));
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

