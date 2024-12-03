using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

public class ScaleDoTween : MonoBehaviour
{
    [Header("Tween Settings")]
    [SerializeField] private Vector3 _startScale = Vector3.one; // Escala inicial
    [SerializeField] private Vector3 _endScale = Vector3.one * 2; // Escala final
    [SerializeField] private float _duration = 1f; // Duração do tween em segundos
    [SerializeField] private Ease _easeType = Ease.InOutQuad; // Tipo de easing
    [SerializeField] private bool _triggerOnEnable = true; // Define se o tween será acionado no OnEnable
    [SerializeField] private bool _loop = false; // Define se o tween deve repetir
    [SerializeField] private LoopType _loopType = LoopType.Restart; // Tipo de loop

    private TweenerCore<Vector3, Vector3, VectorOptions> _currentTween; // Referência ao tween atual

    private void OnEnable()
    {
        if (_triggerOnEnable)
        {
            PlayTween();
        }
    }

    private void OnDisable()
    {
        StopTween();
    }

    /// <summary>
    /// Inicia o tween de escala.
    /// </summary>
    public void PlayTween()
    {
        StopTween(); // Garante que o tween atual seja cancelado antes de criar um novo

        transform.localScale = _startScale; // Define a escala inicial

        _currentTween = transform.DOScale(_endScale, _duration)
            .SetEase(_easeType)
            .SetLoops(_loop ? -1 : 0, _loopType);
    }

    /// <summary>
    /// Para o tween atual.
    /// </summary>
    public void StopTween()
    {
        if (_currentTween != null && _currentTween.IsActive())
        {
            _currentTween.Kill();
        }
    }

    /// <summary>
    /// Atualiza os valores do tween em tempo de execução.
    /// </summary>
    public void SetTweenValues(Vector3 startScale, Vector3 endScale, float duration, Ease easeType)
    {
        _startScale = startScale;
        _endScale = endScale;
        _duration = duration;
        _easeType = easeType;
    }
}
