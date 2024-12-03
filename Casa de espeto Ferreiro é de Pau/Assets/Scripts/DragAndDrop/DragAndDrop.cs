using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	private Vector3 originalPosition;
	private Transform originalParent;

	public int objectID; // ID do objeto arrastável


	public Action OnDrop;
	public Action OnDragStart;


	private void Awake()
	{
		originalPosition = transform.position;
		originalParent = transform.parent;
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		OnDragStart?.Invoke();
		GetComponent<ScaleDoTween>().PlayTween();
		originalPosition = transform.position;
		originalParent = transform.parent;

		GetComponent<Image>().raycastTarget = false;


		// Eleva o objeto na hierarquia para evitar sobreposição visual
		transform.SetParent(transform.root, true);
	}

	public void OnDrag(PointerEventData eventData)
	{
		// Atualiza a posição do objeto com o mouse
		transform.position = Input.mousePosition;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (eventData.pointerEnter != null)
		{
			DropZone dropZone = eventData.pointerEnter.GetComponent<DropZone>();

			if (dropZone != null)
			{
				if (dropZone.zoneID == objectID) // Verifica correspondência de IDs
				{
					// Soltar na zona correta
					transform.SetParent(dropZone.transform, true);
					transform.position = dropZone.transform.position; // Fixa na posição da zona
					dropZone.OnCorrectDrop();
				}
				else
				{
					// Soltar na zona errada
					Debug.Log($"Objeto {objectID} foi solto na zona errada: {dropZone.zoneID}");
					dropZone.OnWrongDrop();
					ResetPosition();
				}
			}
			else
			{
				// Soltar em algo que não é uma zona de drop
				ResetPosition();
			}
		}
		else
		{
			// Não soltou sobre nada
			ResetPosition();
		}
		GetComponent<Image>().raycastTarget = true;
		GetComponent<ScaleDoTween>().PlayTween();
		OnDrop?.Invoke();
	}

	private void ResetPosition()
	{
		// Voltar à posição original
		transform.SetParent(originalParent, true);
		transform.position = originalPosition;
	}
}
