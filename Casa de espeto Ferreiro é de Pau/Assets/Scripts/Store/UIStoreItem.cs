using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIStoreItem : ItemDisplay
{
    public TMP_Text PriceText;
    public Button PurchaseButton;

    private void Awake()
    {
        GameEvents.Economy.OnGoldAdded += UpdateButton;
        GameEvents.Economy.OnGoldSubtracted += UpdateButton;

        PurchaseButton.onClick.AddListener(PurchaseItem);
    }

    public override void UpdateVisual(ItemSettings item, QualitySettings quality, int quantity = 1)
    {
        base.UpdateVisual(item, quality, quantity);
        PriceText.text = item.BasePrice.ToString();
    }

    private void OnDestroy()
    {
        GameEvents.Economy.OnGoldAdded -= UpdateButton;
        GameEvents.Economy.OnGoldSubtracted -= UpdateButton;

        PurchaseButton.onClick.RemoveListener(PurchaseItem);
    }

    public void UpdateButton()
    {
        PurchaseButton.interactable = EconomyService.HaveEnoughGold(Item.BasePrice);
    }

    private void PurchaseItem()
    {
        EconomyService.SubtractGold(Item.BasePrice);
        GameEvents.Inventory.OnAddItem?.Invoke(Item, Quality, 1);
        GetComponent<ScaleDoTween>().PlayTween();
    }
}
