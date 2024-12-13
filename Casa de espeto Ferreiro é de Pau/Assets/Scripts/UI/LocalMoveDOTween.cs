using UnityEngine;
using DG.Tweening;
using System;

public class LocalMoveDOTween : TweenObject
{
    public Vector3 initialLocalPosition;
    public Vector3 targetLocalPosition; 
    public float duration = 1f; 
    public Ease easeType = Ease.Linear;
    public bool PlayReverseOnComplete = true;

    public override void PlayTween()
    {
        base.PlayTween();

        transform.localPosition = initialLocalPosition;

        transform.DOLocalMove(targetLocalPosition, duration)
                    .SetEase(easeType);
    }

    public override void PlayReverse()
    {
        base.PlayReverse();

        transform.localPosition = targetLocalPosition;
        
        transform.DOLocalMove(initialLocalPosition, duration)
                    .SetEase(easeType)
                    .OnComplete(() => gameObject.SetActive(false));
    }

    public void ResetPosition(Action onComplete = null)
    {
        transform.DOMove(initialLocalPosition, duration)
                 .SetEase(easeType);
    }
}
