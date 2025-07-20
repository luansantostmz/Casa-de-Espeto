using UnityEngine;
using UnityEngine.UI;
using System; // Necessário para Action
using System.Collections; // Necessário para Coroutines

public class QTEHandler : MonoBehaviour
{
    public Image outerCircle; // O círculo externo fixo (para referência visual)
    public RectTransform fillingCircleRect; // RectTransform do círculo que encolhe
    public Image fillingCircleImage; // A Image do círculo que encolhe (para mudar a cor)
    public Button qteButton; // O botão que o jogador deve clicar

    [Header("Efeito de Pulso")]
    public Image pulseEffectImage; // A imagem do círculo de pulso
    public float pulseDuration = 0.2f; // Duração do pulso (rápido)
    public float pulseStartScale = 1.0f; // Escala inicial do pulso (1.0 = tamanho do outerCircle)
    public float pulseEndScale = 1.5f; // Escala máxima do pulso
    public Color pulseColor = Color.cyan; // Cor do pulso

    public float qteDuration = 2.0f; // Duração total do QTE em segundos
    public float perfectHitWindow = 0.1f; // Janela de tempo para pontuação máxima (0.1 segundos antes/depois do centro)
    public float goodHitWindow = 0.3f; // Janela de tempo para boa pontuação (0.3 segundos antes/depois do centro)

    [Header("Configuração de Escala")]
    public float startScale = 1.0f; // Escala inicial do círculo interno (1.0 significa 100% do tamanho do RectTransform)
    public float endScale = 0.05f; // Escala final (quase zero) quando o tempo acabar

    private float timer;
    private bool qteActive;
    private Action<int> onQTECompleteCallback; // Callback para informar a pontuação ao CraftingManager
    private bool perfectPulseTriggered; // Flag para garantir que o pulso perfeito toque apenas uma vez

    void Start()
    {
        if (qteButton != null)
        {
            qteButton.onClick.AddListener(OnQTEButtonClick);
        }

        // Garante que o QTE e o efeito de pulso comecem desativados visualmente
        if (outerCircle != null) outerCircle.gameObject.SetActive(false);
        if (fillingCircleRect != null) fillingCircleRect.gameObject.SetActive(false);
        if (qteButton != null) qteButton.gameObject.SetActive(false);
        if (pulseEffectImage != null) pulseEffectImage.gameObject.SetActive(false); // Inicia oculto
    }

    void Update()
    {
        if (qteActive)
        {
            timer += Time.deltaTime;
            float progress = timer / qteDuration; // Progresso de 0 a 1

            // O ponto ideal é no meio da duração (0.5)
            float idealProgress = 0.5f;

            // Calcula a escala do círculo interno
            float currentScale = Mathf.Lerp(startScale, endScale, progress);

            if (fillingCircleRect != null)
            {
                fillingCircleRect.localScale = new Vector3(currentScale, currentScale, 1f);
            }

            // --- Feedback visual de cor e disparo do pulso ---
            if (fillingCircleImage != null)
            {
                float distanceToPerfect = Mathf.Abs(progress - idealProgress);

                // Normaliza as janelas de acerto em relação à duração total do QTE
                float perfectHitWindowNormalized = perfectHitWindow / qteDuration;
                float goodHitWindowNormalized = goodHitWindow / qteDuration;

                if (distanceToPerfect <= perfectHitWindowNormalized)
                {
                    fillingCircleImage.color = Color.green; // Cor verde para o momento perfeito

                    // Dispara o pulso se estiver na janela perfeita E ainda não foi disparado neste QTE
                    if (!perfectPulseTriggered && pulseEffectImage != null)
                    {
                        StartCoroutine(DoPulseEffect());
                        perfectPulseTriggered = true; // Marca que o pulso foi disparado
                    }
                }
                else if (distanceToPerfect <= goodHitWindowNormalized)
                {
                    fillingCircleImage.color = Color.yellow; // Cor amarela para "bom"
                    perfectPulseTriggered = false; // Permite que o pulso seja disparado se voltar para o verde
                                                   // (Importante se o jogador passar e voltar para a zona verde)
                }
                else
                {
                    fillingCircleImage.color = Color.white; // Cor branca padrão
                    perfectPulseTriggered = false; // Permite que o pulso seja disparado se entrar na zona verde
                }
            }

            if (timer >= qteDuration)
            {
                EndQTE(0); // QTE falhou por tempo esgotado, pontuação zero
            }
        }
    }

    public void StartQTE(Action<int> callback)
    {
        onQTECompleteCallback = callback;
        timer = 0f;
        qteActive = true;
        perfectPulseTriggered = false; // Reseta a flag do pulso para o novo QTE

        // Reativa e configura os elementos visuais do QTE
        if (outerCircle != null)
        {
            outerCircle.gameObject.SetActive(true);
            outerCircle.transform.localScale = Vector3.one;
        }
        if (fillingCircleRect != null)
        {
            fillingCircleRect.gameObject.SetActive(true);
            fillingCircleRect.localScale = new Vector3(startScale, startScale, 1f); // Inicia com a escala definida
            if (fillingCircleImage != null)
            {
                fillingCircleImage.color = Color.white; // Reinicia a cor
            }
        }
        if (qteButton != null) qteButton.gameObject.SetActive(true);
        qteButton.interactable = true;

        if (pulseEffectImage != null) pulseEffectImage.gameObject.SetActive(false); // Garante que esteja oculto no início
    }

    void OnQTEButtonClick()
    {
        if (qteActive)
        {
            int score = CalculateScore(); // Calcula a pontuação no momento do clique
            Debug.Log("QTE Pontuação: " + score);
            EndQTE(score);
        }
    }

    // Calcula a pontuação baseada no tempo do clique
    int CalculateScore()
    {
        float progress = timer / qteDuration;
        float idealProgress = 0.5f;
        float timeDifference = Mathf.Abs(progress - idealProgress);

        float perfectHitWindowNormalized = perfectHitWindow / qteDuration;
        float goodHitWindowNormalized = goodHitWindow / qteDuration;

        int score = 0;

        if (timeDifference <= perfectHitWindowNormalized)
        {
            score = 100; // Pontuação máxima
        }
        else if (timeDifference <= goodHitWindowNormalized)
        {
            // Mapeia a pontuação de 100 para 50 dentro da janela "good"
            // Quanto mais perto do perfeito, maior a pontuação dentro da janela "good"
            float normalizedDiff = (timeDifference - perfectHitWindowNormalized) / (goodHitWindowNormalized - perfectHitWindowNormalized);
            score = Mathf.RoundToInt(Mathf.Lerp(100, 50, normalizedDiff));
        }
        else
        {
            // Pontuação mínima (ou falha) para cliques muito fora
            // Mapeia de 50 para 0, dependendo de quão longe está, até o limite de 0.5 (metade da duração do QTE)
            float maxMissDifference = 0.5f;
            float normalizedMiss = Mathf.Clamp01((timeDifference - goodHitWindowNormalized) / (maxMissDifference - goodHitWindowNormalized));
            score = Mathf.RoundToInt(Mathf.Lerp(50, 0, normalizedMiss));
        }
        return score;
    }

    // Coroutine para o efeito de pulso visual
    IEnumerator DoPulseEffect()
    {
        if (pulseEffectImage == null || outerCircle == null) yield break;

        pulseEffectImage.gameObject.SetActive(true);
        pulseEffectImage.color = pulseColor; // Define a cor do pulso
        pulseEffectImage.transform.position = outerCircle.transform.position; // Centraliza no outerCircle

        float elapsed = 0f;
        while (elapsed < pulseDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / pulseDuration;

            // Escala o pulso de startScale para endScale
            float currentPulseScale = Mathf.Lerp(pulseStartScale, pulseEndScale, progress);
            pulseEffectImage.transform.localScale = Vector3.one * currentPulseScale;

            // Fade out da imagem durante o pulso
            Color currentColor = pulseEffectImage.color;
            currentColor.a = Mathf.Lerp(1f, 0f, progress); // Vai de opaco (1) para transparente (0)
            pulseEffectImage.color = currentColor;

            yield return null; // Espera o próximo frame
        }
        pulseEffectImage.gameObject.SetActive(false); // Esconde o pulso após o efeito
    }

    void EndQTE(int finalScore)
    {
        qteActive = false;
        qteButton.interactable = false; // Desativa o botão

        // Desativa os elementos visuais do QTE
        if (outerCircle != null) outerCircle.gameObject.SetActive(false);
        if (fillingCircleRect != null) fillingCircleRect.gameObject.SetActive(false);
        if (qteButton != null) qteButton.gameObject.SetActive(false);
        // Garante que o pulso também seja desativado se o QTE terminar (caso o QTE acabe antes do pulso sumir)
        if (pulseEffectImage != null) pulseEffectImage.gameObject.SetActive(false);

        if (onQTECompleteCallback != null)
        {
            onQTECompleteCallback.Invoke(finalScore);
        }
    }
}