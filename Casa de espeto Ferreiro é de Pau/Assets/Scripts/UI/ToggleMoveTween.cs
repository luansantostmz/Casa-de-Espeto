using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;

public class ToggleMoveDOTween : TweenObject
{
    public Transform positionOnStart;

    [Header("A")]
    public Transform targetPositionA;
    public Transform initialPositionA;
    public float durationToA = 1f;
    public Ease easeToA = Ease.InOutSine;

    [Header("B")]
    public Transform targetPositionB;
    public Transform initialPositionB;
    public float durationToB = 1f;
    public Ease easeToB = Ease.InOutSine;


    private Vector3 initialPosition;
    private bool toggleState; // false = vai pra A, true = vai pra B

    private void Start()
    {
        initialPosition = transform.position;
        transform.position = positionOnStart.position;
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

        if (initialPositionA)
            transform.position = initialPositionA.position;

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

        if (initialPositionB)
            transform.position = initialPositionB.position;

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

#if UNITY_EDITOR
    [Button]
    public void TranslateToPositionA()
    {
        transform.position = targetPositionA.position;
    }

    [Button]
    public void TranslateToPositionB()
    {
        transform.position = targetPositionB.position;
    }
#endif
}
