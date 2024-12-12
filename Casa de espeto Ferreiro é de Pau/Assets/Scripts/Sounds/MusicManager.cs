using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
	[Header("Configurações de Música")]
	public List<AudioClip> musicTracks; // Lista de músicas a serem tocadas
	public AudioSource audioSource;    // Componente AudioSource para reprodução
	public bool isShuffle = false;     // Controle para ativar/desativar aleatoriedade

	[Header("Informações Atuais")]
	[SerializeField] private string currentTrackName;       // Nome da música atual
	[SerializeField] private string currentTrackDuration;   // Duração da música atual (em minutos)

	private Queue<AudioClip> trackQueue; // Fila de reprodução
	private System.Random random;       // Gerador para o modo aleatório

	private void Start()
	{
		random = new System.Random();
		SetupQueue();
		PlayNextTrack();
	}

	/// <summary>
	/// Configura a fila de reprodução com ou sem aleatoriedade.
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
	/// Embaralha a lista de músicas usando o algoritmo Fisher-Yates.
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
	/// Reproduz a próxima música na fila.
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

			// Chama automaticamente a próxima música ao terminar.
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
	/// Pula para a próxima música manualmente.
	/// </summary>
	[ContextMenu("Pular Música")]
	public void SkipTrack()
	{
		if (audioSource.isPlaying)
			audioSource.Stop();

		PlayNextTrack();
	}

	/// <summary>
	/// Adiciona uma nova música à lista e atualiza a fila se necessário.
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
	/// Remove uma música da lista e atualiza a fila se necessário.
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
