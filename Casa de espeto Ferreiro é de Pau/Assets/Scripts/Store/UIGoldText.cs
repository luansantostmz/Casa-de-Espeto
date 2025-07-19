using TMPro;
using UnityEngine;
using DG.Tweening; // Importar DOTween

public class UIGoldText : MonoBehaviour
{
    private TMP_Text text; // Já está private, o que é bom.

    [Header("Configurações da Animação do Texto")]
    [Tooltip("Duração da animação do contador de ouro em segundos.")]
    public float textAnimationDuration = 0.5f; // Duração da animação do texto
    [Tooltip("Tipo de suavização para a animação do contador de ouro.")]
    public Ease textEaseType = Ease.OutSine; // Suavização do texto (ex: mais suave)

    private int _currentDisplayedGold; // O valor de ouro que está sendo exibido no texto
    private Tween _textTween; // Referência para o tween do texto para controlá-lo

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
        // Inicializa o valor de exibição com o ouro atual
        _currentDisplayedGold = EconomyService.CurrentGold;
        text.text = _currentDisplayedGold.ToString(); // Define o texto inicial

        GameEvents.Economy.OnGoldChanged += UpdateValue;
    }

    private void OnEnable()
    {
        // Ao ser habilitado, garante que o valor exibido esteja correto
        // e, opcionalmente, o anima se ele for diferente do atual.
        UpdateValue(); // Chamada com 'true' para forçar a animação se o valor mudar ao habilitar.
    }

    private void OnDestroy()
    {
        GameEvents.Economy.OnGoldChanged -= UpdateValue;
        // Mata tweens ativos quando o objeto é destruído para evitar erros
        _textTween?.Kill();
    }

    // Alterado para aceitar um parâmetro opcional para forçar a animação
    private void UpdateValue()
    {
        int targetGold = EconomyService.CurrentGold;

        // Anima o texto do ouro
        // Mata qualquer tween de texto anterior para evitar sobreposição e garantir fluidez
        _textTween?.Kill();

        // Só inicia a animação se o valor alvo for diferente do valor atualmente exibido
        // OU se a animação for explicitamente forçada (como no OnEnable).
        if (_currentDisplayedGold != targetGold)
        {
            // Anima o valor numérico de _currentDisplayedGold até targetGold
            _textTween = DOTween.To(
                    () => _currentDisplayedGold, // Getter: Pega o valor atual
                    x => _currentDisplayedGold = x, // Setter: Define o valor (enquanto anima)
                    targetGold, // Valor final
                    textAnimationDuration // Duração
                )
                .SetEase(textEaseType)
                .OnUpdate(() =>
                {
                    // A cada frame da animação, atualiza o texto do TextMeshPro
                    text.text = _currentDisplayedGold.ToString();
                })
                .OnComplete(() =>
                {
                    // Garante que o valor final seja exatamente o targetGold ao final da animação
                    _currentDisplayedGold = targetGold;
                    text.text = _currentDisplayedGold.ToString();
                    Debug.Log("Texto do ouro atualizado suavemente para: " + targetGold);
                });
        }
        else
        {
            // Se o valor não mudou e não foi forçado, apenas atualiza o texto diretamente (sem animação)
            // Isso evita criar um tween desnecessário.
            text.text = targetGold.ToString();
            Debug.Log("Texto do ouro já no valor correto: " + targetGold);
        }

        // Você tinha um GetComponent<ScaleDoTween>().PlayTween(); aqui.
        // Mantenho, mas lembre-se que isso vai disparar a animação de escala
        // no próprio objeto do texto, se ele tiver o componente ScaleDoTween.
        // Se a ideia é animar *outro* elemento visual ligado ao ouro,
        // você precisaria de uma referência para esse outro elemento.
        // Por exemplo, se o texto e a imagem do ouro fossem filhos de um painel,
        // e esse painel tivesse o ScaleDoTween.
        if (TryGetComponent<ScaleDoTween>(out ScaleDoTween scaleTween))
        {
            scaleTween.PlayTween();
        }
    }
}