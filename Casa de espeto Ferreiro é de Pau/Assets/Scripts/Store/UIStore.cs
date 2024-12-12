using System.Collections.Generic;
using UnityEngine;

public class UIStore : MonoBehaviour
{
    [SerializeField] UIStoreItem _storeItemPrefab;
    [SerializeField] RectTransform _container;
    [SerializeField] List<ItemSettings> _itemsToSell = new List<ItemSettings>();

    List<UIStoreItem> _uiItems = new List<UIStoreItem>();

    private void Start()
    {
        InitializeStore();
    }

    void InitializeStore()
    {
        foreach (var item in _itemsToSell)
        {
            var ui = Instantiate(_storeItemPrefab, _container);
            ui.UpdateVisual(item, QualityProvider.Instance.GetFirstQuality());
            _uiItems.Add(ui);
        }
    }
} 