using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIOrder : MonoBehaviour
{
    [SerializeField] OrderData _orderData;

    [SerializeField] UIItem _itemPrefab;
    [SerializeField] TMP_Text _orderIdText;
    [SerializeField] TMP_Text _rewardText;
    [SerializeField] TMP_Text _remainingTimeText;
    [SerializeField] RectTransform _container;

    [SerializeField] GameObject _deliveredObject;
    [SerializeField] GameObject _failObject;
    [SerializeField] Button _completeButton;

    List<UIItem> _itemsUI = new List<UIItem>();

    private void Awake()
    {
        GameEvents.Inventory.OnItemAdded += UpdateVisual;
        GameEvents.Inventory.OnItemRemoved += UpdateVisual;
        GameEvents.Forge.OnItemAddedToForge += UpdateVisual;
        GameEvents.Anvil.OnItemRemovedFromAnvil += UpdateVisual;
        GameEvents.Anvil.OnItemAddedToAnvil += UpdateVisual;
        _completeButton.onClick.AddListener(Complete);
    }

    private void OnDestroy()
    {
        GameEvents.Inventory.OnItemAdded -= UpdateVisual;
        GameEvents.Inventory.OnItemRemoved -= UpdateVisual;
        GameEvents.Forge.OnItemAddedToForge -= UpdateVisual;
        GameEvents.Anvil.OnItemRemovedFromAnvil -= UpdateVisual;
        GameEvents.Anvil.OnItemAddedToAnvil -= UpdateVisual;
        _completeButton.onClick.RemoveListener(Complete); 
    }

    private void FixedUpdate()
    {
        if (_orderData.IsFailed || _orderData.IsCompleted)
            return;

        if (_orderData.RemainingTime > 0)
        {
            _orderData.RemainingTime -= Time.fixedDeltaTime;
            _remainingTimeText.text = $"{(int)_orderData.RemainingTime}s";
        }

        if (_orderData.RemainingTime < 0)
        {
            StartCoroutine(Fail());
        }
    }

    IEnumerator Deliver()
    {
        _deliveredObject.SetActive(true);
        _orderData.DestroyItems();
        yield return new WaitForSeconds(2f);
        _orderData.Complete();
        Destroy(gameObject);
    }

    IEnumerator Fail()
    {
        _failObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        _orderData.Fail();
        Destroy(gameObject);
    }

    private void UpdateVisual(InventoryItem item)
    {
        bool haveAllItems = _orderData.HaveAllItems();
        _completeButton.interactable = haveAllItems;
    }

    private void UpdateVisual(UICardItem item)
    {
        bool haveAllItems = _orderData.HaveAllItems();
        _completeButton.interactable = haveAllItems;
    }

    private void Complete()
    {
        StartCoroutine(Deliver());
    }

    public void Initialize(OrderData orderData)
    {
        _orderData = orderData;

        _orderIdText.text = $"Order #{orderData.OrderId.ToString("0000")}";
        _rewardText.text = orderData.Reward.ToString();

        foreach (var item in orderData.Items)
        {
            var ui = Instantiate(_itemPrefab, _container);
            ui.Setup(item.Settings, item.Quality, item.Quantity);
            _itemsUI.Add(ui);
        }

        _completeButton.interactable = _orderData.HaveAllItems();
    }
}
