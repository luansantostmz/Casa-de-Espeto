using UnityEditor;
using UnityEngine;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

public class LocalizationUpdater : EditorWindow
{
    private string googleSheetsUrl = "https://docs.google.com/spreadsheets/d/1T0ofHNGO1M_zu5dARDv7RQf-Q35tLS46QgFO8gP9Sic/edit?usp=sharing";

    [MenuItem("PirateTools/Localization/Update Locales.csv")]
    public static void ShowWindow()
    {
        GetWindow<LocalizationUpdater>("Localization Updater");
    }

    private void OnGUI()
    {
        GUILayout.Label("Download locales.csv from Google Sheets", EditorStyles.boldLabel);
        googleSheetsUrl = EditorGUILayout.TextField("URL da planilha:", googleSheetsUrl);

        if (GUILayout.Button("Update locales.csv"))
        {
            string csvUrl = ConvertToCsvUrl(googleSheetsUrl);
            if (string.IsNullOrEmpty(csvUrl))
            {
                Debug.LogError("Invalid URL");
                return;
            }

            DownloadAndReplaceCSV(csvUrl);
        }
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

                Debug.Log("locales.csv updated: " + path);
                AssetDatabase.Refresh();
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error on download CSV: " + ex.Message);
        }
    }
}
