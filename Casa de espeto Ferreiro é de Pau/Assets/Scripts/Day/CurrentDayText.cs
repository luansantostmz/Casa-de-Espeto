using TMPro;
using UnityEngine;

public class CurrentDayText : MonoBehaviour
{
    TMP_Text _text;

    private void Awake()
    {
        _text = GetComponent<TMP_Text>();

        GameEvents.Day.OnDayStart += SetText;
    }

    private void OnDestroy()
    {
        GameEvents.Day.OnDayStart -= SetText;
    }

    void SetText()
    {
        _text.text = DayManager.Instance.CurrentDay.ToString();
    }
}
