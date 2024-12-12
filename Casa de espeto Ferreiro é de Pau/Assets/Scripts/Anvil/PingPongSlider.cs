using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PingPongSlider : MonoBehaviour
{
    public Slider slider;
    public Button stopButton;
    public QTEAnvilSettings settings;

    private float speed;
    private float acceleration;
    private float maxSpeed;
    private float timeElapsed;
    private bool isMoving;

    private Image sliderFillImage;
    private Image sliderBackgroundImage;

    // Pool de objetos para os segmentos
    private List<GameObject> segmentPool = new List<GameObject>();

    void OnEnable()
    {
        if (!slider)
        {
            Debug.LogError("Slider não atribuído!");
            return;
        }

        CacheUIComponents();
        InitializeSettings();
        SubscribeEvents();

        ResetSlider();
        SetBackgroundColors();
    }

    void OnDisable()
    {
        UnsubscribeEvents();
    }

    public void RestartBar()
    {
        isMoving = true;
        ResetSlider();
    }

    public void SetQTESettings(QTEAnvilSettings settings)
    {
        this.settings = settings;

        if (settings != null)
        {
            speed = settings.speed;
            acceleration = settings.acceleration;
            maxSpeed = settings.maxSpeed;
        }
    }

    void Update()
    {
        if (!isMoving) return;

        MoveSlider();
        CheckCurrentRange(slider.value);
    }

    private void MoveSlider()
    {
        timeElapsed += Time.deltaTime * speed;
        slider.value = Mathf.PingPong(timeElapsed, slider.maxValue);
        speed = Mathf.Min(speed + acceleration * Time.deltaTime, maxSpeed);
    }

    private void CheckCurrentRange(float currentValue)
    {
        foreach (var range in settings.QTESettings.valueRanges)
        {
            if (currentValue >= range.minValue && currentValue <= range.maxValue)
            {
                // Opcional: Implementar ações enquanto o slider se move.
                break;
            }
        }
    }

    public void StopSlider()
    {
        if (!isMoving) return;

        isMoving = false;
        Debug.Log($"Slider parou no valor: {slider.value}");

        foreach (var range in settings.QTESettings.valueRanges)
        {
            if (slider.value >= range.minValue && slider.value <= range.maxValue)
            {
                Debug.Log($"O slider parou na faixa: {range.quality}");
                GameEvents.Anvil.OnHammer?.Invoke(range.quality);
                break;
            }
        }
    }

    private void SetBackgroundColors()
    {
        if (!sliderBackgroundImage) return;

        ResetSegments(); // Reutiliza os segmentos existentes no pool.

        float totalWidth = sliderBackgroundImage.rectTransform.rect.width;
        float currentXPosition = 0;

        foreach (var range in settings.QTESettings.valueRanges)
        {
            float width = (range.maxValue - range.minValue) / slider.maxValue * totalWidth;
            CreateOrReuseSegment(range.quality.Color, currentXPosition, width, totalWidth);
            currentXPosition += width;
        }
    }

    private void CreateOrReuseSegment(Color color, float startX, float width, float totalWidth)
    {
        GameObject segment;

        if (segmentPool.Count > 0)
        {
            // Reutiliza um segmento do pool
            segment = segmentPool[0];
            segmentPool.RemoveAt(0);
            segment.SetActive(true);
        }
        else
        {
            // Cria um novo segmento se o pool estiver vazio
            segment = new GameObject("Segment", typeof(Image));
            segment.transform.SetParent(sliderBackgroundImage.transform, false);
        }

        // Configura o segmento
        var rectTransform = segment.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(startX / totalWidth, 0);
        rectTransform.anchorMax = new Vector2((startX + width) / totalWidth, 1);
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        var image = segment.GetComponent<Image>();
        image.color = new Color(color.r, color.g, color.b, 1f); // Garantir visibilidade total
    }

    private void ResetSegments()
    {
        foreach (Transform child in sliderBackgroundImage.transform)
        {
            // Adiciona os segmentos ao pool
            child.gameObject.SetActive(false);
            segmentPool.Add(child.gameObject);
        }
    }

    private void CacheUIComponents()
    {
        sliderFillImage = slider.fillRect?.GetComponent<Image>();
        sliderBackgroundImage = slider.GetComponentInChildren<Image>();

        if (sliderFillImage)
            sliderFillImage.color = new Color(sliderFillImage.color.r, sliderFillImage.color.g, sliderFillImage.color.b, 0f);

        if (sliderBackgroundImage)
            sliderBackgroundImage.color = new Color(sliderBackgroundImage.color.r, sliderBackgroundImage.color.g, sliderBackgroundImage.color.b, 0f);
    }

    private void InitializeSettings()
    {
        if (settings != null)
        {
            speed = settings.speed;
            acceleration = settings.acceleration;
            maxSpeed = settings.maxSpeed;
        }

        timeElapsed = 0f;
        isMoving = true;
    }

    private void ResetSlider()
    {
        timeElapsed = 0f;
        slider.value = 0f;
    }

    private void SubscribeEvents()
    {
        if (stopButton)
        {
            stopButton.onClick.AddListener(StopSlider);
        }
    }

    private void UnsubscribeEvents()
    {
        if (stopButton)
        {
            stopButton.onClick.RemoveListener(StopSlider);
        }
    }
}
