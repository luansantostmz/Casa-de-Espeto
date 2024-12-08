using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAndDropObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
	private Vector3 originalPosition;
	private Transform originalParent;
	private Transform mainParent;

	public DropZone CurrentDropZone;

    public bool useSlotId;
	public int objectID; // ID do objeto arrastável

    public Action<DropZone> OnDrop;
	public Action OnDragStart;

	public AudioClip _onDragClip;
	public AudioClip _onDropClip;

    Image _image;

	private void Awake()
	{
		originalPosition = transform.position;
		originalParent = transform.parent;
        mainParent = transform.parent;
    }

    private void ActivateRaycastTarget(bool activate)
	{
		_image.raycastTarget = activate;
    }

	public void OnBeginDrag(PointerEventData eventData)
	{
		CursorManager.OnCursorToHandDragging?.Invoke();

		OnDragStart?.Invoke();
		GetComponent<ScaleDoTween>().PlayTween();
		originalPosition = transform.position;
		originalParent = transform.parent;

		GetComponent<Image>().raycastTarget = false;

		// Eleva o objeto na hierarquia para evitar sobreposição visual
		transform.SetParent(mainParent.root, true);

		GameEvents.DragAndDrop.OnAnyDragStart?.Invoke(this);

		if (_onDragClip)
			AudioManager.Instance.PlaySFX(_onDragClip);
	}

	public void OnDrag(PointerEventData eventData)
	{
		// Atualiza a posição do objeto com o mouse
		var newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		newPos.z = 0;
		transform.position = newPos;
    }

	public void OnEndDrag(PointerEventData eventData)
	{
        CursorManager.OnCursorToIdle?.Invoke();

        if (_onDropClip)
			AudioManager.Instance.PlaySFX(_onDropClip);
        
		if (eventData.pointerEnter != null)
		{
			DropZone dropZone = eventData.pointerEnter.GetComponent<DropZone>();

            if (dropZone != null && !dropZone.IsBlocked)
			{
				if (!useSlotId || (dropZone.zoneID == objectID && useSlotId)) // Verifica correspondência de IDs
				{
					// Soltar na zona correta
					transform.SetParent(dropZone.Container ? dropZone.Container : dropZone.transform, true);
					transform.localPosition = Vector3.zero; // Fixa na posição da zona
					dropZone.OnCorrectDrop();
					OnDrop?.Invoke(dropZone);
					GameEvents.DragAndDrop.OnAnyDragEnd?.Invoke(this, dropZone);
					CurrentDropZone = dropZone;
                }
                else
				{
					// Soltar na zona errada
					Debug.Log($"Objeto {objectID} foi solto na zona errada: {dropZone.zoneID}");
					dropZone.OnWrongDrop();
					ResetPosition();
					OnDrop?.Invoke(null);
					GameEvents.DragAndDrop.OnAnyDragEnd?.Invoke(this, null);
                }
            }
			else
			{
				// Soltar em algo que não é uma zona de drop
				ResetPosition();
				OnDrop?.Invoke(null);
				GameEvents.DragAndDrop.OnAnyDragEnd?.Invoke(this, null);
            }
        }
		else
		{
			// Não soltou sobre nada
			ResetPosition();
			GameEvents.DragAndDrop.OnAnyDragEnd?.Invoke(this, null);
        }

        GetComponent<Image>().raycastTarget = true;
		GetComponent<ScaleDoTween>().PlayTween();
	}

	private void ResetPosition()
	{
		// Voltar à posição original
		transform.SetParent(originalParent, true);
		transform.position = originalPosition;
	}

    public void OnPointerEnter(PointerEventData eventData)
    {
        CursorManager.OnCursorToHand?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CursorManager.OnCursorToIdle?.Invoke();
    }
}
