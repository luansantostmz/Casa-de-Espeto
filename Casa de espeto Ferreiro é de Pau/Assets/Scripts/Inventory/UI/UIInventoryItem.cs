using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInventoryItem : MonoBehaviour
{
    [SerializeField] TMP_Text _name;
    [SerializeField] TMP_Text _amount;
    [SerializeField] Image _sprite;

    public InventoryItem Item { get; private set; }

    public void SetItem(InventoryItem inventoryItem)
    {
        Item = inventoryItem;

        _name.text = inventoryItem.Settings.ItemName;
        _amount.text = $"x{inventoryItem.Quantity}";
        _sprite.sprite = inventoryItem.Settings.Sprite;

        GetComponent<ScaleDoTween>().PlayTween();
    }
}
