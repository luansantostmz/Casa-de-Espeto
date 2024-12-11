using UnityEngine;

public class GamePlayInitializer : MonoBehaviour
{
    [SerializeField] InitialSettings _initialSettings;

    private void Start()
    {
        GameManager.Instance.SetReputation(_initialSettings.Reputation);
        EconomyService.AddGold(_initialSettings.Gold);
        
        foreach (var item in _initialSettings.Items)
        {
            for (int i = 0; i < item.Quantity; i++)
            {
                OldInventoryService.AddItem(new InventoryItem(item.Item, QualityProvider.Instance.GetFirstQuality()));
            }
        }
    }
}
