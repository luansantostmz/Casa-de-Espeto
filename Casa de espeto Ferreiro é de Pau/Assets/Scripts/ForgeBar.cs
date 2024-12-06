using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

[System.Serializable]
public class TimeValueRange // Classe específica para este script
{
	public float minValue;  // Valor mínimo da faixa
	public float maxValue;  // Valor máximo da faixa
	public QualityType quality;    // Qualidade associado a essa faixa
	public Color color;     // Cor associada a essa faixa
}

public class ForgeBar : MonoBehaviour
{
	public Slider slider;                              // Referência ao componente Slider
	public float durationInSeconds => forgeSettings.valueRanges[^1].maxValue;// Duração total (em segundos)
	public ForgeSettings forgeSettings;

	private float timeElapsed;                        // Tempo acumulado
	private bool isRunning;                    // Controle se o slider está em execução
	private Image sliderBackgroundImage;              // Imagem que representa o fundo do slider

	public Action<QualityType> OnBarStopped;

	void OnEnable()
	{
		if (slider == null)
		{
			Debug.LogError("O Slider não foi atribuído!");
			return;
		}

		sliderBackgroundImage = slider.GetComponentInChildren<Image>();  // Pega a imagem do fundo do slider

		// Inicializa o slider
		slider.value = 0f;
		slider.minValue = 0f;
		slider.maxValue = 1f;

		timeElapsed = 0f;
		SetBackgroundColors(); // Atualiza as cores no início
	}

	void Update()
	{
		if (!isRunning || slider == null) return;

		// Incrementa o tempo acumulado
		timeElapsed += Time.deltaTime;

		// Atualiza o valor do slider conforme o tempo decorrido (mapeando de 0 a 1)
		slider.value = Mathf.Clamp01(timeElapsed / durationInSeconds);

		// Verifica se o tempo total foi atingido
		if (timeElapsed >= durationInSeconds)
		{
			StopBar(); // Para automaticamente quando o tempo acabar
		}
	}

	private void CheckCurrentRange(float currentValue)
	{
		foreach (var range in forgeSettings.valueRanges)
		{
			if (currentValue >= range.minValue && currentValue <= range.maxValue)
			{
				break;
			}
		}
	}

	public void StartBar()
	{
		isRunning = true;
		gameObject.SetActive(true);
	}

	// Método para parar o movimento do slider
	public void StopBar()
	{
        gameObject.SetActive(false);

        isRunning = false;  // Para o movimento

		// Verifica em qual faixa o valor do slider parou
		foreach (var range in forgeSettings.valueRanges)
		{
			if (timeElapsed >= range.minValue && timeElapsed <= range.maxValue)
			{
				OnBarStopped?.Invoke(range.quality);
                break;
			}
		}
	}

	public QualityType GetCurrentQuality()
	{
        foreach (var range in forgeSettings.valueRanges)
        {
            if (timeElapsed >= range.minValue && timeElapsed <= range.maxValue)
            {
				return range.quality;
            }
        }

		return QualityType.Good;
    }

	// Método para dividir a barra de fundo em segmentos de cores conforme as faixas
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
			foreach (var range in forgeSettings.valueRanges)
			{
				// Garantir que o alfa seja 1 (visível completamente)
				Color colorWithAlpha = range.color;
				colorWithAlpha.a = 1f;

				// Normaliza os valores de minValue e maxValue de acordo com a duração total
				float normalizedMin = range.minValue / durationInSeconds;  // Mapeia minValue para o intervalo de 0 a 1
				float normalizedMax = range.maxValue / durationInSeconds;  // Mapeia maxValue para o intervalo de 0 a 1

				// Calcula a posição inicial e o comprimento das faixas em termos de largura da barra
				float startX = normalizedMin * totalWidth;
				float width = (normalizedMax - normalizedMin) * totalWidth;

				// Criação do segmento da faixa
				GameObject segment = new GameObject("Segment", typeof(Image));
				segment.transform.SetParent(sliderBackgroundImage.transform, false);

				Image segmentImage = segment.GetComponent<Image>();
				segmentImage.color = colorWithAlpha;

				RectTransform rectTransform = segment.GetComponent<RectTransform>();
				rectTransform.anchorMin = new Vector2(0, 0); // Alinhar ao fundo
				rectTransform.anchorMax = new Vector2(0, 1); // Preenchimento na vertical
				rectTransform.pivot = new Vector2(0, 0.5f); // Ponto de rotação à esquerda
				rectTransform.sizeDelta = new Vector2(width, bgRect.rect.height); // Ajusta o tamanho do segmento
				rectTransform.anchoredPosition = new Vector2(startX, 0); // Define a posição inicial
			}
		}
	}
}
