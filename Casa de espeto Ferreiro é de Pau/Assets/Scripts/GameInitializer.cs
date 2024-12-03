using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [SerializeField] InitialSettings _initialSettings;

    private void Awake()
    {
        EconomyService.AddGold(_initialSettings.Gold);
        foreach(var item in _initialSettings.Items)
        {
            InventoryService.AddItem(item.Settings, item.Quantity);
        }
    }
}
