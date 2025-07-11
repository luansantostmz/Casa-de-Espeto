using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PirateSheep.Localization
{
	public static class LocalizationService
	{
		private static Dictionary<string, Dictionary<string, string>> _localizedTexts;
		private static string _currentLanguage = "en";

		private static string[] allLanguages = new string[0];

		public static event Action OnLanguageChanged;

		public static string CurrentLanguage => _currentLanguage;

		public static void Init()
		{
			LoadLocalizationCSV();
			OnLanguageChanged?.Invoke();
		}

		private static void LoadLocalizationCSV()
		{
			TextAsset csvFile = Resources.Load<TextAsset>("locales");

			if (csvFile == null)
			{
				Debug.LogError("Locales.csv file not found in Resources folder.");
				return;
			}

			_localizedTexts = new Dictionary<string, Dictionary<string, string>>();

			string[] lines = csvFile.text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

			if (lines.Length < 2)
			{
				Debug.LogError("CSV not found or is empty.");
				return;
			}

			string[] headers = lines[0].Split(',');

			allLanguages = headers.Skip(1).Select(h => h.Trim()).ToArray();

			for (int i = 1; i < lines.Length; i++)
			{
				string[] values = lines[i].Split(',');

				if (values.Length < 2) continue;

				string key = values[0].Trim();

				for (int j = 1; j < headers.Length; j++)
				{
					string lang = headers[j].Trim();
					string value = j < values.Length ? values[j].Trim() : "";

					if (!_localizedTexts.ContainsKey(lang))
						_localizedTexts[lang] = new Dictionary<string, string>();

					_localizedTexts[lang][key] = value;
				}
			}

			Debug.Log("Localization loaded successfully.");
		}

		public static void SetLanguage(string languageCode)
		{
			if (_localizedTexts == null)
			{
				Debug.LogWarning("LocalizationService not initialized. Initializing now.");
				Init();
			}

			if (_localizedTexts.ContainsKey(languageCode))
			{
				_currentLanguage = languageCode;
				OnLanguageChanged?.Invoke();
			}
			else
			{
				Debug.LogWarning($"Language '{languageCode}' not found on CSV.");
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

			var firstLang = _localizedTexts.Values.FirstOrDefault();
			if (firstLang == null) return new string[] { "(no keys)" };

			return firstLang.Keys.ToArray();
		}

		public static string[] GetAllLanguages()
		{
			return allLanguages;
		}
	}

}