using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIStoreItem : MonoBehaviour
{
    [SerializeField] TMP_Text _name;
    [SerializeField] TMP_Text _price;
    [SerializeField] Image _sprite;
    [SerializeField] Button _purchaseButton;

    [SerializeField] ItemSettings _item;

    private void Awake()
    {
        GameEvents.Economy.OnGoldAdded += UpdateButton;
        GameEvents.Economy.OnGoldSubtracted += UpdateButton;

        _purchaseButton.onClick.AddListener(PurchaseItem);
    }

    private void OnDestroy()
    {
        GameEvents.Economy.OnGoldAdded -= UpdateButton;
        GameEvents.Economy.OnGoldSubtracted -= UpdateButton;

        _purchaseButton.onClick.RemoveListener(PurchaseItem);
    }

    public void Initialize(ItemSettings item)
    {
        _item = item;

        _name.text = _item.ItemName;
        _price.text = _item.BasePrice.ToString();
        _sprite.sprite = _item.Sprite;
    }

    public void UpdateButton()
    {
        _purchaseButton.interactable = EconomyService.HaveEnoughGold(_item.BasePrice);
    }

    private void PurchaseItem()
    {
        EconomyService.SubtractGold(_item.BasePrice);
        InventoryService.AddItem(new InventoryItem(_item, QualityProvider.Instance.GetFirstQuality()));
        GetComponent<ScaleDoTween>().PlayTween();
    }
}
