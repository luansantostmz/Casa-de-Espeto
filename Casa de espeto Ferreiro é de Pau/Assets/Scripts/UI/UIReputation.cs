using UnityEngine;
using UnityEngine.UI;

public class UIReputation : MonoBehaviour
{
    [SerializeField] Image _fillBar;

    private void Awake()
    {
        GameEvents.Reputation.OnReputationChanged += OnReputationChange;
    }

    private void OnDestroy()
    {
        GameEvents.Reputation.OnReputationChanged -= OnReputationChange;
    }

    void OnReputationChange()
    {
        _fillBar.fillAmount = Mathf.Clamp(GameManager.Instance.GetReputationFill(), 0, 1);
    }
}
