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
        GameEvents.Economy.OnGoldAdded += Initialize;
        GameEvents.Economy.OnGoldSubtracted += Initialize;

        _purchaseButton.onClick.AddListener(PurchaseItem);
    }

    private void OnDestroy()
    {
        GameEvents.Economy.OnGoldAdded -= Initialize;
        GameEvents.Economy.OnGoldSubtracted -= Initialize;

        _purchaseButton.onClick.RemoveListener(PurchaseItem);
    }

    private void OnEnable()
    {
        Initialize();
    }

    private void Initialize()
    {
        _purchaseButton.interactable = EconomyService.HaveEnoughGold(_item.BasePrice);

        _name.text = _item.ItemName;
        _price.text = _item.BasePrice.ToString();
        _sprite.sprite = _item.Sprite;
    }

    private void PurchaseItem()
    {
        EconomyService.SubtractGold(_item.BasePrice);
        InventoryService.AddItem(new InventoryItem(_item, QualityProvider.Instance.GetFirstQuality()));
        GetComponent<ScaleDoTween>().PlayTween();
    }
}
