using TMPro;
using UnityEngine;

public class UIGoldText : MonoBehaviour
{
    TMP_Text text;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
        GameEvents.Economy.OnGoldAdded += UpdateValue;
        GameEvents.Economy.OnGoldSubtracted += UpdateValue; 
    }

    private void OnEnable()
    {
        UpdateValue();
    }

    private void OnDestroy()
    {
        GameEvents.Economy.OnGoldAdded -= UpdateValue;
        GameEvents.Economy.OnGoldSubtracted -= UpdateValue;
    }

    private void UpdateValue()
    {
        text.text = EconomyService.CurrentGold.ToString();
        GetComponent<ScaleDoTween>().PlayTween();
    }
}
