using System;
using UnityEngine;
using UnityEngine.UI;

public class UIFillClock : MonoBehaviour
{
    public float CurrentTime;
    public float TotalTime;
    public Image FillImage;

    public bool InProgress { get; private set; }

    public Action OnStart;
    public Action OnComplete;

    public void StartTime(float totalTime)
    {
        TotalTime = totalTime;
        CurrentTime = 0;

        InProgress = true;
        gameObject.SetActive(true);
        OnStart?.Invoke();
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
        FillImage.fillAmount = CurrentTime / TotalTime;

        if (CurrentTime > TotalTime)
        {
            CurrentTime = TotalTime;
            OnComplete?.Invoke();
            StopTime();
        }
    }
}