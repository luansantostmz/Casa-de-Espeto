using UnityEngine;

public class SleepButton : BaseButton
{
    protected override void OnClick()
    {
        base.OnClick();

        DayManager.Instance.Sleep();
    }
}
