using PirateSheep.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDebt : MonoBehaviour
{
    [SerializeField] TMP_Text _debtNameText;
    [SerializeField] TMP_Text _debtDescriptionText;
    [SerializeField] TMP_Text _debtAmountText;
    [SerializeField] Button _payButton;
    [SerializeField] GameObject _background;

    public DebtSettings Settings { get; private set; }
    DebtsController _controller;

    void Awake()
    {
        _payButton.onClick.AddListener(Pay);

        LocalizationService.OnLanguageChanged += EvaluateUI;
    }

    void OnDestroy()
    {
        LocalizationService.OnLanguageChanged -= EvaluateUI;
    }

    public void Init(DebtsController controller, DebtSettings settings, bool activateBackground)
    {
        _controller = controller;
        Settings = settings;
        EvaluateUI();
        _background.SetActive(activateBackground);
    }

    public void EvaluateUI()
    {
        _debtNameText.text = LocalizationService.GetLocalizedText(Settings.DebtName);
        _debtDescriptionText.text = LocalizationService.GetLocalizedText(Settings.DebtDescription);
        _debtAmountText.text = Settings.DebtAmount.ToString();

        _payButton.interactable = EconomyService.HaveEnoughGold(Settings.DebtAmount);
        _payButton.interactable = !_controller.IsPaid(Settings);
    }

    void Pay()
    {
        _controller.Pay(this);
    }
}