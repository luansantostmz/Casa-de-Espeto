using UnityEngine;
using System;
using System.Collections;

public class CameraShake : MonoBehaviour
{
	public static Action TriggerShakeEvent; // Action estática para acionar o shake

	public float duration = 0.5f; // Duração do shake
	public float magnitude = 0.1f; // Intensidade do shake

	private Vector3 originalPosition;

	private void Start()
	{
		originalPosition = transform.localPosition; // Salva a posição original da câmera

		// Inscreve o método TriggerShake na Action
		TriggerShakeEvent += TriggerShake;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			CameraShake.TriggerShakeEvent?.Invoke(); // Chama o evento do shake
		}
	}
	private void OnDestroy()
	{
		// Remove a inscrição para evitar erros de referência nula
		TriggerShakeEvent -= TriggerShake;
	}

	private void TriggerShake()
	{
		StartCoroutine(Shake());
	}

	private IEnumerator Shake()
	{
		float elapsed = 0f;

		while (elapsed < duration)
		{
			float x = UnityEngine.Random.Range(-1f, 1f) * magnitude;
			float y = UnityEngine.Random.Range(-1f, 1f) * magnitude;

			transform.localPosition = originalPosition + new Vector3(x, y, 0);

			elapsed += Time.deltaTime;
			yield return null;
		}

		transform.localPosition = originalPosition;
	}
}
