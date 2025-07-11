using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System;

public class ScaleDoTween : TweenObject
{
    [Header("Tween Settings")]
    [SerializeField] private Vector3 _startScale = Vector3.one * 2; // Escala inicial
    [SerializeField] private Vector3 _endScale = Vector3.one; // Escala final
    [SerializeField] private float _duration = .15f; // Dura��o do tween em segundos
    [SerializeField] private Ease _easeType = Ease.InOutQuad; // Tipo de easing
    [SerializeField] private bool _loop = false; // Define se o tween deve repetir
    [SerializeField] private LoopType _loopType = LoopType.Restart; // Tipo de loop

    private TweenerCore<Vector3, Vector3, VectorOptions> _currentTween; // Refer�ncia ao tween atual

    private void OnDisable()
    {
        StopTween();
    }

    /// <summary>
    /// Inicia o tween de escala.
    /// </summary>
    public override void PlayTween()
    {
        StopTween(); // Garante que o tween atual seja cancelado antes de criar um novo

        transform.localScale = _startScale; // Define a escala inicial

        _currentTween = transform.DOScale(_endScale, _duration)
            .SetEase(_easeType)
            .SetLoops(_loop ? -1 : 0, _loopType);
    }

    /// <summary>
    /// Inicia o tween de escala reverso (de _endScale para _startScale).
    /// </summary>
    public override void PlayReverse()
    {
        StopTween(); // Garante que o tween atual seja cancelado antes de criar um novo

        transform.localScale = _endScale; // Começa do fim

        _currentTween = transform.DOScale(_startScale, _duration)
            .SetEase(_easeType)
            .SetLoops(0); // chama método que desativa o objeto
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
    /// Atualiza os valores do tween em tempo de execu��o.
    /// </summary>
    public void SetTweenValues(Vector3 startScale, Vector3 endScale, float duration, Ease easeType)
    {
        _startScale = startScale;
        _endScale = endScale;
        _duration = duration;
        _easeType = easeType;
    }
}
