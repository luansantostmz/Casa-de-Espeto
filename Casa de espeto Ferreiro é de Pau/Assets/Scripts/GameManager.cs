using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int MaxReputation = 100;

    public static int CurrentReputation;

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

    public static void AddReputation(int reputation)
    {
        CurrentReputation += reputation;
        GameEvents.Reputation.OnReputationChanged?.Invoke();
    }

    public static void RemoveReputation(int reputation)
    {
        CurrentReputation -= reputation;
        GameEvents.Reputation.OnReputationChanged?.Invoke();
    }

    public float GetReputationFill()
    {
        return (float)CurrentReputation / MaxReputation;
    }
}
