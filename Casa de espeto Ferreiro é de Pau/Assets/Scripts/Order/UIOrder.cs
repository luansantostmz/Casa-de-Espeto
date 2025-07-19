using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIOrder : ItemContainer
{
    [Header("Order")]
    [SerializeField] OrderData _orderData;

    [SerializeField] OrderItemDisplay _itemPrefab;
    [SerializeField] TMP_Text _orderIdText;
    [SerializeField] TMP_Text _rewardText;
    [SerializeField] TMP_Text _remainingTimeText;
    [SerializeField] TMP_Text _earnedReputationText;
    [SerializeField] TMP_Text _earnedGoldText;
    [SerializeField] TMP_Text _lostReputationText;
    [SerializeField] Image _timeBar;
    [SerializeField] RectTransform _container;

    [SerializeField] GameObject _deliveredObject;
    [SerializeField] GameObject _failObject;

    List<OrderItemDisplay> _itemsToDeliver = new List<OrderItemDisplay>();

    protected override void Awake()
    {
        base.Awake();

        GameEvents.DragAndDrop.OnAnyDragStart += ControlDropHandler;
    }

    protected override void OnDestroy()
    {
        GameEvents.DragAndDrop.OnAnyDragStart -= ControlDropHandler;
    }

    private void FixedUpdate()
    {
        if (_orderData.OrderState != OrderState.Uncomplete)
            return;

        if (_orderData.RemainingTime > 0)
        {
            _orderData.RemainingTime -= Time.fixedDeltaTime;
            _remainingTimeText.text = $"{(int)_orderData.RemainingTime}s";
            _timeBar.fillAmount = _orderData.RemainingTime / _orderData.DeliveryTime;
        }

        if (_orderData.RemainingTime < 0)
        {
            StartCoroutine(Fail());
        }
    }

    private void ControlDropHandler(UIDragHandler dragHandler)
    {
        if (_orderData.OrderState != OrderState.Uncomplete)
            return;

        var uiItem = dragHandler.GetComponent<UIItem>();

        bool activate = false;
        foreach (var item in _itemsToDeliver)
        {
            if (item.IsCompleted)
                continue;

            if (item.Item != uiItem.Item)
                continue;

            activate = true;
            break;
        }

        DropHandler.IsBlocked = !activate;
    }

    IEnumerator Deliver()
    {
        DropHandler.IsBlocked = true;
        _earnedReputationText.text = "+" + _orderData.GetReputationBasedOnDeliveredItems();
        _earnedGoldText.text = "+" + _orderData.GetGoldBasedOnDeliveredItems();
        _deliveredObject.SetActive(true);
        _orderData.OrderState = OrderState.WaitingReward;
        GetComponent<ScaleDoTween>().PlayTween();
        _orderData.GiveRewards();
        yield return new WaitForSeconds(2f);
        _orderData.Complete();
        Destroy(gameObject);
    }

    IEnumerator Fail()
    {
        _lostReputationText.text = "-" + GameManager.Instance.GameplaySettings.ReputationToSubtractOnFail;
        _failObject.SetActive(true);
        _orderData.LoseReputation();
        yield return new WaitForSeconds(2f);
        _orderData.Fail();
        Destroy(gameObject);
    }

    public void Initialize(OrderData orderData)
    {
        _orderData = orderData;

        _orderIdText.text = $"Order #{orderData.OrderId.ToString("0000")}";
        _rewardText.text = orderData.Reward.ToString();

        foreach (var item in orderData.Items)
        {
            var ui = Instantiate(_itemPrefab, _container);
            ui.UpdateVisual(item, QualityProvider.Instance.GetFirstQuality(), 1);
            _itemsToDeliver.Add(ui);
        }
    }

    public override void AddItem(UIItem uiItem)
    {
        base.AddItem(uiItem);

        _orderData.DeliveredItems.Add(new InventoryItem(uiItem.Item, uiItem.Quality));

        bool orderCompleted = true;
        bool delivered = false;
        foreach (var item in _itemsToDeliver)
        {
            if (item.IsCompleted)
                continue;

            if (item.Item == uiItem.Item && !delivered)
            {
                item.SetComplete();
                RemoveItem(uiItem);
                Destroy(uiItem);
                delivered = true;
            }

            if (!item.IsCompleted)
                orderCompleted = false;
        }

        if (!orderCompleted)
            return;

        StartCoroutine(Deliver());
    }
}
