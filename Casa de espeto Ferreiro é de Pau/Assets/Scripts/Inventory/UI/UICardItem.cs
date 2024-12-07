using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICardItem : MonoBehaviour
{
    [SerializeField] TMP_Text _name;
    [SerializeField] Image _sprite;

    [SerializeField] List<QualityObject> _qualityObjects = new List<QualityObject>();

    public InventoryItem Item { get; private set; }

    public void SetItem(InventoryItem inventoryItem)
    {
        Item = inventoryItem;
        UpdateVisual();
    }

    public void UpdateVisual()
    {
        _name.text = Item.Settings.ItemName;
        _sprite.sprite = Item.Settings.Sprite;

        _qualityObjects.ForEach(qualityObject => qualityObject.GameObject.SetActive(qualityObject.Quality == Item.Quality));

        GetComponent<ScaleDoTween>().PlayTween();
    }
}

[System.Serializable]
public class QualityObject
{
    public QualitySettings Quality;
    public GameObject GameObject;
}
