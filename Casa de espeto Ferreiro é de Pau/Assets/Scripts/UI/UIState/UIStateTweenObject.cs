
public class UIStateTweenObject : UIStateObject
{
    public TweenObject OnActivateTween;
    public TweenObject OnDeactivateTween;

    public override void Activate(bool activate)
    {
        if (activate)
            OnActivateTween.PlayTween();
        else
            OnDeactivateTween.PlayTween();
    }
}
