using UnityEngine;

public class StartDayButton : BaseButton
{
    protected override void OnClick()
    {
        base.OnClick();

        DayManager.Instance.StartDay();
    }
}
