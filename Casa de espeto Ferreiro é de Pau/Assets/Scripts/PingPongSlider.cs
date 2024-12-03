using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class ValueRange
{
	public float minValue;  // Valor mínimo da faixa
	public float maxValue;  // Valor máximo da faixa
	public string label;    // Rótulo associado a essa faixa
	public Color color;     // Cor associada a essa faixa
}

public class PingPongSlider : MonoBehaviour
{
	public Slider slider;                          // Referência ao componente Slider
	public float speed = 0.5f;                     // Velocidade inicial
	public float acceleration = 0.1f;              // Aceleração
	public float maxSpeed = 2f;                    // Velocidade máxima

	public List<ValueRange> valueRanges = new List<ValueRange>();  // Lista de faixas definidas no Inspetor
	public Button stopButton;                     // Referência ao botão para parar o slider

	private float timeElapsed;
	private bool isMoving = true;                  // Flag para saber se o slider está se movendo

	private Image sliderFillImage;                  // Imagem que representa o preenchimento do slider
	private RectTransform fillRectTransform;       // Transform do preenchimento
	private Image sliderBackgroundImage;           // Imagem que representa o fundo do slider

	void Start()
	{
		if (slider == null)
		{
			Debug.LogError("O Slider não foi atribuído!");
			return;
		}

		sliderFillImage = slider.fillRect.GetComponent<Image>();  // Pega a imagem do preenchimento do slider
		fillRectTransform = slider.fillRect.GetComponent<RectTransform>();  // Pega o transform do preenchimento
		sliderBackgroundImage = slider.GetComponentInChildren<Image>();  // Pega a imagem do fundo do slider

		// Definindo o alfa do fundo e do preenchimento como 0
		if (sliderFillImage != null)
		{
			sliderFillImage.color = new Color(sliderFillImage.color.r, sliderFillImage.color.g, sliderFillImage.color.b, 0f);
		}

		if (sliderBackgroundImage != null)
		{
			sliderBackgroundImage.color = new Color(sliderBackgroundImage.color.r, sliderBackgroundImage.color.g, sliderBackgroundImage.color.b, 0f);
		}

		if (stopButton != null)
		{
			stopButton.onClick.AddListener(StopSlider);  // Adiciona o listener ao botão
		}

		timeElapsed = 0f;
		SetBackgroundColors(); // Atualiza as cores no início
	}

	void Update()
	{
		if (slider == null || !isMoving) return;

		// Incrementa o tempo com base na velocidade atual
		timeElapsed += Time.deltaTime * speed;

		// Calcula o valor do slider usando PingPong
		slider.value = Mathf.PingPong(timeElapsed, slider.maxValue);

		// Aumenta a velocidade gradualmente até o limite
		speed = Mathf.Min(speed + acceleration * Time.deltaTime, maxSpeed);

		// Verifica em qual faixa o valor atual do slider está
		CheckCurrentRange(slider.value);
	}

	void CheckCurrentRange(float currentValue)
	{
		foreach (var range in valueRanges)
		{
			if (currentValue >= range.minValue && currentValue <= range.maxValue)
			{
				break;
			}
		}
	}

	// Método para parar o movimento do slider
	public void StopSlider()
	{
		isMoving = false;  // Para o movimento
		Debug.Log($"Slider parou no valor: {slider.value}");

		// Verifica em qual faixa o valor do slider parou
		foreach (var range in valueRanges)
		{
			if (slider.value >= range.minValue && slider.value <= range.maxValue)
			{
				Debug.Log($"O slider parou na faixa: {range.label}");
				break;
			}
		}
	}

	// Método para dividir a barra de fundo em segmentos de cores conforme as faixas
	private void SetBackgroundColors()
	{
		if (sliderBackgroundImage != null)
		{
			// Se o fundo do slider for uma Image com um Material não customizado, vamos criar várias imagens para as faixas de cores
			float totalWidth = sliderBackgroundImage.rectTransform.rect.width;

			// Remove todas as faixas antigas
			foreach (Transform child in sliderBackgroundImage.transform)
			{
				Destroy(child.gameObject);
			}

			// Agora, criamos as faixas de cores
			float currentXPosition = 0;
			foreach (var range in valueRanges)
			{
				// Garantir que o alfa seja 1 (visível completamente)
				Color colorWithAlpha = range.color;
				colorWithAlpha.a = 1f;  // Garantir que o alfa seja 1 (totalmente visível)

				// Calcula a largura da faixa, proporcional ao intervalo entre minValue e maxValue
				float width = Mathf.Lerp(0, totalWidth, (range.maxValue - range.minValue) / slider.maxValue);

				GameObject segment = new GameObject("Segment", typeof(Image));
				segment.transform.SetParent(sliderBackgroundImage.transform, false);
				segment.GetComponent<Image>().color = colorWithAlpha;
				RectTransform rectTransform = segment.GetComponent<RectTransform>();
				rectTransform.anchorMin = new Vector2(currentXPosition, 0);
				rectTransform.anchorMax = new Vector2(currentXPosition + (width / totalWidth), 1);
				rectTransform.offsetMin = Vector2.zero;
				rectTransform.offsetMax = Vector2.zero;

				currentXPosition += width / totalWidth;  // Atualiza a posição para a próxima faixa
			}
		}
	}
}
