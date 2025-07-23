using UnityEngine;
using DG.Tweening; // Não esqueça de incluir o namespace do DOTween

public class ShakeTween : TweenObject
{
    [Header("Configurações do Shake")]
    [Tooltip("Duração do efeito de shake em segundos.")]
    public float shakeDuration = 0.5f;

    [Tooltip("Força do shake (amplitude do movimento). Valores maiores resultam em um shake mais intenso.")]
    public float shakeStrength = 0.5f;

    [Tooltip("Vibração do shake (número de movimentos rápidos). Valores maiores tornam o shake mais 'nervoso'.")]
    [Range(0, 100)] // Adiciona um slider no Inspector
    public int shakeVibrato = 10;

    [Tooltip("Aleatoriedade do shake. Controla o quão imprevisível o shake será.")]
    [Range(0, 180)] // Adiciona um slider no Inspector, 90 é um bom valor padrão
    public float shakeRandomness = 90f;

    [Tooltip("Define se o shake será suavizado no início e no fim.")]
    public bool fadeOut = true;

    private Vector3 initialPosition; // Posição inicial para onde o objeto retornará se o shake não for relativo

    private void Awake() // Use Awake para garantir que a posição inicial seja salva antes de qualquer Start()
    {
        initialPosition = transform.position;
    }

    public override void PlayTween()
    {
        base.PlayTween(); // Chama o método PlayTween da classe base, se houver.

        // Interrompe qualquer tween de shake ativo neste objeto para evitar sobreposição
        transform.DOKill(true);

        // Salva a posição atual antes de aplicar o shake para garantir que ele retorne corretamente
        // se você chamar outro tween depois.
        initialPosition = transform.position;

        // Aplica o shake na posição do objeto
        transform.DOShakePosition(
            shakeDuration,      // Duração do shake
            shakeStrength,      // Força do shake
            shakeVibrato,       // Vibração (quantidade de tremidas)
            shakeRandomness,    // Aleatoriedade
            fadeOut             // Suavizar o fim do shake
        )
        .OnComplete(() =>
        {
            Debug.Log("Shake concluído!");
            // Opcional: Você pode querer resetar a posição para a initialPosition
            // se o shake for muito forte e desviar o objeto permanentemente.
            // transform.position = initialPosition; 
        });
    }

    /// <summary>
    /// Reseta a posição do objeto para sua posição inicial, com um pequeno tween.
    /// Útil se o shake tiver deixado o objeto ligeiramente fora do lugar.
    /// </summary>
    public void ResetPositionAfterShake(float resetDuration = 0.2f, Ease resetEase = Ease.OutQuad)
    {
        transform.DOMove(initialPosition, resetDuration)
                 .SetEase(resetEase);
    }
}