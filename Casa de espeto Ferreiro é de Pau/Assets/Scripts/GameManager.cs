using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Expandable] public GameplaySettings GameplaySettings;
    [SerializeField] float _currentTime;

    public int CurrentReputation;

    public UIState GameOverUI;
    public UIState GameWinUI;
    public bool IsGameEnd;

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

        DayManager.Instance.Initialize();
        GameEvents.OnGameWin += OnGameWin;
    }

    void Update()
    {
        EvaluateTime();
    }

    private void OnDestroy()
    {
        ResetProgression();
        GameEvents.OnGameWin -= OnGameWin;
    }

    private void OnGameWin()
    {
        GameWinUI.Activate();
    }

    private void EvaluateTime()
    {
        _currentTime += Time.deltaTime;
        GameEvents.Time.OnTimeChanged?.Invoke(_currentTime);
    }

    public void SetReputation(int value)
    {
        CurrentReputation = value;
        GameEvents.Reputation.OnReputationChanged?.Invoke();
    }

    public void GainReputation()
    {
        CurrentReputation = Mathf.Clamp(CurrentReputation + GameplaySettings.ReputationToAddOnDeliver, 0, GameplaySettings.MaxReputation);
        GameEvents.Reputation.OnReputationChanged?.Invoke();
    }

    public void LoseReputation()
    {
        if (IsGameEnd)
            return;

        CurrentReputation = Mathf.Clamp(CurrentReputation - GameplaySettings.ReputationToSubtractOnFail, 0, GameplaySettings.MaxReputation);
        GameEvents.Reputation.OnReputationChanged?.Invoke();

        if (CurrentReputation <= 0)
        {
            GameOver();
        }
    }

    public void AddReputation(int value)
    {
        if (IsGameEnd)
            return;

        CurrentReputation = Mathf.Clamp(CurrentReputation + value, 0, GameplaySettings.MaxReputation);
        GameEvents.Reputation.OnReputationChanged?.Invoke();
    }

    private void GameOver()
    {

        IsGameEnd = true;
        GameOverUI.Activate();
        GameEvents.OnGameOver?.Invoke();
    }

    public static void ResetProgression()
    {
        OrderService.OrderCount = 0;
        Debug.Log("Game progression has been reseted");
    }

    public float GetReputationFill()
    {
        return (float)CurrentReputation / GameplaySettings.MaxReputation;
    }
}
