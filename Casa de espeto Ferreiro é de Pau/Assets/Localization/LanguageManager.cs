using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class LanguageManager : MonoBehaviour
{
	public TextAsset csvFile;
	public TMP_Text buttonText; // Texto do bot�o
	private Dictionary<string, Dictionary<string, string>> translations = new Dictionary<string, Dictionary<string, string>>();
	private List<string> availableLanguages = new List<string>(); // Lista de idiomas dispon�veis
	private string currentLanguage = "PT"; // Idioma padr�o
	private int currentLanguageIndex = 0; // �ndice do idioma atual

	void Start()
	{
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

		// Adicionar idiomas dispon�veis � lista
		for (int i = 1; i < headers.Length; i++)
		{
			availableLanguages.Add(headers[i].Trim());
		}

		// Carregar as tradu��es
		for (int i = 1; i < lines.Length; i++)
		{
			string[] values = lines[i].Split(',');

			if (values.Length != headers.Length) continue; // Ignora linhas inv�lidas

			string id = values[0];

			if (!translations.ContainsKey(id))
				translations[id] = new Dictionary<string, string>();

			for (int j = 1; j < headers.Length; j++)
			{
				translations[id][headers[j].Trim()] = values[j].Trim(); // Adiciona a tradu��o
			}
		}

		// Atualiza o texto do bot�o com o idioma inicial
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
	}

	public void ChangeLanguage()
	{
		// Alterna para o pr�ximo idioma
		currentLanguageIndex = (currentLanguageIndex + 1) % availableLanguages.Count;
		string nextLanguage = availableLanguages[currentLanguageIndex];

		// Define o pr�ximo idioma
		SetLanguage(nextLanguage);

		// Atualiza o texto do bot�o
		if (buttonText != null)
		{
			buttonText.text = nextLanguage;
		}
		Debug.Log(nextLanguage);
	}
}
