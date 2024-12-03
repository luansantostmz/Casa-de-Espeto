using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class TimeValueRange // Classe espec�fica para este script
{
	public float minValue;  // Valor m�nimo da faixa
	public float maxValue;  // Valor m�ximo da faixa
	public string label;    // R�tulo associado a essa faixa
	public Color color;     // Cor associada a essa faixa
}

public class TimedSlider : MonoBehaviour
{
	public Slider slider;                              // Refer�ncia ao componente Slider
	public float durationInSeconds = 120f;            // Dura��o total (em segundos)
	public List<TimeValueRange> valueRanges = new List<TimeValueRange>();  // Lista de faixas definidas no Inspetor
	public Button stopButton;                         // Refer�ncia ao bot�o para parar o slider

	private float timeElapsed;                        // Tempo acumulado
	private bool isRunning = true;                    // Controle se o slider est� em execu��o
	private Image sliderBackgroundImage;              // Imagem que representa o fundo do slider

	void Start()
	{
		if (slider == null)
		{
			Debug.LogError("O Slider n�o foi atribu�do!");
			return;
		}

		sliderBackgroundImage = slider.GetComponentInChildren<Image>();  // Pega a imagem do fundo do slider

		// Inicializa o slider
		slider.value = 0f;
		slider.minValue = 0f;
		slider.maxValue = 1f;

		if (stopButton != null)
		{
			stopButton.onClick.AddListener(StopSlider);  // Adiciona o listener ao bot�o
		}

		timeElapsed = 0f;
		SetBackgroundColors(); // Atualiza as cores no in�cio
	}

	void Update()
	{
		if (!isRunning || slider == null) return;

		// Incrementa o tempo acumulado
		timeElapsed += Time.deltaTime;
		Debug.Log(timeElapsed);

		// Atualiza o valor do slider conforme o tempo decorrido (mapeando de 0 a 1)
		slider.value = Mathf.Clamp01(timeElapsed / durationInSeconds);

		// Verifica se o tempo total foi atingido
		if (timeElapsed >= durationInSeconds)
		{
			StopSlider(); // Para automaticamente quando o tempo acabar
		}

		// Exibe a faixa atual no console
		CheckCurrentRange(slider.value);
	}

	private void CheckCurrentRange(float currentValue)
	{
		foreach (var range in valueRanges)
		{
			if (currentValue >= range.minValue && currentValue <= range.maxValue)
			{
				Debug.Log($"O valor atual est� na faixa: {range.label}");
				break;
			}
		}
	}

	// M�todo para parar o movimento do slider
	public void StopSlider()
	{
		isRunning = false;  // Para o movimento
		Debug.Log($"Slider parou no tempo de: {timeElapsed} segundos");

		// Verifica em qual faixa o valor do slider parou
		foreach (var range in valueRanges)
		{
			if (timeElapsed >= range.minValue && timeElapsed <= range.maxValue)
			{
				Debug.Log($"O slider parou na faixa: {range.label}");
				break;
			}
		}
	}

	// M�todo para dividir a barra de fundo em segmentos de cores conforme as faixas
	private void SetBackgroundColors()
	{
		if (sliderBackgroundImage != null)
		{
			RectTransform bgRect = sliderBackgroundImage.rectTransform; // Transform do fundo do slider
			float totalWidth = bgRect.rect.width;

			// Remove todas as faixas antigas
			foreach (Transform child in sliderBackgroundImage.transform)
			{
				Destroy(child.gameObject);
			}

			// Agora, criamos as faixas de cores
			foreach (var range in valueRanges)
			{
				// Garantir que o alfa seja 1 (vis�vel completamente)
				Color colorWithAlpha = range.color;
				colorWithAlpha.a = 1f;

				// Normaliza os valores de minValue e maxValue de acordo com a dura��o total
				float normalizedMin = range.minValue / durationInSeconds;  // Mapeia minValue para o intervalo de 0 a 1
				float normalizedMax = range.maxValue / durationInSeconds;  // Mapeia maxValue para o intervalo de 0 a 1

				// Calcula a posi��o inicial e o comprimento das faixas em termos de largura da barra
				float startX = normalizedMin * totalWidth;
				float width = (normalizedMax - normalizedMin) * totalWidth;

				// Cria��o do segmento da faixa
				GameObject segment = new GameObject("Segment", typeof(Image));
				segment.transform.SetParent(sliderBackgroundImage.transform, false);

				Image segmentImage = segment.GetComponent<Image>();
				segmentImage.color = colorWithAlpha;

				RectTransform rectTransform = segment.GetComponent<RectTransform>();
				rectTransform.anchorMin = new Vector2(0, 0); // Alinhar ao fundo
				rectTransform.anchorMax = new Vector2(0, 1); // Preenchimento na vertical
				rectTransform.pivot = new Vector2(0, 0.5f); // Ponto de rota��o � esquerda
				rectTransform.sizeDelta = new Vector2(width, bgRect.rect.height); // Ajusta o tamanho do segmento
				rectTransform.anchoredPosition = new Vector2(startX, 0); // Define a posi��o inicial
			}
		}
	}
}
