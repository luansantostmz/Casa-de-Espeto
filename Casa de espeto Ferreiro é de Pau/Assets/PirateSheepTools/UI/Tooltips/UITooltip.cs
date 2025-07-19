using PirateSheep.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UITooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private const float DELAY = .5f;

    [SerializeField] private GameObject _tooltipObject;

    private float _timer;
    private bool _isHovering;
    private TweenObject _tween; // Assumo que TweenObject é uma classe existente para animação

    private void Awake()
    {
        // Garante que o tooltip esteja desativado ao iniciar.
        // Se _tooltipObject for nulo, as outras funções lidarão com isso.
        _tooltipObject?.SetActive(false);
        _tween = _tooltipObject?.GetComponent<TweenObject>();
    }

    private void Update()
    {
        // Se estiver sobre o elemento e o tooltip não estiver ativo ou em processo de ser mostrado
        if (_isHovering && !IsTooltipActive())
        {
            _timer += Time.unscaledDeltaTime;
            if (_timer >= DELAY)
            {
                ShowTooltip();
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isHovering = true;
        _timer = 0f; // Reinicia o timer para o delay

        // Se o tooltip já estiver ativo por alguma razão (ex: mouse saiu e voltou muito rápido),
        // garante que ele não seja exibido imediatamente sem o delay.
        if (IsTooltipActive())
        {
            HideTooltipImmediately();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isHovering = false;
        _timer = 0f; // Reinicia o timer, caso o mouse volte antes do delay terminar

        HideTooltip();
    }

    private void ShowTooltip()
    {
        if (_tooltipObject == null) return;

        // Ativa o objeto. Se houver um TweenObject, ele gerenciará a animação.
        // Se não houver, ele simplesmente se torna visível.
        _tooltipObject.SetActive(true);
    }

    private void HideTooltip()
    {
        if (_tooltipObject == null) return;

        if (_tween != null)
        {
            // Inicia a animação inversa. A responsabilidade de desativar o objeto
            // no final da animação é do TweenObject.
            _tween.PlayReverse();
        }
        else
        {
            // Se não houver TweenObject, apenas desativa o objeto.
            _tooltipObject.SetActive(false);
        }
    }

    // Novo método para esconder o tooltip imediatamente, sem animação,
    // útil para reiniciar o estado quando o mouse entra novamente.
    private void HideTooltipImmediately()
    {
        if (_tooltipObject != null)
        {
            _tooltipObject.SetActive(false);
        }
    }

    // Novo método para verificar se o tooltip está ativo.
    // Isso lida com o estado visual do tooltip, seja por SetActive(true) direto
    // ou se o TweenObject está em processo de animação para mostrar.
    private bool IsTooltipActive()
    {
        return _tooltipObject != null && _tooltipObject.activeSelf;
    }
}