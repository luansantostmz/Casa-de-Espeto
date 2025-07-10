using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
	private void Start()
	{
		LocalizationService.Init();
		AudioManager.Instance.LoadVolume();
	}
}
