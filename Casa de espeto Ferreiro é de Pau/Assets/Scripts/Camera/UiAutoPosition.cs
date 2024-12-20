using UnityEngine;

public class UiAutoPosition : MonoBehaviour
{
	public Vector2Int gridSteps;

	private void Start()
	{
		MoveObjectToGridPosition();
	}

	private void MoveObjectToGridPosition()
	{
		Camera mainCamera = Camera.main;
		if (mainCamera == null)
		{
			Debug.LogError("Nenhuma câmera principal encontrada.");
			return;
		}

		Vector3 screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

		float horizontalStep = screenBounds.x * 2; // Largura total da tela
		float verticalStep = screenBounds.y * 2;   // Altura total da tela

		Vector3 screenCenter = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, mainCamera.nearClipPlane));
		transform.position = Vector3.zero;

		Vector3 newPosition = transform.position;

		newPosition.x += gridSteps.x * horizontalStep;
		newPosition.y += gridSteps.y * verticalStep;

		transform.position = newPosition;
	}
}