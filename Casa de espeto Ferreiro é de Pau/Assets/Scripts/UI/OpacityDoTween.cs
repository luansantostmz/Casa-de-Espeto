using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System;

public class OpacityDoTween : TweenObject
{
    [Header("Componentes")]
    [SerializeField] private CanvasGroup _canvasGroup; // Componente CanvasGroup para controlar a opacidade

    [Header("Tween Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float _startAlpha = 0f; // Opacidade inicial (0 a 1)
    [Range(0f, 1f)]
    [SerializeField] private float _endAlpha = 1f; // Opacidade final (0 a 1)
    [SerializeField] private float _duration = .3f; // Duração do tween em segundos
    [SerializeField] private Ease _easeType = Ease.OutQuad; // Tipo de easing
    [SerializeField] private bool _loop = false; // Define se o tween deve repetir
    [SerializeField] private LoopType _loopType = LoopType.Restart; // Tipo de loop

    private TweenerCore<float, float, FloatOptions> _currentTween; // Referência ao tween atual

    private void Awake()
    {
        // Garante que temos uma referência ao CanvasGroup.
        // Se não for atribuído via Inspector, tenta obter no próprio GameObject.
        if (_canvasGroup == null)
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
            {
                Debug.LogError("OpacityDoTween requer um CanvasGroup no mesmo GameObject ou atribuído no Inspector.", this);
            }
        }
    }

    protected override void OnEnable()
    {
        // Chama a implementação base para respeitar PlayOnEnable
        base.OnEnable();
        // Garante que o CanvasGroup esteja habilitado para interatividade e raycasts
        // ao ativar o objeto, a menos que ele comece com alpha 0.
        if (_canvasGroup != null && _canvasGroup.alpha > 0.01f) // Se não estiver quase invisível
        {
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }
    }

    private void OnDisable()
    {
        // Ao desativar, apenas pare qualquer tween em andamento
        // A lógica de fade-out e desativação real será feita por HideWithFadeOut()
        StopTween();
    }

    /// <summary>
    /// Inicia o tween de opacidade (de _startAlpha para _endAlpha).
    /// </summary>
    public override void PlayTween()
    {
        if (_canvasGroup == null) return;

        StopTween(); // Garante que o tween atual seja cancelado antes de criar um novo

        _canvasGroup.alpha = _startAlpha; // Define a opacidade inicial
        _canvasGroup.interactable = false; // Desativa interatividade ao iniciar para evitar cliques durante o fade-in se startAlpha for 0
        _canvasGroup.blocksRaycasts = false; // Desativa raycasts

        _currentTween = _canvasGroup.DOFade(_endAlpha, _duration)
            .SetEase(_easeType)
            .OnComplete(() =>
            {
                // Ao completar o fade-in, reativa interatividade e raycasts
                _canvasGroup.interactable = true;
                _canvasGroup.blocksRaycasts = true;
            });

        if (_loop)
        {
            _currentTween.SetLoops(-1, _loopType);
        }
        else
        {
            _currentTween.SetLoops(0); // Não repete
        }
    }

    /// <summary>
    /// Inicia o tween de opacidade reverso (de _endAlpha para _startAlpha).
    /// Este método é para ser chamado internamente ou para um fade-out sem desativação.
    /// Para fade-out e desativação, use HideWithFadeOut().
    /// </summary>
    public override void PlayReverse()
    {
        if (_canvasGroup == null) return;

        StopTween(); // Garante que o tween atual seja cancelado antes de criar um novo

        _canvasGroup.alpha = _endAlpha; // Começa da opacidade final
        _canvasGroup.interactable = false; // Desativa interatividade e raycasts ao iniciar o fade-out
        _canvasGroup.blocksRaycasts = false;

        _currentTween = _canvasGroup.DOFade(_startAlpha, _duration)
            .SetEase(_easeType)
            .SetLoops(0); // Não repete para o reverso
    }

    /// <summary>
    /// Inicia o tween de opacidade reverso e desativa o GameObject quando a animação terminar.
    /// Chame este método em vez de gameObject.SetActive(false) diretamente.
    /// </summary>
    public void HideWithFadeOut()
    {
        if (_canvasGroup == null) return;

        StopTween(); // Garante que o tween atual seja cancelado

        _canvasGroup.interactable = false; // Desativa interatividade imediatamente
        _canvasGroup.blocksRaycasts = false; // Desativa raycasts imediatamente

        // Começa o fade-out da opacidade atual para _startAlpha
        _currentTween = _canvasGroup.DOFade(_startAlpha, _duration)
            .SetEase(_easeType)
            .OnComplete(() =>
            {
                // Só desativa o GameObject quando a animação de fade-out estiver completa
                gameObject.SetActive(false);
            });
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
    public void SetTweenValues(float startAlpha, float endAlpha, float duration, Ease easeType)
    {
        _startAlpha = startAlpha;
        _endAlpha = endAlpha;
        _duration = duration;
        _easeType = easeType;
    }
}