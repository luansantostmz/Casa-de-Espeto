
public class PlayTweenButton : BaseButton
{
    public TweenObject TweenObject;

    protected override void OnClick()
    {
        base.OnClick();
        TweenObject.PlayTween();
    }
}
