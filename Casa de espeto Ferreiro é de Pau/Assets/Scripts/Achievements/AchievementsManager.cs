using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementsManager : MonoBehaviour
{
    public AchievementSettings OnFirstCraft;
    public AchievementSettings OnFirstCopperForge;
    public AchievementSettings OnFirstIronForged;

    public Queue<AchievementSettings> AchievementsQueue = new Queue<AchievementSettings>();

    public UIAchievements UIAchievement;

    public float ShowTime = 3f;
    public float WaitForNextAchievementTime = 1f;

    public static AchievementsManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            GameEvents.OnTryGetAchievement += TryGetAchievement;
            return;
        }

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        GameEvents.OnTryGetAchievement -= TryGetAchievement;
    }

    private void TryGetAchievement(AchievementSettings achievement)
    {
        if (PlayerPrefs.GetInt(achievement.Key) == 1)
            return;

        PlayerPrefs.SetInt(achievement.Key, 1);
        AchievementsQueue.Enqueue(achievement);

        if (AchievementsQueue.Count == 1) // Só inicia o processo se não estiver em execução
        {
            StartCoroutine(ProcessAchievementsQueue());
        }
    }

    private IEnumerator ProcessAchievementsQueue()
    {
        while (AchievementsQueue.Count > 0)
        {
            AchievementSettings achievement = AchievementsQueue.Peek();
            UIAchievement.Show(achievement);

            yield return new WaitForSeconds(ShowTime);

            UIAchievement.Hide();
            AchievementsQueue.Dequeue();
            yield return new WaitForSeconds(WaitForNextAchievementTime);
        }
    }
}
