using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int MaxReputation = 100;
    public int CurrentReputation;

    public int ReputationToWinOnDeliver = 5;
    public int ReputationToLoseOnFail = 20;

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
        CurrentReputation += ReputationToWinOnDeliver;
        if (CurrentReputation > MaxReputation)
            CurrentReputation = MaxReputation;
        GameEvents.Reputation.OnReputationChanged?.Invoke();
    }

    public void LoseReputation()
    {
        if (IsGameOver)
            return;

        CurrentReputation -= ReputationToLoseOnFail;
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
        OldInventoryService.Items.Clear();
        Debug.Log("Game progression has been deleted");
    }

    public float GetReputationFill()
    {
        return (float)CurrentReputation / MaxReputation;
    }
}
