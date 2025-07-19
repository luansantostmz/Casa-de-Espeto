using UnityEngine;
using UnityEditor;

public class Cheats
{
    [MenuItem("HammerForge/Add 100 gold")]
    public static void Add100Gold()
    {
        EconomyService.AddGold(100);
    }

    [MenuItem("HammerForge/Add 1000 gold")]
    public static void Add1000Gold()
    {
        EconomyService.AddGold(1000);
    }

    [MenuItem("HammerForge/Add 20 reputation")]
    public static void Add20Reputation()
    {
        GameManager.Instance.AddReputation(20);
    }
}
