using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using System.Collections.Generic;

public class AnvilQTE : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] Anvil anvil;
    [SerializeField] private RectTransform pointer;
    [SerializeField] private RectTransform qteBar;
    [SerializeField] private Image[] greenAreas;
    [SerializeField] private Button actionButton;
    [SerializeField] private ParticleSystem correctFeedback;
    [SerializeField] private ParticleSystem errorFeedback;
    [SerializeField] private Animator _hammerAnimator;
    [SerializeField] private TweenObject _hammerShakeTween;

    [SerializeField] private ParticleSystem _qualityVfx;

    public QTEAnvilSettings Settings
    { get; set; }

    private float currentPointerSpeed;
    private float pointerDirection = 1f;
    private float leftLimit;
    private float rightLimit;
    private int score = 0;
    private int remainingChances;
    private bool qteActive = false;

    public int Score => score;

    void Start()
    {
        StartQTE();
    }

    void Update()
    {
        if (qteActive)
        {
            MovePointer();
        }
    }

    /// <summary>
    /// Initializes the QTE system.
    /// </summary>
    [Button("Start QTE")]
    public void StartQTE()
    {
        float barWidth = qteBar.rect.width;
        leftLimit = -barWidth / 2f + pointer.rect.width / 2f;
        rightLimit = barWidth / 2f - pointer.rect.width / 2f;

        pointer.anchoredPosition = new Vector2(leftLimit, pointer.anchoredPosition.y);
        pointerDirection = 1f;
        currentPointerSpeed = Settings.InitialPointerSpeed;
        score = 0;
        remainingChances = Settings.MaxChances;
        UpdateUI();
        qteActive = true;

        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(CheckPointerPosition);
        actionButton.interactable = true;
    }

    /// <summary>
    /// Moves the pointer horizontally and reverses direction when hitting limits.
    /// Increases speed with each bounce.
    /// </summary>
    private void MovePointer()
    {
        float newPositionX = pointer.anchoredPosition.x + pointerDirection * currentPointerSpeed * Time.deltaTime;

        if (newPositionX >= rightLimit)
        {
            newPositionX = rightLimit;
            pointerDirection = -1f;
            currentPointerSpeed += Settings.SpeedIncreasePerHit;
        }
        else if (newPositionX <= leftLimit)
        {
            newPositionX = leftLimit;
            pointerDirection = 1f;
            currentPointerSpeed += Settings.SpeedIncreasePerHit;
        }

        pointer.anchoredPosition = new Vector2(newPositionX, pointer.anchoredPosition.y);
    }

    /// <summary>
    /// Positions and dimensions the green areas randomly within the bar, ensuring no overlap.
    /// </summary>
    public void Init(QTEAnvilSettings settings)
    {
        Settings = settings;

        float barWidth = qteBar.rect.width;
        float barStartX = -barWidth / 2f;
        float barEndX = barWidth / 2f;

        List<Rect> placedAreas = new List<Rect>();

        foreach (Image area in greenAreas)
        {
            RectTransform areaRect = area.GetComponent<RectTransform>();
            bool placed = false;
            int maxAttempts = 200; // Increased attempts to give more chances
            int attempts = 0;

            // Loop to try and place the area without overlap
            while (!placed && attempts < maxAttempts)
            {
                attempts++;

                float randomWidth = Random.Range(Settings.MinAreaWidth, Settings.MaxAreaWidth);
                areaRect.sizeDelta = new Vector2(randomWidth, areaRect.sizeDelta.y);

                float minPosX = barStartX + randomWidth / 2f;
                float maxPosX = barEndX - randomWidth / 2f;
                float randomPosX = Random.Range(minPosX, maxPosX);

                Rect proposedRect = new Rect(randomPosX - randomWidth / 2f, areaRect.anchoredPosition.y, randomWidth, areaRect.rect.height);

                bool overlaps = false;
                foreach (Rect placedRect in placedAreas)
                {
                    // Add padding to overlap check
                    Rect paddedPlacedRect = new Rect(placedRect.x - Settings.AreaPadding, placedRect.y, placedRect.width + Settings.AreaPadding * 2, placedRect.height);
                    Rect paddedProposedRect = new Rect(proposedRect.x - Settings.AreaPadding, proposedRect.y, proposedRect.width + Settings.AreaPadding * 2, proposedRect.height);

                    if (paddedProposedRect.Overlaps(paddedPlacedRect))
                    {
                        overlaps = true;
                        break;
                    }
                }

                if (!overlaps)
                {
                    areaRect.anchoredPosition = new Vector2(randomPosX, areaRect.anchoredPosition.y);
                    placedAreas.Add(proposedRect);
                    placed = true;
                }
            }

            // Fallback if placement failed after maxAttempts
            if (!placed)
            {
                Debug.LogWarning($"Could not place a green area without overlap after {maxAttempts} attempts. Consider adjusting min/max area width, padding, or the number of green areas. Placing it at the beginning as fallback, it might overlap.");
                // As a last resort, place it at the beginning. This might still overlap,
                // but it prevents the game from freezing or errors.
                areaRect.anchoredPosition = new Vector2(barStartX + areaRect.rect.width / 2f, areaRect.anchoredPosition.y);
                placedAreas.Add(new Rect(areaRect.anchoredPosition.x - areaRect.rect.width / 2f, areaRect.anchoredPosition.y, areaRect.rect.width, areaRect.rect.height)); // Add to placedAreas even if it overlaps for consistency
            }
        }
    }

    /// <summary>
    /// Checks if the pointer is in a green area when the button is pressed.
    /// </summary>
    private void CheckPointerPosition()
    {
        if (!qteActive) return;

        remainingChances--;
        UpdateUI();

        bool hit = false;
        float pointerMinX = pointer.anchoredPosition.x - pointer.rect.width / 2f;
        float pointerMaxX = pointer.anchoredPosition.x + pointer.rect.width / 2f;

        foreach (Image area in greenAreas)
        {
            RectTransform areaRect = area.GetComponent<RectTransform>();
            float areaMinX = areaRect.anchoredPosition.x - areaRect.rect.width / 2f;
            float areaMaxX = areaRect.anchoredPosition.x + areaRect.rect.width / 2f;

            if (pointerMaxX > areaMinX && pointerMinX < areaMaxX)
            {
                score++;
                hit = true;
                correctFeedback.transform.position = area.transform.position;
                correctFeedback.Play();
                areaRect.anchoredPosition = new Vector2(100000, areaRect.anchoredPosition.y);
                break;
            }
        }

        if (hit)
        {
            _hammerAnimator.SetTrigger("Hammer");
            _qualityVfx.Play();
        }
        else
        {
            errorFeedback.transform.position = new Vector3(pointer.transform.position.x, qteBar.transform.position.y, 0);
            errorFeedback.Play();
            _hammerShakeTween.PlayTween();
        }

        anvil.OnHit(remainingChances <= 0);

        if (remainingChances <= 0)
        {
            EndQTESystem();
        }
    }

    /// <summary>
    /// Updates the score and chances texts in the UI.
    /// </summary>
    private void UpdateUI()
    {

    }

    /// <summary>
    /// Ends the QTE system.
    /// </summary>
    private void EndQTESystem()
    {
        qteActive = false;
        actionButton.interactable = false;
        Debug.Log($"QTE Finished! Final Score: {score}");
    }
}