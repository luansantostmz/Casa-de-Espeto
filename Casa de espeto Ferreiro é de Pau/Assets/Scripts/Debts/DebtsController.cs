using System.Collections.Generic;
using UnityEngine;

public class DebtsController : MonoBehaviour
{
    [SerializeField] UIDebt _debtPrefab;
    [SerializeField] Transform _debtsContainer;

    public List<DebtSettings> PaidDebts = new List<DebtSettings>();

    public void Start()
    {
        for (int i = 0; i < GameManager.Instance.GameplaySettings.Debts.Count; i++)
        {
            var debtSettings = GameManager.Instance.GameplaySettings.Debts[i];
            var debtUI = Instantiate(_debtPrefab, _debtsContainer);
            debtUI.Init(this, debtSettings, i % 2 != 0);
        }
    }

    public void Pay(UIDebt uIDebt)
    {
        PaidDebts.Add(uIDebt.Settings);
        EconomyService.SubtractGold(uIDebt.Settings.DebtAmount);
        uIDebt.EvaluateUI();

        if (PaidDebts.Count == GameManager.Instance.GameplaySettings.Debts.Count)
        {
            GameEvents.OnGameWin?.Invoke();
        }
    }

    public bool IsPaid(DebtSettings settings)
    {
        return PaidDebts.Contains(settings);
    }
}
