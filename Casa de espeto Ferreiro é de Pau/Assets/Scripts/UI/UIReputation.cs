using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // Não se esqueça de adicionar esta linha!

public class UIReputation : MonoBehaviour
{
    [SerializeField] Image _fillBar;
    [SerializeField] TMP_Text _reputationText;

    [Header("Configurações da Animação da Barra")]
    [Tooltip("Duração da animação do preenchimento da barra em segundos.")]
    public float fillAnimationDuration = 0.5f; // Duração da animação da barra
    [Tooltip("Tipo de suavização para a animação do preenchimento.")]
    public Ease fillEaseType = Ease.OutQuad; // Tipo de suavização da barra

    [Header("Configurações da Animação do Texto")]
    [Tooltip("Duração da animação do contador de reputação em segundos.")]
    public float textAnimationDuration = 0.5f; // Duração da animação do texto
    [Tooltip("Tipo de suavização para a animação do contador de reputação.")]
    public Ease textEaseType = Ease.OutSine; // Tipo de suavização do texto (ex: mais suave)

    private int _currentDisplayedReputation; // A reputação que está *sendo exibida* no texto
    private Tween _textTween; // Referência para o tween do texto para controlá-lo

    private void Start()
    {
        GameEvents.Reputation.OnReputationChanged += OnReputationChange;

        // Inicializa o texto e a barra com os valores atuais no Awake
        // Isso evita que eles comecem vazios ou em 0 antes do primeiro evento.
        // É importante que GameManager.Instance.CurrentReputation e GetReputationFill()
        // já estejam disponíveis aqui.
        _currentDisplayedReputation = GameManager.Instance.CurrentReputation;
        _reputationText.text = _currentDisplayedReputation.ToString();
        _fillBar.fillAmount = Mathf.Clamp(GameManager.Instance.GetReputationFill(), 0, 1);
    }

    private void OnDestroy()
    {
        GameEvents.Reputation.OnReputationChanged -= OnReputationChange;
        // Mata tweens ativos quando o objeto é destruído para evitar erros.
        _fillBar.DOKill();
        _textTween?.Kill(); // Usa o operador ?. para evitar NullReferenceException se _textTween for null
    }

    void OnReputationChange()
    {
        int targetReputation = GameManager.Instance.CurrentReputation;
        float targetFillAmount = Mathf.Clamp(GameManager.Instance.GetReputationFill(), 0, 1);

        // --- Animação da Barra de Preenchimento (já existente) ---
        _fillBar.DOKill(); // Mata qualquer tween anterior na barra
        _fillBar.DOFillAmount(targetFillAmount, fillAnimationDuration)
            .SetEase(fillEaseType);

        // --- Animação do Texto (NOVO!) ---
        // Mata qualquer tween de texto anterior para evitar sobreposição e garantir fluidez
        _textTween?.Kill();

        // Anima o valor numérico de _currentDisplayedReputation até targetReputation
        _textTween = DOTween.To(
                () => _currentDisplayedReputation, // Getter: Pega o valor atual
                x => _currentDisplayedReputation = x, // Setter: Define o valor (enquanto anima)
                targetReputation, // Valor final
                textAnimationDuration // Duração
            )
            .SetEase(textEaseType)
            .OnUpdate(() =>
            {
                // A cada frame da animação, atualiza o texto do TextMeshPro
                _reputationText.text = _currentDisplayedReputation.ToString();
            });
    }
}