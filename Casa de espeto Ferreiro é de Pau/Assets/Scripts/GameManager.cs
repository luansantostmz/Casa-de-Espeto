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
    private bool IsGameOver;

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

    public void SetReputation(int value)
    {
        CurrentReputation = ReputationToWinOnDeliver;
        GameEvents.Reputation.OnReputationChanged?.Invoke();
    }

    public void GainReputation()
    {
        CurrentReputation += ReputationToWinOnDeliver;
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
            IsGameOver = true;
            GameOverUI.Activate();
            GameEvents.OnGameOver?.Invoke();
        }
    }

    public float GetReputationFill()
    {
        return (float)CurrentReputation / MaxReputation;
    }
}
