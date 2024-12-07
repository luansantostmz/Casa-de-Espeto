using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [SerializeField] InitialSettings _initialSettings;

    private void Awake()
    {
        EconomyService.AddGold(_initialSettings.Gold);

        foreach(var item in _initialSettings.Items)
        {
            for (int i = 0; i < item.Quantity; i++)
            {
                InventoryService.AddItem(new InventoryItem(item.Item, QualityProvider.Instance.GetFirstQuality()));
            }
        }
    }
}
