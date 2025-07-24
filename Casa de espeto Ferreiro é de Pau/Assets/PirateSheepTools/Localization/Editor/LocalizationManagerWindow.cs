using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Collections.Generic; // Adicionado para usar Dictionary

namespace PirateSheep.Localization
{
    public class LocalizationManagerWindow : EditorWindow
    {
        private string[] availableLanguages = new string[0];
        private string googleJsonUrl = "https://script.google.com/macros/s/SEU_ID_AQUI/exec";

        private bool showHelpDownload = false;
        private bool showHelpApply = false;

        [MenuItem("PirateSheep/Localization")]
        public static void ShowWindow()
        {
            GetWindow<LocalizationManagerWindow>("Localization Manager");
        }

        private void OnEnable()
        {
            LoadLanguagesFromJSONFiles(); // Alterado para carregar dos arquivos separados
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();

            DrawSectionHeader("📥 Update locales.json from Google Sheets (Apps Script)", ref showHelpDownload,
                "Downloads the JSON file from your Google Apps Script endpoint.\nEach language will be saved into a separate JSON file in Resources/Locales/.");

            googleJsonUrl = EditorGUILayout.TextField("JSON URL:", googleJsonUrl);

            if (GUILayout.Button("⬇️ Download and update locale files")) // Texto do botão alterado
            {
                DownloadAndSplitJSON(googleJsonUrl); // Chamada para a nova função
                LoadLanguagesFromJSONFiles(); // Recarrega dos arquivos
            }

            EditorGUILayout.Space(20);

            DrawSectionHeader("🌐 Apply language to current scene", ref showHelpApply,
                "Sets the selected language on all 'LocalizedText' components in the current scene.\nOnly affects the Editor view — not during runtime.");

            if (availableLanguages.Length == 0)
            {
                GUILayout.Label("No languages found in Resources/Locales folder."); // Texto alterado
                if (GUILayout.Button("🔄 Reload languages"))
                {
                    LoadLanguagesFromJSONFiles();
                }
                return;
            }

            foreach (string lang in availableLanguages)
            {
                if (GUILayout.Button($"🌍 {lang.ToUpper()}"))
                {
                    ApplyLanguageToScene(lang);
                }
            }
        }

        private void DrawSectionHeader(string title, ref bool showHelp, string tooltip)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(title, EditorStyles.boldLabel);
            if (GUILayout.Button("?", GUILayout.Width(20)))
            {
                showHelp = !showHelp;
            }
            EditorGUILayout.EndHorizontal();

            if (showHelp)
            {
                EditorGUILayout.HelpBox(tooltip, MessageType.Info);
            }
        }

        private void LoadLanguagesFromJSONFiles() // Função alterada para carregar dos arquivos separados
        {
            string localesFolderPath = Path.Combine(Application.dataPath, "Resources/Locales");
            if (!Directory.Exists(localesFolderPath))
            {
                Debug.LogWarning("Pasta 'Resources/Locales' não encontrada. Crie-a ou faça o download dos arquivos.");
                availableLanguages = new string[0];
                return;
            }

            // Pega todos os arquivos .json na pasta Locales e extrai o nome do arquivo (sem extensão)
            availableLanguages = Directory.GetFiles(localesFolderPath, "*.json")
                                    .Select(path => Path.GetFileNameWithoutExtension(path))
                                    .ToArray();

            // Certifica-se de que a lista de idiomas no LocalizationService está atualizada
            // Isso pode exigir uma alteração em LocalizationService para que ele carregue múltiplos arquivos
            // ou tenha um método para registrar os idiomas disponíveis.
            // Por simplicidade, assumimos que LocalizationService irá lidar com isso internamente
            // ao ser inicializado com os arquivos separados.
        }

        private void DownloadAndSplitJSON(string url) // Nova função para baixar e separar
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    string jsonContent = client.DownloadString(url);
                    JObject fullJson = JObject.Parse(jsonContent);

                    string localesFolderPath = Path.Combine(Application.dataPath, "Resources/Locales");
                    if (!Directory.Exists(localesFolderPath))
                    {
                        Directory.CreateDirectory(localesFolderPath);
                    }

                    // Limpa a pasta antes de baixar novos arquivos
                    foreach (string file in Directory.GetFiles(localesFolderPath, "*.json"))
                    {
                        File.Delete(file);
                    }

                    foreach (var property in fullJson.Properties())
                    {
                        string languageCode = property.Name;
                        JToken languageData = property.Value;

                        string languageFilePath = Path.Combine(localesFolderPath, $"{languageCode}.json");
                        File.WriteAllText(languageFilePath, languageData.ToString(Newtonsoft.Json.Formatting.Indented));
                        Debug.Log($"✅ Arquivo de idioma '{languageCode}.json' salvo em: {languageFilePath}");
                    }

                    Debug.Log("🎉 Download e separação de arquivos de idioma concluídos!");
                    AssetDatabase.Refresh();
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Erro ao baixar e separar JSON: " + ex.Message);
            }
        }

        private void ApplyLanguageToScene(string languageCode)
        {
            // O LocalizationService precisará ser ajustado para carregar um arquivo de idioma específico
            // de `Resources/Locales/{languageCode}.json`
            if (LocalizationService.GetAllLanguages().Length == 0)
            {
                LocalizationService.Init(); // Ainda precisa carregar os dados. Isso será um ponto de atenção.
            }

            LocalizationService.SetLanguage(languageCode);

            var localizedTexts = GameObject.FindObjectsOfType<LocalizationText>(true);
            int count = 0;
            foreach (var lt in localizedTexts)
            {
                lt.UpdateText();
                count++;
            }

            Debug.Log($"Idioma '{languageCode}' aplicado. {count} textos atualizados.");

            SceneView.RepaintAll();
        }
    }
}