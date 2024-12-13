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
                     .OnComplete(() => Debug.Log("Movimento concluído!"));
        }
        else
        {
            Debug.LogWarning("Nenhuma posição alvo foi definida.");
        }
    }

    public void ResetPosition()
    {
        transform.DOMove(initialLocalPosition, duration)
                 .SetEase(easeType)
                 .OnComplete(() => Debug.Log("Objeto voltou à posição inicial!"));
    }
}
