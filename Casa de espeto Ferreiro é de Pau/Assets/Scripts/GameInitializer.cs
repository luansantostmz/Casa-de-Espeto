using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [SerializeField] InitialSettings _initialSettings;

    private void Start()
    {
        GameManager.Instance.SetReputation(_initialSettings.Reputation);
        EconomyService.AddGold(_initialSettings.Gold);
        
        InventoryService.Items.Clear();
        foreach (var item in _initialSettings.Items)
        {
            for (int i = 0; i < item.Quantity; i++)
            {
                InventoryService.AddItem(new InventoryItem(item.Item, QualityProvider.Instance.GetFirstQuality()));
            }
        }
    }
}
