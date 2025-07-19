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

        // Verifica se a sequência de rotação já está ativa.
        // Se estiver ativa e não estiver completada ou morta, saímos do método.
        // Isso impede que o tween seja reiniciado se já estiver em andamento.
        if (rotateSequence != null && rotateSequence.IsActive())
        {
            Debug.Log("Tween de rotação já ativo, ignorando nova chamada.");
            return;
        }

        // Resetar a rotação para o valor inicial antes de iniciar o tween.
        // Isso garante que a animação sempre comece do ponto de partida original.
        transform.rotation = initialRotation;

        // Mata qualquer tween anterior no transform.
        // O 'true' faz com que ele complete instantaneamente se houver um tween anterior,
        // mas como já resetamos a rotação, o efeito visual será de um novo começo.
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
        rotateSequence.SetLink(gameObject); // Linka a sequência ao GameObject
        rotateSequence.Play(); // Inicia a sequência
    }

    // Este método 'ResetRotation' ainda pode ser útil para um reset manual forçado para a posição inicial,
    // independentemente de qualquer animação em andamento.
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