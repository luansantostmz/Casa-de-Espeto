using UnityEngine;
using PirateSheep.Localization;

public class GameInitializer : MonoBehaviour
{
	private void Start()
	{
		LocalizationService.Init();
		AudioManager.Instance.LoadVolume();
	}
}
