using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class LanguageManager : MonoBehaviour
{
	public TextAsset csvFile;

	private Dictionary<string, Dictionary<string, string>> translations = new Dictionary<string, Dictionary<string, string>>();
	private string currentLanguage = "PT"; // Idioma padr�o

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

		// Exibir os idiomas dispon�veis no Console (debug)
		Debug.Log("Idiomas dispon�veis:");
		for (int i = 1; i < headers.Length; i++)
		{
			Debug.Log(headers[i]);  // Exibe os idiomas no Console
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
				translations[id][headers[j]] = values[j].Trim(); // Adiciona a tradu��o
			}
		}

		// Exibir as tradu��es de cada idioma no Console (debug)
		Debug.Log("Tradu��es carregadas:");
		foreach (var translation in translations)
		{
			string id = translation.Key;
			string translationsForID = $"ID: {id}";
			foreach (var language in translation.Value)
			{
				translationsForID += $" | {language.Key}: {language.Value}";
			}
			Debug.Log(translationsForID);  // Exibe o ID e suas tradu��es no Console
		}
	}

	// Altera o idioma
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

	// Fun��o chamada pelo bot�o para mudar de idioma
	public void ChangeLanguage(string language)
	{
		SetLanguage(language);
	}
}
