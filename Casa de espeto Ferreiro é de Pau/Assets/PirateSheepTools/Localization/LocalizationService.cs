using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace PirateSheep.Localization
{
	public static class LocalizationService
	{
		// Dicionário: idioma -> (chave -> valor)
		private static Dictionary<string, Dictionary<string, string>> _localizedTexts;
		private static string _currentLanguage = "en";

		private static string[] allLanguages = new string[0];

		public static event Action OnLanguageChanged;

		public static string CurrentLanguage => _currentLanguage;

		public static void Init()
		{
			LoadLocalizationJSON();
			OnLanguageChanged?.Invoke();
		}

		private static void LoadLocalizationJSON()
		{
			TextAsset jsonFile = Resources.Load<TextAsset>("locales");

			if (jsonFile == null)
			{
				Debug.LogError("File 'locales.json' not found in Resources folder.");
				return;
			}

			try
			{
				// Desserializa para Dictionary<language, Dictionary<key, value>>
				_localizedTexts = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(jsonFile.text);

				if (_localizedTexts == null || _localizedTexts.Count == 0)
				{
					Debug.LogError("Localization JSON is empty or invalid.");
					return;
				}

				allLanguages = new List<string>(_localizedTexts.Keys).ToArray();

				Debug.Log("Localization loaded successfully from JSON.");
			}
			catch (Exception ex)
			{
				Debug.LogError("Failed to load localization JSON: " + ex.Message);
			}
		}

		public static void SetLanguage(string languageCode)
		{
			if (_localizedTexts == null)
			{
				Debug.LogWarning("LocalizationService not initialized. Initializing now.");
				Init();
			}

			if (allLanguages != null && Array.Exists(allLanguages, lang => lang == languageCode))
			{
				_currentLanguage = languageCode;
				OnLanguageChanged?.Invoke();
			}
			else
			{
				Debug.LogWarning($"Language '{languageCode}' not found in localization data.");
			}
		}

		public static string GetLocalizedText(string key)
		{
			if (_localizedTexts == null)
			{
				Debug.LogError("LocalizationService not initialized. Call Init() first.");
				return $"[!{key}]";
			}

			if (_localizedTexts.TryGetValue(_currentLanguage, out var translations) &&
				translations != null &&
				translations.TryGetValue(key, out var value))
			{
				return value;
			}

			return $"[!{key}]";
		}

		public static string[] GetAllKeys()
		{
			if (_localizedTexts == null || _localizedTexts.Count == 0)
				return new string[] { "(no keys)" };

			// Pega todas as chaves do idioma atual (ou do primeiro idioma disponível)
			Dictionary<string, string> dict = null;

			if (_localizedTexts.TryGetValue(_currentLanguage, out var translations))
			{
				dict = translations;
			}
			else
			{
				// fallback: pega o primeiro idioma
				foreach (var langDict in _localizedTexts.Values)
				{
					dict = langDict;
					break;
				}
			}

			if (dict == null)
				return new string[] { "(no keys)" };

			var keys = new List<string>(dict.Keys);
			return keys.ToArray();
		}

		public static string[] GetAllLanguages()
		{
			return allLanguages;
		}
	}
}
