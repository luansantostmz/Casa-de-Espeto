using UnityEngine;

public class EarnGoldButton : BaseButton
{
    [SerializeField] int _value;

    protected override void OnClick()
    {
        base.OnClick();
        EconomyService.AddGold(_value);
    }
}
