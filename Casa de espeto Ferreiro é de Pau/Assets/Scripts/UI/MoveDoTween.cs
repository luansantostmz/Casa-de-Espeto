using UnityEngine;
using DG.Tweening; 

public class MoveDOTween : TweenObject
{
    public Transform targetPosition; // Posi��o alvo para onde o objeto vai se mover
    public float duration = 1f; // Dura��o da anima��o em segundos
    public Ease easeType = Ease.Linear; // Tipo de suaviza��o da anima��o

    private Vector3 initialPosition; // Posi��o inicial para resetar

    private void Start()
    {
        // Salva a posi��o inicial do objeto
        initialPosition = transform.position;
    }

    public override void PlayTween()
    {
        base.PlayTween();
        if (targetPosition != null)
        {
            transform.DOMove(targetPosition.position, duration)
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
        transform.DOMove(initialPosition, duration)
                 .SetEase(easeType)
                 .OnComplete(() => Debug.Log("Objeto voltou � posi��o inicial!"));
    }
}
