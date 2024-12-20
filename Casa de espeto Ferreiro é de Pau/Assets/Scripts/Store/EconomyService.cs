public class EconomyService
{
    public static int CurrentGold { get; set; } = 0;

    public static void AddGold(int quantity)
    {
        CurrentGold += quantity;
        GameEvents.Economy.OnEarnGold?.Invoke(quantity);
        GameEvents.Economy.OnGoldChanged?.Invoke();
    }

    public static void SubtractGold(int quantity)
    {
        CurrentGold -= quantity;
        GameEvents.Economy.OnGoldChanged?.Invoke();
    }

    public static bool HaveEnoughGold(int necessary)
    {
        return CurrentGold >= necessary;
    }
}
