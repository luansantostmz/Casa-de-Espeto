using UnityEngine;

public class DropZone : MonoBehaviour
{
	public int zoneID; // ID da zona de drop

	public void OnCorrectDrop()
	{
		Debug.Log($"Objeto correto foi solto na zona {zoneID}!");
	}

	public void OnWrongDrop()
	{
		Debug.Log($"Objeto incorreto solto na zona {zoneID}!");
	}
}
