using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
	public GameObject objectToSpawn; // O prefab a ser instanciado
	public Camera mainCamera;        // Referência à câmera principal (opcional)

	void Start()
	{
		if (mainCamera == null)
		{
			mainCamera = Camera.main; // Obtém automaticamente a câmera principal, se não configurada
		}
	}

	void Update()
	{
		if (Input.GetMouseButtonDown(0)) // Botão esquerdo do mouse
		{
			Vector3 mousePosition = Input.mousePosition;
			mousePosition.z = 10f; // Distância da câmera ao plano de projeção (ajuste conforme necessário)

			Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
			Instantiate(objectToSpawn, worldPosition, Quaternion.identity);
		}
	}
}
