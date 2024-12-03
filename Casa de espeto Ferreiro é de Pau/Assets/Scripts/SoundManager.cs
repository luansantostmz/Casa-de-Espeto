using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class SoundManager : MonoBehaviour
{

	[SerializeField] private AudioMixer audioMixer;

	[SerializeField] private Slider masterSlider;
	[SerializeField] private Slider musicSlider;
	[SerializeField] private Slider sfxSlider;


	private void Start()
	{
		LoadVolume();
	}


	public void SetMasterVolume(float level)
	{		

		level = masterSlider.value;
		audioMixer.SetFloat("MasterVolume", Mathf.Log10(level) * 20f);

		PlayerPrefs.SetFloat("MasterVolumeKey", level);
		PlayerPrefs.Save();
	}
	public void SetMusicVolume(float level)
	{
		level = musicSlider.value;
		audioMixer.SetFloat("MusicVolume", Mathf.Log10(level) * 20f);

		PlayerPrefs.SetFloat("MusicVolumeKey", level);
		PlayerPrefs.Save();
	}
	public void SetSoundFXVolume(float level)
	{
		level = sfxSlider.value;
		audioMixer.SetFloat("SoundFXVolume", Mathf.Log10(level) * 20f);

		PlayerPrefs.SetFloat("SoundFXVolumeKey", level);
		PlayerPrefs.Save();
	}

	public void LoadVolume() 
	{
		//Master
		float savedVolumeMaster = PlayerPrefs.GetFloat("MasterVolumeKey");
		audioMixer.SetFloat("MasterVolume", Mathf.Log10(savedVolumeMaster) * 20f);
		masterSlider.value = savedVolumeMaster;

		//Music
		float savedVolumeMusic = PlayerPrefs.GetFloat("MusicVolumeKey");
		audioMixer.SetFloat("MusicVolume", Mathf.Log10(savedVolumeMusic) * 20f);
		musicSlider.value = savedVolumeMusic;

		//SFX
		float savedVolumeSFX = PlayerPrefs.GetFloat("SoundFXVolumeKey");
		audioMixer.SetFloat("SoundFXVolume", Mathf.Log10(savedVolumeSFX) * 20f);
		sfxSlider.value = savedVolumeSFX;
	}
}
