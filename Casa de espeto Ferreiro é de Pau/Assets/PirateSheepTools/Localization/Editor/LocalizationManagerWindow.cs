using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json.Linq;

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
            LoadLanguagesFromJSON();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();

            DrawSectionHeader("📥 Update locales.json from Google Sheets (Apps Script)", ref showHelpDownload,
                "Downloads the JSON file from your Google Apps Script endpoint.\nThe JSON will be saved into Resources/locales.json.");

            googleJsonUrl = EditorGUILayout.TextField("JSON URL:", googleJsonUrl);

            if (GUILayout.Button("⬇️ Download and update locales.json"))
            {
                DownloadAndReplaceJSON(googleJsonUrl);
                LoadLanguagesFromJSON();
            }

            EditorGUILayout.Space(20);

            DrawSectionHeader("🌐 Apply language to current scene", ref showHelpApply,
                "Sets the selected language on all 'LocalizedText' components in the current scene.\nOnly affects the Editor view — not during runtime.");

            if (availableLanguages.Length == 0)
            {
                GUILayout.Label("No languages found in locales.json");
                if (GUILayout.Button("🔄 Reload languages"))
                {
                    LoadLanguagesFromJSON();
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

        private void LoadLanguagesFromJSON()
        {
            var textAsset = Resources.Load<TextAsset>("locales");
            if (textAsset == null)
            {
                Debug.LogError("File 'locales.json' not found in Resources folder.");
                availableLanguages = new string[0];
                return;
            }

            try
            {
                JObject json = JObject.Parse(textAsset.text);

                // Corrigido: pegar as keys do root JSON (idiomas)
                availableLanguages = json.Properties().Select(p => p.Name).ToArray();
            }
            catch (System.Exception e)
            {
                Debug.LogError("Failed to parse locales.json: " + e.Message);
                availableLanguages = new string[0];
            }
        }

        private void DownloadAndReplaceJSON(string url)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    string jsonContent = client.DownloadString(url);

                    string path = Path.Combine(Application.dataPath, "Resources/locales.json");
                    File.WriteAllText(path, jsonContent);

                    Debug.Log("✅ locales.json updated: " + path);
                    AssetDatabase.Refresh();
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error downloading JSON: " + ex.Message);
            }
        }

        private void ApplyLanguageToScene(string languageCode)
        {
            if (LocalizationService.GetAllLanguages().Length == 0)
            {
                LocalizationService.Init();
            }

            LocalizationService.SetLanguage(languageCode);

            var localizedTexts = GameObject.FindObjectsOfType<LocalizationText>(true);
            int count = 0;
            foreach (var lt in localizedTexts)
            {
                lt.UpdateText();
                count++;
            }

            Debug.Log($"Language '{languageCode}' applied. {count} texts updated.");

            SceneView.RepaintAll();
        }
    }
}
