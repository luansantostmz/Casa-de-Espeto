using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
	private void Start()
	{
		AudioManager.Instance.LoadVolume();
		LocalizationService.Initialize();
	}
}
