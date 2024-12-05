using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
	public GameObject objectToSpawn; // O prefab a ser instanciado
	public Camera mainCamera;        // Refer�ncia � c�mera principal (opcional)

	void Start()
	{
		if (mainCamera == null)
		{
			mainCamera = Camera.main; // Obt�m automaticamente a c�mera principal, se n�o configurada
		}
	}

	void Update()
	{
		if (Input.GetMouseButtonDown(0)) // Bot�o esquerdo do mouse
		{
			Vector3 mousePosition = Input.mousePosition;
			mousePosition.z = 10f; // Dist�ncia da c�mera ao plano de proje��o (ajuste conforme necess�rio)

			Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
			Instantiate(objectToSpawn, worldPosition, Quaternion.identity);
		}
	}
}
