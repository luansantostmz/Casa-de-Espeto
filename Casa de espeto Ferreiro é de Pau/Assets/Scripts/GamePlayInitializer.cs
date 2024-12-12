using UnityEngine;

public class GamePlayInitializer : MonoBehaviour
{
    [SerializeField] InitialSettings _initialSettings;

    private void Start()
    {
        GameManager.Instance.SetReputation(_initialSettings.Reputation);
        EconomyService.AddGold(_initialSettings.Gold);
    }
}
