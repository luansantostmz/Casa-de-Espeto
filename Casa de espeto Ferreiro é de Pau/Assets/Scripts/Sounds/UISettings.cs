using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class UISettings : MonoBehaviour
{
	[SerializeField] private Slider masterSlider;
	[SerializeField] private Slider musicSlider;
	[SerializeField] private Slider sfxSlider;



	private void OnEnable()
	{
		OnSliderVolumeSet();
	}

	#region Audio Settings

	public void OnSliderVolumeSet() 
	{
		masterSlider.value = AudioManager.Instance.CacheMasterVolume;
		musicSlider.value = AudioManager.Instance.CacheMusicVolume;
		sfxSlider.value = AudioManager.Instance.CacheSFXVolume;
	}

	public void SetMaster(float level) 
	{
		level = masterSlider.value;
		AudioManager.Instance.SetMasterVolume(level);
	}
	public void SetMusic(float level)
	{
		level = musicSlider.value;
		AudioManager.Instance.SetMusicVolume(level);
	}
	public void SetSFX(float level)
	{
		level = sfxSlider.value;
		AudioManager.Instance.SetSoundFXVolume(level);
	}
	#endregion

}
