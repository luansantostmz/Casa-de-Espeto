using UnityEngine;

public class TweenObject : MonoBehaviour
{
    public bool PlayOnEnable;

    protected virtual void OnEnable()
    {
        if (PlayOnEnable)
            PlayTween();
    }

    public virtual void PlayTween()
    {

    }
}
