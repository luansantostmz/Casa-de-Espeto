using UnityEngine;
using DG.Tweening;

public class LocalMoveDOTween : TweenObject
{
    public Vector3 initialLocalPosition;
    public Vector3 targetLocalPosition; 
    public float duration = 1f; 
    public Ease easeType = Ease.Linear; 

    public override void PlayTween()
    {
        base.PlayTween();

        transform.localPosition = initialLocalPosition;

        if (targetLocalPosition != null)
        {
            transform.DOLocalMove(targetLocalPosition, duration)
                     .SetEase(easeType)
                     .OnComplete(() => Debug.Log("Movimento conclu�do!"));
        }
        else
        {
            Debug.LogWarning("Nenhuma posi��o alvo foi definida.");
        }
    }

    public void ResetPosition()
    {
        transform.DOMove(initialLocalPosition, duration)
                 .SetEase(easeType)
                 .OnComplete(() => Debug.Log("Objeto voltou � posi��o inicial!"));
    }
}
