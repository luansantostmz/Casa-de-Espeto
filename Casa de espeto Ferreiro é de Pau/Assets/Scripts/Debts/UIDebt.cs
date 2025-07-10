using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDebt : MonoBehaviour
{
    [SerializeField] TMP_Text _debtNameText;
    [SerializeField] TMP_Text _debtAmountText;
    [SerializeField] Button _payButton;

    public DebtSettings Settings { get; private set; }
    DebtsController _controller;

    void Awake()
    {
        _payButton.onClick.AddListener(Pay);
    }

    public void Init(DebtsController controller, DebtSettings settings)
    {
        _controller = controller;
        Settings = settings;
        EvaluateUI();
    }

    public void EvaluateUI()
    {
        _debtNameText.text = Settings.DebtName;
        _debtAmountText.text = Settings.DebtAmount.ToString();

        _payButton.interactable = EconomyService.HaveEnoughGold(Settings.DebtAmount);
        _payButton.interactable = !_controller.IsPaid(Settings);
    }

    void Pay()
    {
        _controller.Pay(this);
    }
}