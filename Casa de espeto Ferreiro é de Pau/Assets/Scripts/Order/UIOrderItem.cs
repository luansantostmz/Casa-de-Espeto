using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIOrderItem : MonoBehaviour
{
    [SerializeField] TMP_Text _itemName;
    [SerializeField] Image _icon;
    [SerializeField] List<QualityObject> _qualityObjects = new List<QualityObject>();

    InventoryItem _item;

    public void Initialize(InventoryItem item)
    {
        _item = item;

        _itemName.text = item.Settings.ItemName;
        _icon.sprite = item.Settings.Sprite;
        _qualityObjects.ForEach(qualityObject => qualityObject.GameObject.SetActive(qualityObject.Quality == item.Quality));
    }
}
