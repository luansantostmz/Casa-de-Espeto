using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
	[SerializeField] DebtsController _debtsController;
	private void Start()
	{
		AudioManager.Instance.LoadVolume();
	}
}
