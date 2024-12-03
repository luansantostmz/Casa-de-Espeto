using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	private Vector3 originalPosition;
	private Transform originalParent;

	public int objectID; // ID do objeto arrast�vel


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


		// Eleva o objeto na hierarquia para evitar sobreposi��o visual
		transform.SetParent(transform.root, true);
	}

	public void OnDrag(PointerEventData eventData)
	{
		// Atualiza a posi��o do objeto com o mouse
		transform.position = Input.mousePosition;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (eventData.pointerEnter != null)
		{
			DropZone dropZone = eventData.pointerEnter.GetComponent<DropZone>();

			if (dropZone != null)
			{
				if (dropZone.zoneID == objectID) // Verifica correspond�ncia de IDs
				{
					// Soltar na zona correta
					transform.SetParent(dropZone.transform, true);
					transform.position = dropZone.transform.position; // Fixa na posi��o da zona
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
				// Soltar em algo que n�o � uma zona de drop
				ResetPosition();
			}
		}
		else
		{
			// N�o soltou sobre nada
			ResetPosition();
		}
		GetComponent<Image>().raycastTarget = true;
		GetComponent<ScaleDoTween>().PlayTween();
		OnDrop?.Invoke();
	}

	private void ResetPosition()
	{
		// Voltar � posi��o original
		transform.SetParent(originalParent, true);
		transform.position = originalPosition;
	}
}
