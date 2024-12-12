using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using TMPro;
using UnityEditor;
using UnityEngine;

public class LocalizationService : MonoBehaviour
{
	private const string csvUrlBase = "https://docs.google.com/spreadsheets/d/1kdne-qY0r5m3on7c05eRZwZ9ZJZX2AcNECntPOeWcLM/export?format=csv";
	private const string savePathBase = "Assets/CSVFiles/arquivo_{0}.csv"; // Caminho onde o arquivo ser� salvo no projeto, com o idioma inclu�do.

	private static Dictionary<string, string> localizationData = new Dictionary<string, string>();
	private static string currentLanguage = "pt"; // Idioma padr�o

	// Armazena os idiomas dispon�veis
	private static List<string> supportedLanguages = new List<string> { "pt", "en", "es" };

	// M�todo para baixar e salvar o CSV do idioma selecionado
	[MenuItem("Tools/Baixar CSV e Salvar", priority = 1)]
	public static void DownloadAndSaveCSV()
	{
		try
		{
			string savePath = string.Format(savePathBase, currentLanguage); // Altera o caminho com base no idioma
			string csvUrl = string.Format(csvUrlBase, currentLanguage); // Altera a URL com base no idioma

			// Cria a pasta caso n�o exista
			string directory = Path.GetDirectoryName(savePath);
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
				Debug.Log($"Pasta criada: {directory}");
			}

			using (WebClient client = new WebClient())
			{
				client.DownloadFile(csvUrl, savePath);
				Debug.Log($"Arquivo CSV ({currentLanguage}) baixado e salvo em: {savePath}");
			}

			// For�a o Unity a reconhecer o novo arquivo
			AssetDatabase.Refresh();
		}
		catch (Exception ex)
		{
			Debug.LogError($"Erro ao baixar o arquivo: {ex.Message}");
		}
	}

	// M�todo para inicializar a localiza��o (carregar o arquivo CSV)
	[RuntimeInitializeOnLoadMethod]
	public static void Initialize()
	{
		string savePath = string.Format(savePathBase, currentLanguage);
		if (!File.Exists(savePath))
		{
			Debug.LogError("Arquivo CSV n�o encontrado. Certifique-se de baix�-lo antes de inicializar.");
			return;
		}

		localizationData.Clear();

		string[] lines = File.ReadAllLines(savePath);
		Debug.Log($"Carregando {lines.Length} linhas do arquivo CSV...");

		foreach (var line in lines)
		{
			if (string.IsNullOrWhiteSpace(line)) continue; // Ignora linhas vazias

			string[] parts = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			if (parts.Length >= 2)
			{
				string key = parts[0].Trim();
				string value = parts[1].Trim();

				if (!localizationData.ContainsKey(key))
				{
					localizationData[key] = value;
					Debug.Log($"Carregado: [{key}] = [{value}]");
				}
				else
				{
					Debug.LogWarning($"Chave duplicada encontrada: {key}");
				}
			}
			else
			{
				Debug.LogWarning($"Linha inv�lida no CSV: {line}");
			}
		}

		Debug.Log("Localiza��o inicializada com sucesso.");
	}

	// M�todo para localizar uma chave
	public static string Localize(string key)
	{
		if (localizationData.TryGetValue(key, out string value))
		{
			return value;
		}

		Debug.LogWarning($"Chave de localiza��o n�o encontrada: {key}");
		return key; // Retorna a chave original se n�o for encontrada.
	}

	public static Action OnChangedLanguage;	
	// M�todo para trocar o idioma
	public static void ChangeLanguage(string language)
	{
		if (supportedLanguages.Contains(language))
		{
			currentLanguage = language;
			Initialize(); // Recarrega as tradu��es do novo idioma
			Debug.Log($"Idioma alterado para: {language}");
			OnChangedLanguage?.Invoke();
		}
		else
		{
			Debug.LogWarning($"Idioma n�o suportado: {language}");
		}
	}
}
