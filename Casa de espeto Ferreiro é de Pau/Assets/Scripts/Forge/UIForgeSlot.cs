using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIForgeSlot : ItemContainer
{
    [Header("Forge")]
    [SerializeField] UIFillClock _clock;
    [SerializeField] int _currentLevel;
    [SerializeField] Button _upgradeButton;
    [SerializeField] TMP_Text _upgradePriceText;
    [SerializeField] TMP_Text _currentLevelText;

    [Header("Coal")]
    [SerializeField] ItemSettings _coalItem;
    [SerializeField] float _coalModifier = 0.5f;
    [SerializeField] float _coalBoostDuration = 10f;
    [SerializeField] Image _coalBuffFill;
    [SerializeField] float _coalDisplayTreshold;
    [SerializeField] ParticleSystem _coalVfx;

    private float _currentCoalBoostTimer;
    private bool _coalBuffed;
    private float _activeCoalModifier = 1.0f;

    public UIFillClock Clock => _clock;
    ForgeController _controller;
    ItemSettings _toForgeItem;

    bool OnMaxLevel => _currentLevel >= GameManager.Instance.GameplaySettings.ForgeUpgradeSettings.Count - 1;
    ForgeUpgradeData CurrentSettings => GameManager.Instance.GameplaySettings.ForgeUpgradeSettings[_currentLevel];

    protected override void Awake()
    {
        base.Awake();
        _clock.StopTime();
        _clock.OnComplete += ForgeItem;
        _upgradeButton.onClick.AddListener(Upgrade);

        GameEvents.Economy.OnGoldChanged += RefreshButton;
        UpdateLevelText();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _clock.OnComplete -= ForgeItem;
        GameEvents.Economy.OnGoldChanged -= RefreshButton;
        _upgradeButton.onClick.RemoveListener(Upgrade);
    }

    void FixedUpdate()
    {
        _coalBuffFill.fillAmount = Mathf.Clamp01(_currentCoalBoostTimer / _coalDisplayTreshold);

        if (_coalBuffed)
        {
            _currentCoalBoostTimer -= Time.fixedDeltaTime;

            if (_currentCoalBoostTimer <= 0f)
            {
                // Certifique-se de que o modificador seja resetado antes de qualquer recalculo
                _coalBuffed = false;
                _activeCoalModifier = 1.0f;
                Debug.Log("Buff de carvão terminou.");

                if (_clock.IsRunning && _toForgeItem != null)
                {
                    float newTotalTime = _toForgeItem.ForgeTime * CurrentSettings.LevelModifier * _activeCoalModifier;
                    _clock.RecalculateTime(newTotalTime);
                    Debug.Log($"Recalculando tempo com modificador normal: {newTotalTime:F2}s");
                }
            }
        }
    }

    public void Initialize(ForgeController controller)
    {
        _controller = controller;
        RefreshButton();
        UpdateLevelText();
    }

    private void RefreshButton()
    {
        _upgradeButton.gameObject.SetActive(!OnMaxLevel);

        if (!OnMaxLevel)
        {
            _upgradePriceText.text = CurrentSettings.UpgradePrice.ToString();
            _upgradeButton.interactable = EconomyService.CurrentGold >= CurrentSettings.UpgradePrice;
        }
        else
        {
            _upgradePriceText.text = "MAX";
            _upgradeButton.interactable = false;
        }
    }

    private void UpdateLevelText()
    {
        var levelText = OnMaxLevel ? "MAX" : (_currentLevel + 1).ToString();
        _currentLevelText.text = $"Level {levelText}";
    }

    private void Upgrade()
    {
        if (EconomyService.CurrentGold < CurrentSettings.UpgradePrice)
        {
            Debug.LogWarning("Ouro insuficiente para o upgrade!");
            return;
        }

        EconomyService.SubtractGold(CurrentSettings.UpgradePrice);
        _currentLevel++;
        UpdateLevelText();
        RefreshButton();

        if (_toForgeItem != null && _clock.IsRunning)
        {
            // Recalcula o tempo da forja atual após o upgrade
            float newTotalTime = _toForgeItem.ForgeTime * CurrentSettings.LevelModifier * _activeCoalModifier;
            _clock.RecalculateTime(newTotalTime);
            Debug.Log($"Upgrade aplicado. Novo tempo total: {newTotalTime:F2}s");
        }
    }

    private void ForgeItem()
    {
        foreach (var item in Items)
        {
            Destroy(item.gameObject);
        }
        Items.Clear();

        InstantiateNewItem(_toForgeItem, QualityProvider.Instance.GetFirstQuality(), 1);
        _clock.StopTime();
        _toForgeItem = null;
    }

    private void TryStartForge()
    {
        if (_toForgeItem != null || _clock.IsRunning) return;

        var itemToForge = _controller.Recipes.GetToCraftItem(Items);

        if (itemToForge != null)
        {
            _toForgeItem = itemToForge;
            // Garante que o modificador de carvão esteja atualizado antes de iniciar a forja
            // Se o timer de boost acabou, _activeCoalModifier já deveria ser 1.0f pelo FixedUpdate
            // Mas para garantir, podemos adicionar uma pequena verificação.
            // No entanto, o mais provável é que o problema esteja em um FixedUpdate não rodando antes do TryStartForge.
            // A forma atual já está correta, pois _activeCoalModifier é atualizado em FixedUpdate.
            // O problema pode ser o _coalDisplayTreshold. Se o timer for menor que ele, mas ainda positivo,
            // _coalBuffed ainda será true.

            // Para ter certeza, podemos forçar o reset se o timer estiver <= 0
            if (_currentCoalBoostTimer <= 0f)
            {
                _coalBuffed = false;
                _activeCoalModifier = 1.0f;
            }

            float finalForgeTime = _toForgeItem.ForgeTime * CurrentSettings.LevelModifier * _activeCoalModifier;
            _clock.StartTime(finalForgeTime);
            Debug.Log($"Iniciando forja de {_toForgeItem.name} com tempo: {finalForgeTime:F2}s (Modificador Carvão: {_activeCoalModifier})");
        }
        else
        {
            _clock.StopTime();
        }
    }

    public override void AddItem(UIItem uiItem)
    {
        base.AddItem(uiItem);

        if (uiItem.Item == _coalItem)
        {
            _coalVfx.Play();

            // Resetamos o timer e ativamos o buff
            _currentCoalBoostTimer = _coalBoostDuration;
            _activeCoalModifier = _coalModifier;
            _coalBuffed = true;

            GameEvents.Forge.OnAddCoal?.Invoke();

            if (_toForgeItem != null && _clock.IsRunning)
            {
                // Se já estiver forjando, recalcula o tempo imediatamente
                float newTotalTime = _toForgeItem.ForgeTime * CurrentSettings.LevelModifier * _activeCoalModifier;
                _clock.RecalculateTime(newTotalTime);
                Debug.Log($"Buff de carvão ativado. Novo tempo total: {newTotalTime:F2}s");
            }

            RemoveItem(uiItem); // Remove o carvão do slot
            Destroy(uiItem.gameObject); // Destroi o objeto de UI do carvão
        }
        else
        {
            // Se for outro item, tenta iniciar a forja
            TryStartForge();
        }
    }

    public override void RemoveItem(UIItem item)
    {
        base.RemoveItem(item);

        if (item.Item != _coalItem)
        {
            if (_toForgeItem != null)
            {
                var stillValid = _controller.Recipes.GetToCraftItem(Items);
                if (stillValid == null || stillValid != _toForgeItem)
                {
                    _clock.StopTime();
                    _toForgeItem = null;
                }
            }
            TryStartForge();
        }
    }
}