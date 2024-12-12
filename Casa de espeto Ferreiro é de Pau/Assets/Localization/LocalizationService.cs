using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using UnityEditor;
using UnityEngine;

public class LocalizationService : MonoBehaviour
{
	private const string csvUrlBase = "https://docs.google.com/spreadsheets/d/1kdne-qY0r5m3on7c05eRZwZ9ZJZX2AcNECntPOeWcLM/export?format=csv";
	private const string savePathBase = "Assets/Localization/localization.csv"; // Caminho onde o arquivo será salvo no projeto, com o idioma incluído.

	private static Dictionary<string, Dictionary<string, string>> localizationData = new ();
	private static string currentLanguage; // Idioma padrão

	// Armazena os idiomas disponíveis
	private static List<string> supportedLanguages = new List<string>();

    // Método para baixar e salvar o CSV do idioma selecionado
    [MenuItem("Tools/Update Localization", priority = 1)]
	public static void UpdateLocalization()
	{
		try
		{
			// Cria a pasta caso não exista
			string directory = Path.GetDirectoryName(savePathBase);
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
				Debug.Log($"Pasta criada: {directory}");
			}

			using (WebClient client = new WebClient())
			{
				client.DownloadFile(csvUrlBase, savePathBase);
				Debug.Log($"Arquivo CSV ({currentLanguage}) baixado e salvo em: {savePathBase}");
			}

			// Força o Unity a reconhecer o novo arquivo
			AssetDatabase.Refresh();
		}
		catch (Exception ex)
		{
			Debug.LogError($"Erro ao baixar o arquivo: {ex.Message}");
		}
	}

	// Método para inicializar a localização (carregar o arquivo CSV)
	[RuntimeInitializeOnLoadMethod]
	public static void Initialize()
	{
		if (!File.Exists(savePathBase))
		{
			Debug.LogError("Arquivo CSV não encontrado. Certifique-se de baixá-lo antes de inicializar.");
			return;
		}

		localizationData.Clear();

		string[] lines = File.ReadAllLines(savePathBase);
		Debug.Log($"Carregando {lines.Length} linhas do arquivo CSV...");

		// Separate the first line into a list of column names
		supportedLanguages = lines[0].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
		supportedLanguages.RemoveAt(0);

        localizationData = new ();
		
		// Populate languages on dictionary
        foreach (var language in supportedLanguages)
		{
			localizationData.Add(language, new Dictionary<string, string>());
		}

		// Populate translations on dictionary
		for (int i = 1; i < lines.Length; i++)
		{
			string line = lines[i];
			if (string.IsNullOrWhiteSpace(line)) 
				continue; 

			string[] parts = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			
			// Create a dictionary for this row, mapping column names to values
			for (int j = 0; j < supportedLanguages.Count; j++)
			{
				localizationData[supportedLanguages[j]].Add(parts[0], parts[i]);
            }
        }

		ChangeLanguage(supportedLanguages[0]);
	}

	// Método para localizar uma chave
	public static string Localize(string key)
	{
		if (!localizationData.ContainsKey(currentLanguage) || !string.IsNullOrEmpty(localizationData[currentLanguage][key]))
			return $"*{key}*";

		return localizationData[currentLanguage][key]; 
	}

	public static Action OnChangedLanguage;	
	// Método para trocar o idioma
	public static void ChangeLanguage(string language)
	{
		if (supportedLanguages.Contains(language))
		{
			currentLanguage = language;
			Debug.Log($"Idioma alterado para: {language}");
			OnChangedLanguage?.Invoke();
		}
		else
		{
			Debug.LogWarning($"Idioma não suportado: {language}");
		}
	}
}
