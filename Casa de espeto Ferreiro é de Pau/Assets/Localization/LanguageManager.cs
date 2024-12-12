using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(TMP_Text))]
public class LanguageManager : MonoBehaviour
{
	public TextAsset csvFile;
	public TMP_Text buttonText; // Texto do botão
	public Image languageImage; // Imagem que muda de acordo com o idioma
	public Sprite[] languageSprites; // Array de Sprites (imagens) para os idiomas
	private Dictionary<string, Dictionary<string, string>> translations = new Dictionary<string, Dictionary<string, string>>();
	private List<string> availableLanguages = new List<string>(); // Lista de idiomas disponíveis
	private string currentLanguage = "PT"; // Idioma padrão
	private int currentLanguageIndex = 0; // Índice do idioma atual

	void Start()
	{
		// Verificar se o idioma está salvo no PlayerPrefs
		if (PlayerPrefs.HasKey("Language"))
		{
			currentLanguage = PlayerPrefs.GetString("Language");
			currentLanguageIndex = PlayerPrefs.GetInt("LanguageIndex", 0); // Recuperar o índice da imagem
		}

		if (csvFile != null)
		{
			LoadTranslations(csvFile.text);
			SetLanguage(currentLanguage);
		}
		else
		{
			Debug.LogError("CSV file not assigned!");
		}
	}

	void LoadTranslations(string csvContent)
	{
		var lines = csvContent.Split('\n');
		string[] headers = lines[0].Split(',');

		// Adicionar idiomas disponíveis à lista
		for (int i = 1; i < headers.Length; i++)
		{
			availableLanguages.Add(headers[i].Trim());
		}

		// Carregar as traduções
		for (int i = 1; i < lines.Length; i++)
		{
			string[] values = lines[i].Split(',');

			if (values.Length != headers.Length) continue; // Ignora linhas inválidas

			string id = values[0];

			if (!translations.ContainsKey(id))
				translations[id] = new Dictionary<string, string>();

			for (int j = 1; j < headers.Length; j++)
			{
				translations[id][headers[j].Trim()] = values[j].Trim(); // Adiciona a tradução
			}
		}

		// Atualiza o texto do botão com o idioma inicial
		if (buttonText != null)
		{
			buttonText.text = currentLanguage;
		}
	}

	public void SetLanguage(string language)
	{
		currentLanguage = language;

		// Atualiza todos os textos na cena
		var translatableTexts = FindObjectsOfType<TranslatableText>();
		foreach (var translatableText in translatableTexts)
		{
			if (translations.ContainsKey(translatableText.translationID) && translations[translatableText.translationID].ContainsKey(language))
			{
				string translatedText = translations[translatableText.translationID][language];
				translatableText.UpdateText(translatedText);
			}
		}

		// Salva o idioma e o índice no PlayerPrefs
		PlayerPrefs.SetString("Language", currentLanguage);
		PlayerPrefs.SetInt("LanguageIndex", currentLanguageIndex);

		// Atualiza o texto do botão
		if (buttonText != null)
		{
			buttonText.text = currentLanguage;
		}

		// Atualiza a imagem de acordo com o idioma selecionado
		UpdateLanguageImage();
	}

	public void ChangeLanguage()
	{
		// Alterna para o próximo idioma
		currentLanguageIndex = (currentLanguageIndex + 1) % availableLanguages.Count;
		string nextLanguage = availableLanguages[currentLanguageIndex];

		// Define o próximo idioma
		SetLanguage(nextLanguage);

		Debug.Log(nextLanguage);
	}

	private void UpdateLanguageImage()
	{
		if (languageImage != null && languageSprites.Length > currentLanguageIndex)
		{
			languageImage.sprite = languageSprites[currentLanguageIndex]; // Atualiza a imagem com base no índice do idioma
		}
	}
}
