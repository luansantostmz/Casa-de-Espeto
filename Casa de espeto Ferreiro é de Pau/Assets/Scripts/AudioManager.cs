using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [SerializeField] List<float> _pitchValues = new List<float>();

	[SerializeField] private AudioMixer audioMixer;

    public  float CacheMasterVolume {get; private set;}
    public  float CacheMusicVolume {get; private set;}
    public  float CacheSFXVolume {get; private set;}

    const string MasterVolumeKey = "MasterVolume";
    const string MusicVolumeKey = "MusicVolume";
    const string SfxVolumeKey = "SoundFXVolume";

	private void Awake()
    {
        // Implementação do padrão Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

	private void Start()
	{
		LoadVolume();
	}


	/// <summary>
	/// Reproduz um efeito sonoro (SFX) uma única vez.
	/// </summary>
	/// <param name="clip">O áudio a ser reproduzido.</param>
	public void PlaySFX(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("Tentativa de reproduzir um SFX nulo.");
            return;
        }

        sfxSource.pitch = _pitchValues[Random.Range(0, _pitchValues.Count)];
        sfxSource.PlayOneShot(clip);
    }

    /// <summary>
    /// Reproduz uma música no loop.
    /// </summary>
    /// <param name="clip">O áudio a ser reproduzido.</param>
    public void PlayMusic(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("Tentativa de reproduzir uma música nula.");
            return;
        }

        // Troca a música somente se for diferente da atual
        if (musicSource.clip != clip)
        {
            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    /// <summary>
    /// Para a música atual.
    /// </summary>
    public void StopMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }

	public void SetMasterVolume(float level)
	{
		audioMixer.SetFloat(MasterVolumeKey, Mathf.Log10(level) * 20f);

		PlayerPrefs.SetFloat(MasterVolumeKey, level);
		PlayerPrefs.Save();
	}

	public void SetMusicVolume(float level)
	{
		audioMixer.SetFloat(MusicVolumeKey, Mathf.Log10(level) * 20f);

		PlayerPrefs.SetFloat(MusicVolumeKey, level);
		PlayerPrefs.Save();
	}

	public void SetSoundFXVolume(float level)
	{
		audioMixer.SetFloat(SfxVolumeKey, Mathf.Log10(level) * 20f);

		PlayerPrefs.SetFloat(SfxVolumeKey, level);
		PlayerPrefs.Save();
	}

	public void LoadVolume()
	{
		//Master
		CacheMasterVolume = PlayerPrefs.GetFloat(MasterVolumeKey, .5f);
		audioMixer.SetFloat(MasterVolumeKey, Mathf.Log10(CacheMasterVolume) * 20f);
        GameEvents.Audio.OnMasterChanged?.Invoke(CacheMasterVolume);

		//Music
		CacheMusicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, .5f);
		audioMixer.SetFloat(MusicVolumeKey, Mathf.Log10(CacheMusicVolume) * 20f);
		GameEvents.Audio.OnMasterChanged?.Invoke(CacheMusicVolume);

		//SFX
		CacheSFXVolume = PlayerPrefs.GetFloat(SfxVolumeKey, .5f);
		audioMixer.SetFloat(SfxVolumeKey, Mathf.Log10(CacheSFXVolume) * 20f);
		GameEvents.Audio.OnMasterChanged?.Invoke(CacheSFXVolume);
	}
}
