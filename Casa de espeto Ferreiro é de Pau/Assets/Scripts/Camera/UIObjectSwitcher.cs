using System;
using System.Collections;
using UnityEngine;

public class UIObjectSwitcher : MonoBehaviour
{
	public RectTransform[] images; // Array de imagens
	private float transitionDuration = 1f; // Tempo de transição
	private bool isTransitioning = false; // Para evitar múltiplas transições
	public RectTransform canvasRect; // Referência ao Canvas principal

	private void Start()
	{
		// Obtém o RectTransform do Canvas
		Canvas canvas = GetComponentInParent<Canvas>();
		if (canvas != null)
		{
			canvasRect = canvas.GetComponent<RectTransform>();
		}

		
	}

	public void MoveLeft()
	{
		if (isTransitioning) return;
		StartCoroutine(SwapObjects(Vector2.left));
	}

	public void MoveRight()
	{
		if (isTransitioning) return;
		StartCoroutine(SwapObjects(Vector2.right));
	}

	public void MoveUp()
	{
		if (isTransitioning) return;
		StartCoroutine(SwapObjects(Vector2.up));
	}

	public void MoveDown()
	{
		if (isTransitioning) return;
		StartCoroutine(SwapObjects(Vector2.down));
	}

	private IEnumerator SwapObjects(Vector2 direction)
	{
		isTransitioning = true;

		float canvasWidth = canvasRect.rect.width;
		float canvasHeight = canvasRect.rect.height;

		Vector2 offset = Vector2.zero;
		if (direction == Vector2.left) offset = new Vector2(-canvasWidth, 0);
		if (direction == Vector2.right) offset = new Vector2(canvasWidth, 0);
		if (direction == Vector2.up) offset = new Vector2(0, canvasHeight);
		if (direction == Vector2.down) offset = new Vector2(0, -canvasHeight);

		Vector2[] targetPositions = new Vector2[images.Length];
		for (int i = 0; i < images.Length; i++)
		{
			targetPositions[i] = images[i].anchoredPosition + offset;
		}

		float elapsedTime = 0f;

		while (elapsedTime < transitionDuration/2)
		{
			for (int i = 0; i < images.Length; i++)
			{
				images[i].anchoredPosition = Vector2.Lerp(images[i].anchoredPosition, targetPositions[i], elapsedTime / transitionDuration);
			}
			elapsedTime += Time.deltaTime;
			yield return null;
		}

		for (int i = 0; i < images.Length; i++)
		{
			images[i].anchoredPosition = targetPositions[i];
		}

		if (direction == Vector2.left || direction == Vector2.down)
		{
			RectTransform firstImage = images[0];
			for (int i = 0; i < images.Length - 1; i++)
			{
				images[i] = images[i + 1];
			}
			images[images.Length - 1] = firstImage;
		}
		else if (direction == Vector2.right || direction == Vector2.up)
		{
			RectTransform lastImage = images[images.Length - 1];
			for (int i = images.Length - 1; i > 0; i--)
			{
				images[i] = images[i - 1];
			}
			images[0] = lastImage;
		}

		isTransitioning = false;
	}
}

