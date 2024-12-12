using UnityEngine;
using DG.Tweening; 

public class MoveDOTween : TweenObject
{
    public Transform targetPosition; // Posição alvo para onde o objeto vai se mover
    public float duration = 1f; // Duração da animação em segundos
    public Ease easeType = Ease.Linear; // Tipo de suavização da animação

    private Vector3 initialPosition; // Posição inicial para resetar

    private void Start()
    {
        // Salva a posição inicial do objeto
        initialPosition = transform.position;
    }

    public override void PlayTween()
    {
        base.PlayTween();
        if (targetPosition != null)
        {
            transform.DOMove(targetPosition.position, duration)
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
        transform.DOMove(initialPosition, duration)
                 .SetEase(easeType)
                 .OnComplete(() => Debug.Log("Objeto voltou à posição inicial!"));
    }
}
