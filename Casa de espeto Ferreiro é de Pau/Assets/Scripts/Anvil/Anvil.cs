using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Anvil : ItemContainer
{
    [Header("Anvil Settings")]
    [SerializeField] Recipes _recipes;
    [SerializeField] GameObject _hammerVFX;
    [SerializeField] float _craftCompleteWaitTime;
    [SerializeField] GameObject _hammer;
    [SerializeField] AnvilQTE _qte;
    [SerializeField] ItemDisplay _toCraftItemUI;
    [SerializeField] Button _startHammerButton;

    ItemSettings _lastToCraftItem;

    public bool _inProgress;
    public int _points;

    protected override void Awake()
    {
        base.Awake();

        _qte.gameObject.SetActive(false);
        _startHammerButton.gameObject.SetActive(false);

        GameEvents.Anvil.OnHammer += OnHammer;

        _startHammerButton.onClick.AddListener(StartHammer);
    }

    void Start()
    {
        _toCraftItemUI.gameObject.SetActive(false);
        _hammer.gameObject.SetActive(false);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        GameEvents.Anvil.OnHammer -= OnHammer;

        _startHammerButton.onClick.RemoveListener(StartHammer);
    }

    private void StartHammer()
    {
        _qte.Init(_lastToCraftItem.AnvilSettings);

        DropHandler.IsBlocked = true;

        _inProgress = true;

        _hammer.gameObject.SetActive(true);

        _startHammerButton.gameObject.SetActive(false);
        _toCraftItemUI.gameObject.SetActive(true);
        _qte.gameObject.SetActive(true);
        _qte.StartQTE();

        foreach (var item in Items)
        {
            Destroy(item.gameObject);
        }

        Items.Clear();
    }

    public void OnHit(bool complete)
    {
        // _toCraftItemUI.GetComponent<ScaleDoTween>().PlayTween();
        _toCraftItemUI.gameObject.SetActive(true);
        StartCoroutine(UpdateItem());

        if (complete)
        {
            _qte.gameObject.SetActive(false);
            StartCoroutine(Complete());
        }
    }

    IEnumerator UpdateItem()
    {
        yield return new WaitForSeconds(.25f);
        UpdateToCraftItem();
    }

    IEnumerator Complete()
    {
        yield return new WaitForSeconds(_craftCompleteWaitTime);

        GameEvents.Inventory.OnAddItem?.Invoke(
                           _toCraftItemUI.Item,
                           _toCraftItemUI.Quality,
                           _toCraftItemUI.Quantity);

        // AchievementsManager.Instance.OnFirstCraft.TryAchieve();

        _inProgress = false;

        DropHandler.IsBlocked = false;
        UpdateToCraftItem();
        _lastToCraftItem = null;
    }

    private void OnHammer(QualitySettings quality)
    {
        DropHandler.IsBlocked = true;
        UpdateToCraftItem(true);
        _hammerVFX.gameObject.SetActive(true);
    }

    private void UpdateToCraftItem(bool forceUpdateVisual = false)
    {
        if (_inProgress)
        {
            var quality = QualityProvider.Instance.GetQualityByPoints(_qte.Score);
            _toCraftItemUI.UpdateVisual(_lastToCraftItem, quality, 1);
            _toCraftItemUI.gameObject.SetActive(true);
            return;
        }

        var toCraftItem = _recipes.GetToCraftItem(Items);

        if (_lastToCraftItem == toCraftItem && !forceUpdateVisual)
            return;

        if (toCraftItem != null)
        {
            _lastToCraftItem = toCraftItem;

            _qte.gameObject.SetActive(true);

            var quality = QualityProvider.Instance.GetQualityByPoints(0);
            _toCraftItemUI.UpdateVisual(toCraftItem, quality, 1);
            _toCraftItemUI.gameObject.SetActive(true);

            if (!_inProgress)
            {
                _startHammerButton.gameObject.SetActive(true);
                _qte.gameObject.SetActive(false);
            }
        }
        else
        {
            _toCraftItemUI.gameObject.SetActive(false);
            _hammer.gameObject.SetActive(false);
            _qte.gameObject.SetActive(false);
            _startHammerButton.gameObject.SetActive(false);
        }
    }

    public override void RemoveItem(UIItem uiItem)
    {
        base.RemoveItem(uiItem);

        DropHandler.IsBlocked = false;
        UpdateToCraftItem();
        _lastToCraftItem = null;
        _startHammerButton.gameObject.SetActive(false);
    }

    public override void AddItem(UIItem uiItem)
    {
        base.AddItem(uiItem);
        UpdateToCraftItem();
    }
}
