using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [SerializeField] InitialSettings _initialSettings;

    private void Start()
    {
        GameManager.AddReputation(_initialSettings.Reputation);
        EconomyService.AddGold(_initialSettings.Gold);
        
        foreach (var item in _initialSettings.Items)
        {
            for (int i = 0; i < item.Quantity; i++)
            {
                InventoryService.AddItem(new InventoryItem(item.Item, QualityProvider.Instance.GetFirstQuality()));
            }
        }
    }
}
