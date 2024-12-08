using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopMusic : MonoBehaviour
{
	private void OnEnable()
	{
		AudioManager.Instance.StopMusic();
	}
}
