using System;
using System.Collections.Generic;
using System.IO; // Adicionado para Path.Combine
using System.Linq; // Adicionado para LINQ
using UnityEngine;
using Newtonsoft.Json;

namespace PirateSheep.Localization
{
	public static class LocalizationService
	{
		// Dicionário para armazenar APENAS as chaves/valores do idioma ATUAL
		private static Dictionary<string, string> _currentLanguageTexts;
		private static string _currentLanguageCode = "en"; // Renomeado para clareza

		// Lista de todos os idiomas disponíveis (obtida dos nomes dos arquivos)
		private static string[] _availableLanguages = new string[0];

		public static event Action OnLanguageChanged;

		public static string CurrentLanguage => _currentLanguageCode; // Alterado

		/// <summary>
		/// Inicializa o serviço de localização, carregando a lista de idiomas disponíveis
		/// e definindo o idioma inicial.
		/// </summary>
		public static void Init()
		{
			LoadAvailableLanguages(); // Carrega a lista de idiomas dos arquivos

			// Define um idioma padrão se a lista não estiver vazia
			if (_availableLanguages.Length > 0)
			{
				// Tenta definir para o idioma salvo anteriormente ou o primeiro da lista
				string defaultLang = PlayerPrefs.GetString("Localization.LastLanguage", _availableLanguages[0]);
				SetLanguage(defaultLang);
			}
			else
			{
				Debug.LogWarning("Nenhum arquivo de idioma encontrado na pasta Resources/Locales. O serviço de localização não pode ser inicializado completamente.");
			}
		}

		/// <summary>
		/// Escaneia a pasta Resources/Locales para encontrar todos os arquivos JSON de idioma.
		/// </summary>
		private static void LoadAvailableLanguages()
		{
			string localesFolderPath = "Locales"; // Caminho relativo a Resources

			// Carrega todos os TextAssets da pasta "Locales"
			// Nota: Resources.LoadAll é necessário para escanear a pasta no Editor e em builds.
			// No Editor, Directory.GetFiles é mais robusto para a janela do editor,
			// mas em runtime, Resources.LoadAll é o método certo.
			TextAsset[] localeFiles = Resources.LoadAll<TextAsset>(localesFolderPath);

			if (localeFiles == null || localeFiles.Length == 0)
			{
				Debug.LogError($"Nenhum arquivo JSON encontrado em 'Resources/{localesFolderPath}'. Certifique-se de que seus arquivos de idioma estão lá.");
				_availableLanguages = new string[0];
				return;
			}

			// Extrai os nomes dos arquivos (que são os códigos dos idiomas)
			_availableLanguages = localeFiles.Select(ta => ta.name).ToArray();
			Debug.Log($"Idiomas disponíveis carregados: {string.Join(", ", _availableLanguages)}");
		}

		/// <summary>
		/// Define o idioma atual e carrega seus dados de localização.
		/// Dispara o evento OnLanguageChanged se o idioma for alterado com sucesso.
		/// </summary>
		/// <param name="languageCode">O código do idioma (ex: "en", "pt-br").</param>
		public static void SetLanguage(string languageCode)
		{
			if (_currentLanguageCode == languageCode && _currentLanguageTexts != null)
			{
				// Já estamos no idioma desejado e ele está carregado
				return;
			}

			// Verifica se o idioma é válido
			if (!_availableLanguages.Contains(languageCode))
			{
				Debug.LogWarning($"Idioma '{languageCode}' não encontrado entre os idiomas disponíveis. Não foi possível definir o idioma.");
				return;
			}

			// Carrega o arquivo JSON do idioma específico
			string jsonPath = $"Locales/{languageCode}";
			TextAsset jsonFile = Resources.Load<TextAsset>(jsonPath);

			if (jsonFile == null)
			{
				Debug.LogError($"Arquivo de idioma '{jsonPath}.json' não encontrado na pasta Resources. Verifique o nome do arquivo.");
				_currentLanguageTexts = null;
				return;
			}

			try
			{
				_currentLanguageTexts = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonFile.text);

				if (_currentLanguageTexts == null)
				{
					Debug.LogError($"Falha ao desserializar o arquivo de idioma '{languageCode}.json'. O JSON pode estar mal formatado.");
					return;
				}

				_currentLanguageCode = languageCode;
				PlayerPrefs.SetString("Localization.LastLanguage", languageCode); // Salva o idioma para a próxima sessão
				Debug.Log($"Idioma definido para '{languageCode}' e dados carregados.");
				OnLanguageChanged?.Invoke(); // Notifica os ouvintes que o idioma mudou
			}
			catch (Exception ex)
			{
				Debug.LogError($"Falha ao carregar ou desserializar o JSON para o idioma '{languageCode}': {ex.Message}");
				_currentLanguageTexts = null; // Limpa os dados em caso de erro
			}
		}

		/// <summary>
		/// Obtém o texto localizado para a chave fornecida no idioma atual.
		/// </summary>
		/// <param name="key">A chave de localização.</param>
		/// <returns>O texto traduzido ou uma string de erro se não for encontrado.</returns>
		public static string GetLocalizedText(string key)
		{
			if (_currentLanguageTexts == null)
			{
				Debug.LogError("LocalizationService não inicializado ou nenhum idioma carregado. Chame Init() ou SetLanguage() primeiro.");
				return $"[!{key}]"; // Indica que a chave não foi encontrada
			}

			if (_currentLanguageTexts.TryGetValue(key, out var value))
			{
				return value;
			}

			Debug.LogWarning($"Chave de localização '{key}' não encontrada para o idioma '{_currentLanguageCode}'.");
			return $"[!{key}]"; // Indica que a chave não foi encontrada
		}

		/// <summary>
		/// Retorna todas as chaves de localização disponíveis para o idioma atual.
		/// </summary>
		public static string[] GetAllKeys()
		{
			if (_currentLanguageTexts == null || _currentLanguageTexts.Count == 0)
				return new string[] { "(no keys for current language)" };

			return _currentLanguageTexts.Keys.ToArray();
		}

		/// <summary>
		/// Retorna uma lista de todos os códigos de idiomas disponíveis (baseado nos arquivos na pasta Resources/Locales).
		/// </summary>
		public static string[] GetAllLanguages()
		{
			// Se _availableLanguages ainda não foi carregado, tenta carregar
			if (_availableLanguages == null || _availableLanguages.Length == 0)
			{
				LoadAvailableLanguages();
			}
			return _availableLanguages;
		}
	}
}