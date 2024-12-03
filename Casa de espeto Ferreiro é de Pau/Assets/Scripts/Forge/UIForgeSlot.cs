using UnityEngine;
using UnityEngine.UI;

public class UIForgeSlot : MonoBehaviour
{
    [SerializeField] Image _itemImage;
    [SerializeField] ForgeBar _forgeBar;

    [SerializeField] Button _completeButton;
    [SerializeField] Button _cancelButton;

    public ItemSettings Item { get; private set; }

    private void Awake()
    {
        _forgeBar.StopBar();

        _completeButton.onClick.AddListener(StopBar);
        _cancelButton.onClick.AddListener(Cancel);
        _forgeBar.OnBarStopped += OnBarStopped;

        ActivateButtons(false);
    }

    private void OnDestroy()
    {
        _completeButton.onClick.RemoveListener(StopBar);
        _cancelButton.onClick.RemoveListener(Cancel);
        _forgeBar.OnBarStopped -= OnBarStopped;
    }

    public void SetItem(ItemSettings itemSettings)
    {
        Item = itemSettings;
        _forgeBar.StartBar();

        _itemImage.sprite = itemSettings.Sprite;
        _itemImage.gameObject.SetActive(true);

        ActivateButtons(true);
    }

    void OnBarStopped(QualityType qualityType)
    {
        if (Item == null)
            return;

        InventoryService.AddItem(Item.MeltedItem, 1, qualityType);
        ActivateButtons(false);
    }

    void StopBar()
    {
        _forgeBar.StopBar();
        _itemImage.gameObject.SetActive(false);
        Item = null;
    }

    void Cancel()
    {
        _forgeBar.gameObject.SetActive(false);
        _itemImage.gameObject.SetActive(false);
        Item = null;
        ActivateButtons(false);
    }

    void ActivateButtons(bool active)
    {
        _completeButton.gameObject.SetActive(active);
        _cancelButton.gameObject.SetActive(active);
    }
}
