using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace PirateSheep.Localization
{
    public class LocalizationManagerWindow : EditorWindow
    {
        private string[] availableLanguages = new string[0];
        private string googleSheetsUrl = "https://docs.google.com/spreadsheets/d/1T0ofHNGO1M_zu5dARDv7RQf-Q35tLS46QgFO8gP9Sic/edit?usp=sharing";

        private bool showHelpDownload = false;
        private bool showHelpApply = false;

        [MenuItem("PirateSheep/Localization")]
        public static void ShowWindow()
        {
            GetWindow<LocalizationManagerWindow>("Localization Manager");
        }

        private void OnEnable()
        {
            LoadLanguagesFromCSV();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();

            DrawSectionHeader("📥 Update locales.csv from Google Sheets", ref showHelpDownload,
                "Downloads the CSV file from your shared Google Sheet.\nMake sure the spreadsheet is public or shared properly.\nThe CSV will be saved into Resources/locales.csv.");

            googleSheetsUrl = EditorGUILayout.TextField("Spreadsheet URL:", googleSheetsUrl);

            if (GUILayout.Button("⬇️ Download and update locales.csv"))
            {
                string csvUrl = ConvertToCsvUrl(googleSheetsUrl);
                if (string.IsNullOrEmpty(csvUrl))
                {
                    Debug.LogError("Invalid URL.");
                    return;
                }

                DownloadAndReplaceCSV(csvUrl);
                LoadLanguagesFromCSV();
            }

            EditorGUILayout.Space(20);

            DrawSectionHeader("🌐 Apply language to current scene", ref showHelpApply,
                "Sets the selected language on all 'LocalizedText' components in the current scene.\nOnly affects the Editor view — not during runtime.");

            if (availableLanguages.Length == 0)
            {
                GUILayout.Label("No languages found in locales.csv");
                if (GUILayout.Button("🔄 Reload languages"))
                {
                    LoadLanguagesFromCSV();
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

        private void LoadLanguagesFromCSV()
        {
            var textAsset = Resources.Load<TextAsset>("locales");
            if (textAsset == null)
            {
                Debug.LogError("File 'locales.csv' not found in Resources folder.");
                availableLanguages = new string[0];
                return;
            }

            string[] lines = textAsset.text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length == 0)
            {
                Debug.LogError("locales.csv is empty.");
                availableLanguages = new string[0];
                return;
            }

            string[] headers = lines[0].Split(',');
            availableLanguages = headers.Skip(1).Select(h => h.Trim()).ToArray();
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

        private string ConvertToCsvUrl(string fullUrl)
        {
            var match = Regex.Match(fullUrl, @"https:\/\/docs\.google\.com\/spreadsheets\/d\/([a-zA-Z0-9-_]+)");

            if (match.Success && match.Groups.Count > 1)
            {
                string sheetId = match.Groups[1].Value;
                return $"https://docs.google.com/spreadsheets/d/{sheetId}/export?format=csv";
            }

            return null;
        }

        private void DownloadAndReplaceCSV(string url)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    string csvContent = client.DownloadString(url);

                    string path = Path.Combine(Application.dataPath, "Resources/locales.csv");
                    File.WriteAllText(path, csvContent);

                    Debug.Log("✅ locales.csv updated: " + path);
                    AssetDatabase.Refresh();
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error downloading CSV: " + ex.Message);
            }
        }
    }
}
