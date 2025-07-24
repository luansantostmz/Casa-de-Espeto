using System;
using UnityEngine;
using UnityEngine.UI;

public class UIFillClock : MonoBehaviour
{
    public float CurrentTime { get; private set; }
    public float TotalTime { get; private set; }
    public Image FillImage;

    public bool InProgress { get; private set; }
    public bool IsRunning => InProgress;

    public float RemainingTime => Mathf.Max(0f, TotalTime - CurrentTime);

    public Action OnStart;
    public Action OnComplete;

    public void StartTime(float totalTime)
    {
        TotalTime = Mathf.Max(0.01f, totalTime); // Evita divisão por zero
        CurrentTime = 0f;
        InProgress = true;
        gameObject.SetActive(true);
        FillImage.fillAmount = 0f;
        OnStart?.Invoke();
    }

    /// <summary>
    /// Recalcula o tempo restante mantendo o progresso proporcional.
    /// </summary>
    public void RecalculateTime(float newTotalTime)
    {
        if (!InProgress) return;

        float progress = CurrentTime / TotalTime;
        TotalTime = newTotalTime;
        CurrentTime = progress * TotalTime;

        FillImage.fillAmount = Mathf.Clamp01(CurrentTime / TotalTime); // <-- IMPORTANTE
    }



    /// <summary>
    /// Recomeça com tempo novo baseado no percentual restante anterior.
    /// </summary>
    public void RestartFromRemaining(float newTotalTime)
    {
        if (!InProgress || newTotalTime <= 0f)
            return;

        float remainingPercent = RemainingTime / TotalTime;
        TotalTime = newTotalTime;
        CurrentTime = TotalTime * (1f - remainingPercent);
        FillImage.fillAmount = CurrentTime / TotalTime;
    }

    public void StopTime()
    {
        InProgress = false;
        gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (!InProgress)
            return;

        CurrentTime += Time.fixedDeltaTime;
        FillImage.fillAmount = Mathf.Clamp01(CurrentTime / TotalTime);

        if (CurrentTime >= TotalTime)
        {
            CurrentTime = TotalTime;
            InProgress = false;
            FillImage.fillAmount = 1f;
            gameObject.SetActive(false);
            OnComplete?.Invoke();
        }
    }
}
