using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizedText : MonoBehaviour
{
	[SerializeField]
	private string localizationKey;

	private TextMeshProUGUI textComponent;

	private void Awake()
	{
		textComponent = GetComponent<TextMeshProUGUI>();
		LocalizationService.OnChangedLanguage += UpdateText;
	}
	private void OnDestroy()
	{
		LocalizationService.OnChangedLanguage -= UpdateText;
	}
	private void Start()
	{
		UpdateText();
	}
	public void UpdateText()
	{
		if (string.IsNullOrEmpty(localizationKey))
		{
			Debug.LogWarning("Chave de localização não definida no componente LocalizedText.");
			return;
		}

		string localizedText = LocalizationService.Localize(localizationKey);
		textComponent.text = localizedText;
	}

	public void SetKey(string newKey)
	{
		localizationKey = newKey;
		UpdateText();
	}
}
