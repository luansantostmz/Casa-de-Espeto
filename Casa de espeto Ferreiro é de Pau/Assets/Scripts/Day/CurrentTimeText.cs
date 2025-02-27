using TMPro;
using UnityEngine;

public class CurrentTimeText : MonoBehaviour
{
    public float DayDurationInSeconds => DayManager.Instance.DayDuration;
    private TMP_Text _clockText;

    private float ElapsedTime => DayManager.Instance.ElapsedTime;

    private void Awake()
    {
        _clockText = GetComponent<TMP_Text>();
    }

    void Update()
    {
        UpdateClock();
    }

    private void UpdateClock()
    {
        // Calcula o progresso do dia em relação a 24 horas (00:00 a 23:59)
        float normalizedTime = Mathf.Clamp01(ElapsedTime / DayDurationInSeconds); // Entre 0 e 1
        int totalMinutes = Mathf.FloorToInt(normalizedTime * 1440); // 1440 minutos em 24 horas

        // Converte minutos totais para horas e minutos
        int hours = totalMinutes / 60;
        int minutes = totalMinutes % 60;

        // Atualiza o texto no formato 00:00
        if (_clockText != null)
        {
            _clockText.text = $"{hours:D2}:{minutes:D2}";
        }
    }
}
