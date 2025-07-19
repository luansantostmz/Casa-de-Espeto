using UnityEngine;
using DG.Tweening;

public class RotateDOTween : TweenObject
{
    public Vector3 targetRotation = new Vector3(0, 0, 10); // Exemplo: inclinar 10 graus no eixo Z
    public float duration = 0.2f; // Duração mais curta para um efeito rápido de UI
    public Ease easeType = Ease.OutQuad; // Suavização para o movimento de ida
    public Ease reverseEaseType = Ease.InQuad; // Suavização para o movimento de volta (diferente para dar um feeling de "retorno")
    public RotateMode rotateMode = RotateMode.Fast; // Modo Fast para rotações menores que 360

    private Quaternion initialRotation; // Rotação inicial para resetar
    private Sequence rotateSequence; // Referência para a sequência de animação

    private void Awake()
    {
        // Salva a rotação inicial do objeto
        initialRotation = transform.rotation;
    }

    public override void PlayTween()
    {
        base.PlayTween();

        // Resetar a rotação antes de iniciar o tween
        transform.rotation = initialRotation;

        // Mata qualquer tween anterior
        transform.DOKill(true);

        // Cria nova sequência
        rotateSequence = DOTween.Sequence();

        // Tween de ida
        rotateSequence.Append(
            transform.DORotate(targetRotation, duration, rotateMode)
                .SetEase(easeType)
        );

        // Tween de volta
        rotateSequence.Append(
            transform.DORotateQuaternion(initialRotation, duration)
                .SetEase(reverseEaseType)
        );

        rotateSequence.OnComplete(() => Debug.Log("Sequência de rotação (ida e volta) concluída!"));
        rotateSequence.SetLink(gameObject);
        rotateSequence.Play();
    }


    // Este método não é mais necessário para a lógica de "ida e volta automática"
    // mas pode ser útil se você quiser um reset manual em outras situações.
    public void ResetRotation()
    {
        // Mata a sequência atual e força o objeto de volta à posição inicial
        if (rotateSequence != null && rotateSequence.IsActive())
        {
            rotateSequence.Kill();
        }
        transform.rotation = initialRotation;
        Debug.Log("Objeto resetado para a rotação inicial.");
    }

    private void OnDisable()
    {
        // Garante que a sequência seja morta quando o objeto for desativado ou destruído
        if (rotateSequence != null && rotateSequence.IsActive())
        {
            rotateSequence.Kill();
        }
    }
}