using UnityEngine;
using UnityEngine.UI;

public class UIForgeSlot : MonoBehaviour
{
    [SerializeField] Image _itemImage;
    [SerializeField] ForgeBar _forgeBar;

    [SerializeField] Button _completeButton;

    public ItemSettings Item { get; private set; }

    private void Awake()
    {
        _forgeBar.StopBar();

        _completeButton.onClick.AddListener(StopBar);
        _forgeBar.OnBarStopped += OnBarStopped;
    }

    private void OnDestroy()
    {
        _completeButton.onClick.RemoveListener(StopBar);
        _forgeBar.OnBarStopped -= OnBarStopped;
    }

    public void SetItem(ItemSettings itemSettings)
    {
        Item = itemSettings;
        _forgeBar.StartBar();

        _itemImage.sprite = itemSettings.Sprite;
        _itemImage.gameObject.SetActive(true);
    }

    void OnBarStopped(QualityType qualityType)
    {
        if (Item == null)
            return;

        InventoryService.AddItem(Item.MeltedItem, 1, qualityType);
    }

    void StopBar()
    {
        _forgeBar.StopBar();
        _itemImage.gameObject.SetActive(false);
        Item = null;
    }
}
