using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class UITimer : MonoBehaviour
{
    private TMP_Text _text;

    void Awake()
    {
        _text = GetComponent<TMP_Text>();
        GameEvents.Time.OnTimeChanged += OnTimeChanged;
    }

    void OnDestroy()
    {
        GameEvents.Time.OnTimeChanged -= OnTimeChanged;
    }

    private void OnTimeChanged(float currentTime)
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        _text.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}