using TMPro;
using UnityEngine;

public class UIGoldText : MonoBehaviour
{
    TMP_Text text;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
        GameEvents.Economy.OnGoldChanged += UpdateValue;
    }

    private void OnEnable()
    {
        UpdateValue();
    }

    private void OnDestroy()
    {
        GameEvents.Economy.OnGoldChanged -= UpdateValue;
    }

    private void UpdateValue()
    {
        text.text = EconomyService.CurrentGold.ToString();
        GetComponent<ScaleDoTween>().PlayTween();
    }
}
