using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
	[Header("Configura��es de M�sica")]
	public List<AudioClip> musicTracks; // Lista de m�sicas a serem tocadas
	public AudioSource audioSource;    // Componente AudioSource para reprodu��o
	public bool isShuffle = false;     // Controle para ativar/desativar aleatoriedade

	[Header("Informa��es Atuais")]
	[SerializeField] private string currentTrackName;       // Nome da m�sica atual
	[SerializeField] private string currentTrackDuration;   // Dura��o da m�sica atual (em minutos)

	private Queue<AudioClip> trackQueue; // Fila de reprodu��o
	private System.Random random;       // Gerador para o modo aleat�rio

	private void Start()
	{
		random = new System.Random();
		SetupQueue();
		PlayNextTrack();
	}

	/// <summary>
	/// Configura a fila de reprodu��o com ou sem aleatoriedade.
	/// </summary>
	private void SetupQueue()
	{
		trackQueue = new Queue<AudioClip>();

		if (isShuffle)
		{
			List<AudioClip> shuffledTracks = new List<AudioClip>(musicTracks);
			ShuffleList(shuffledTracks);
			foreach (var track in shuffledTracks)
				trackQueue.Enqueue(track);
		}
		else
		{
			foreach (var track in musicTracks)
				trackQueue.Enqueue(track);
		}
	}

	/// <summary>
	/// Embaralha a lista de m�sicas usando o algoritmo Fisher-Yates.
	/// </summary>
	private void ShuffleList(List<AudioClip> list)
	{
		for (int i = list.Count - 1; i > 0; i--)
		{
			int j = random.Next(0, i + 1);
			(list[i], list[j]) = (list[j], list[i]);
		}
	}

	/// <summary>
	/// Reproduz a pr�xima m�sica na fila.
	/// </summary>
	private void PlayNextTrack()
	{
		if (trackQueue.Count == 0) SetupQueue();

		if (trackQueue.Count > 0)
		{
			AudioClip nextTrack = trackQueue.Dequeue();

			currentTrackName = nextTrack.name;
			float durationInMinutes = nextTrack.length / 60f;
			currentTrackDuration = $"{durationInMinutes:F2} minutos";

			audioSource.clip = nextTrack;
			audioSource.Play();

			// Chama automaticamente a pr�xima m�sica ao terminar.
			audioSource.loop = false;
			audioSource.PlayScheduled(AudioSettings.dspTime + nextTrack.length);
		}
	}

	/// <summary>
	/// Alterna o modo shuffle e reinicia a fila.
	/// </summary>
	public void ToggleShuffle()
	{
		isShuffle = !isShuffle;
		SetupQueue();
		PlayNextTrack();
	}

	/// <summary>
	/// Pula para a pr�xima m�sica manualmente.
	/// </summary>
	[ContextMenu("Pular M�sica")]
	public void SkipTrack()
	{
		if (audioSource.isPlaying)
			audioSource.Stop();

		PlayNextTrack();
	}

	/// <summary>
	/// Adiciona uma nova m�sica � lista e atualiza a fila se necess�rio.
	/// </summary>
	public void AddTrack(AudioClip newTrack)
	{
		if (newTrack != null && !musicTracks.Contains(newTrack))
		{
			musicTracks.Add(newTrack);
			trackQueue.Enqueue(newTrack);
		}
	}

	/// <summary>
	/// Remove uma m�sica da lista e atualiza a fila se necess�rio.
	/// </summary>
	public void RemoveTrack(AudioClip trackToRemove)
	{
		if (musicTracks.Contains(trackToRemove))
		{
			musicTracks.Remove(trackToRemove);
			SetupQueue();
		}
	}
}
