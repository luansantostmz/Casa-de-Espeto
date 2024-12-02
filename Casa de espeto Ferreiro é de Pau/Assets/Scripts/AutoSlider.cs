using UnityEngine;
using UnityEngine.UI;

public class PingPongSlider : MonoBehaviour
{
	public Slider slider;          // Referência ao componente Slider
	public float speed = 0.5f;     // Velocidade inicial do movimento
	public float acceleration = 0.1f; // Quanto a velocidade aumenta com o tempo
	public float maxSpeed = 2f;    // Velocidade máxima permitida

	private float timeElapsed;

	void Start()
	{
		if (slider == null)
		{
			Debug.LogError("O Slider não foi atribuído!");
			return;
		}

		// Inicia o contador de tempo
		timeElapsed = 0f;
	}

	void Update()
	{
		if (slider == null) return;

		// Incrementa o tempo com base na velocidade atual
		timeElapsed += Time.deltaTime * speed;

		// Calcula o valor do slider usando PingPong
		slider.value = Mathf.PingPong(timeElapsed, slider.maxValue);

		// Aumenta a velocidade gradualmente até o limite
		speed = Mathf.Min(speed + acceleration * Time.deltaTime, maxSpeed);
	}
}
