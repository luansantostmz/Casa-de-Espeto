
public class EconomyService
{
    public static int CurrentGold { get; private set; } = 0;

    public static void AddGold(int quantity)
    {
        CurrentGold += quantity;
        GameEvents.Economy.OnGoldAdded?.Invoke();
    }

    public static void RemoveGold(int quantity)
    {
        CurrentGold -= quantity;
        GameEvents.Economy.OnGoldRemoved?.Invoke();
    }

    public static bool HaveEnoughGold(int necessary)
    {
        return CurrentGold >= necessary;
    }
}
