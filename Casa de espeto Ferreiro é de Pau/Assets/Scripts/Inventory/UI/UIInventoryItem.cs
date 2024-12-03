using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInventoryItem : MonoBehaviour
{
    [SerializeField] TMP_Text _name;
    [SerializeField] TMP_Text _amount;
    [SerializeField] Image _sprite;

    [SerializeField] List<QualityObject> _qualityObjects = new List<QualityObject>();

    public InventoryItem Item { get; private set; }

    public void SetItem(InventoryItem inventoryItem)
    {
        Item = inventoryItem;

        _name.text = inventoryItem.Settings.ItemName;
        _amount.text = $"x{inventoryItem.Quantity}";
        _sprite.sprite = inventoryItem.Settings.Sprite;

        _qualityObjects.ForEach(qualityObject => qualityObject.GameObject.SetActive(qualityObject.Quality == Item.Quality));

        GetComponent<ScaleDoTween>().PlayTween();
    }
}

[System.Serializable]
public class QualityObject
{
    public QualityType Quality;
    public GameObject GameObject;
}
