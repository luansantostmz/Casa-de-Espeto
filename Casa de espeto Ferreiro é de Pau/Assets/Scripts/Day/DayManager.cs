using UnityEngine;

public class DayManager : MonoBehaviour
{
    public int CurrentDay { get; private set; }

    public float ElapsedTime { get; private set; }
    public bool IsDayRunning { get; private set; }

    public float DayDuration => GameManager.Instance.GameplaySettings.DayDuration; 

    public static DayManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            return;
        }

        Destroy(gameObject);
    }

    public void Initialize()
    {
        StartDay();
    }

    private void Update()
    {
        if (IsDayRunning)
        {
            ElapsedTime += Time.deltaTime;
        }
    }

    public void Sleep()
    {

    }

    public void StartDay()
    {
        CurrentDay++;
        ElapsedTime = 0f;
        IsDayRunning = true;
        GameEvents.Day.OnDayStart?.Invoke();
    }

    public void EndDay()
    {
        IsDayRunning = false;
        GameEvents.Day.OnDayEnd?.Invoke();
    }

    public void ForceEndDay()
    {
        ElapsedTime = DayDuration;
    }
}
