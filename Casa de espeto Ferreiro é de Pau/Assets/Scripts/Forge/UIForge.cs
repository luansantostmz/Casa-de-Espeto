using System.Collections.Generic;
using UnityEngine;

public class UIForge : MonoBehaviour
{
    [SerializeField] List<UIForgeSlot> _slots = new List<UIForgeSlot>();

    private void Awake()
    {
        GameEvents.Forge.OnItemAddedToForge += OnItemAddedToForge;
    }

    private void OnDestroy()
    {
        GameEvents.Forge.OnItemAddedToForge -= OnItemAddedToForge;
    }

    private void OnItemAddedToForge(ItemSettings itemSettings)
    {
        foreach (var slot in _slots)
        {
            if (slot.Item == null)
            {
                InventoryService.RemoveItem(itemSettings);
                slot.SetForgeBarSettings(itemSettings.ForgeSettings);
                slot.SetItem(itemSettings);

                return;
            }
        }
    }
}
