using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Gameplay Settings", menuName = "Gameplay Settings")]
public class GameplaySettings : ScriptableObject
{
    [Header("Initial Settings")]
    public int InitialReputation;
    public int InitialGold;
    public List<InitialItemData> InitialItems = new List<InitialItemData>();

    [Header("Balancing")]
    public int MaxReputation = 100;
    public int ReputationToWinOnDeliver = 5;
    public int ReputationToLoseOnFail = 20;
}
