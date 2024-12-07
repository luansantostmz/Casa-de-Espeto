using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [SerializeField] List<float> _pitchValues = new List<float>();

    private void Awake()
    {
        // Implementação do padrão Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
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

    /// <summary>
    /// Ajusta o volume do SFX.
    /// </summary>
    /// <param name="volume">Valor entre 0 e 1.</param>
    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = Mathf.Clamp01(volume);
    }

    /// <summary>
    /// Ajusta o volume da música.
    /// </summary>
    /// <param name="volume">Valor entre 0 e 1.</param>
    public void SetMusicVolume(float volume)
    {
        musicSource.volume = Mathf.Clamp01(volume);
    }
}
