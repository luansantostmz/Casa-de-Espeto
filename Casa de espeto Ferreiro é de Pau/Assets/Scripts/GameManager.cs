using NaughtyAttributes;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Expandable, SerializeField] GameplaySettings GameplaySettings;

    public int CurrentReputation;

    public UIState GameOverUI;
    public bool IsGameOver;

    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            return;
        }

        Destroy(gameObject);
        return;
    }

    private void Start()
    {
        SetReputation(GameplaySettings.InitialReputation);
        EconomyService.AddGold(GameplaySettings.InitialGold);

        foreach (var item in GameplaySettings.InitialItems)
        {
            GameEvents.Inventory.OnAddItem?.Invoke(item.Item, item.Quality, item.Quantity);
        }
    }

    private void OnDestroy()
    {
        ResetProgression();
    }

    public void SetReputation(int value)
    {
        CurrentReputation = value;
        GameEvents.Reputation.OnReputationChanged?.Invoke();
    }

    public void GainReputation()
    {
        CurrentReputation = Mathf.Clamp(CurrentReputation + GameplaySettings.ReputationToWinOnDeliver, 0, GameplaySettings.MaxReputation);
        GameEvents.Reputation.OnReputationChanged?.Invoke();
    }

    public void LoseReputation()
    {
        if (IsGameOver)
            return;

        CurrentReputation = Mathf.Clamp(CurrentReputation - GameplaySettings.ReputationToLoseOnFail, 0, GameplaySettings.MaxReputation);
        GameEvents.Reputation.OnReputationChanged?.Invoke();

        if (CurrentReputation <= 0)
        {
            GameOver();
        }
    }

    private void GameOver()
    {

        IsGameOver = true;
        GameOverUI.Activate();
        GameEvents.OnGameOver?.Invoke();
    }

    public static void ResetProgression()
    {
        OrderService.OrderCount = 0;
        Debug.Log("Game progression has been deleted");
    }

    public float GetReputationFill()
    {
        return (float)CurrentReputation / GameplaySettings.MaxReputation;
    }
}
