using UnityEngine;
using DG.Tweening;

public class ToggleMoveDOTween : TweenObject
{
    public Transform targetPositionA;
    public float durationToA = 1f;
    public Ease easeToA = Ease.InOutSine;

    public Transform targetPositionB;
    public float durationToB = 1f;
    public Ease easeToB = Ease.InOutSine;


    private Vector3 initialPosition;
    private bool toggleState; // false = vai pra A, true = vai pra B

    private void Start()
    {
        initialPosition = transform.position;
    }

    public override void PlayTween()
    {
        base.PlayTween();

        if (toggleState)
            PlayTweenToB();
        else
            PlayTweenToA();

        toggleState = !toggleState;
    }

    public void PlayTweenToA()
    {
        if (targetPositionA == null)
        {
            Debug.LogWarning("targetPositionA não foi definido.");
            return;
        }

        transform.DOMove(targetPositionA.position, durationToA)
                 .SetEase(easeToA)
                 .OnComplete(() => Debug.Log("Chegou em A"));
    }

    public void PlayTweenToB()
    {
        if (targetPositionB == null)
        {
            Debug.LogWarning("targetPositionB não foi definido.");
            return;
        }

        transform.DOMove(targetPositionB.position, durationToB)
                 .SetEase(easeToB)
                 .OnComplete(() => Debug.Log("Chegou em B"));
    }

    public void ResetPosition()
    {
        transform.DOMove(initialPosition, Mathf.Min(durationToA, durationToB))
                 .SetEase(Ease.InOutSine)
                 .OnComplete(() => Debug.Log("Objeto voltou à posição inicial!"));
    }
}
