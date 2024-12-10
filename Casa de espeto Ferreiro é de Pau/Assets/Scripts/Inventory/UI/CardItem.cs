using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardItem : MonoBehaviour
{
    public ItemSettings Item { get; set; }
    public QualitySettings Quality { get; set; }

    [SerializeField] TMP_Text _name;
    [SerializeField] Image _sprite;

    [SerializeField] List<QualityObject> _qualityObjects = new List<QualityObject>();

    public void SetItem(ItemSettings item, QualitySettings quality)
    {
        Item = item;
        Quality = quality;
        UpdateVisual();
    }

    public void UpdateVisual()
    {
        _name.text = Item.ItemName;
        _sprite.sprite = Item.Sprite;

        _qualityObjects.ForEach(qualityObject => qualityObject.GameObject.SetActive(qualityObject.Quality == Quality));

        GetComponent<ScaleDoTween>().PlayTween();
    }
}

[System.Serializable]
public class QualityObject
{
    public QualitySettings Quality;
    public GameObject GameObject;
}
