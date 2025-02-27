using UnityEngine;

public class EndDayButton : BaseButton
{
    protected override void OnClick()
    {
        base.OnClick();

        DayManager.Instance.EndDay();
    }
}
