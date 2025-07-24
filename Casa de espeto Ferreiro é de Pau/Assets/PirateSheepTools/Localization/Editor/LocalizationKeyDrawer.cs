using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace PirateSheep.Localization
{
    [CustomPropertyDrawer(typeof(LocalizationKeyAttribute))]
    public class LocalizationKeyDrawer : PropertyDrawer
    {
        private static volatile bool dataLoaded = false;
        private static readonly object dataLock = new object();

        private static string[] allKeys = new string[0];
        private static string[] allLanguages = new string[0];
        private static Dictionary<string, Dictionary<string, string>> localizationData = new Dictionary<string, Dictionary<string, string>>();

        private string currentSearch = "";
        private string[] filteredKeysCache = null;

        private Vector2 dropdownScroll;

        private int currentPage = 0;
        private const int KeysPerPage = 10;

        private float LineHeight => EditorGUIUtility.singleLineHeight + 2;
        private const int MaxDropdownSuggestions = 10;
        private const float DropdownPadding = 2f;
        private const float TranslationsVerticalPadding = 5f;
        private float PaginationHeight => EditorGUIUtility.singleLineHeight + 5;

        private void EnsureDataLoaded()
        {
            if (dataLoaded) return;

            lock (dataLock)
            {
                if (dataLoaded) return;

                TextAsset jsonAsset = Resources.Load<TextAsset>("locales"); // espera o arquivo locales.json dentro de Resources
                if (jsonAsset == null)
                {
                    Debug.LogWarning("Localization JSON (locales.json) não encontrado em Resources. Certifique-se que o arquivo está lá.");
                    allKeys = new[] { "(Nenhuma chave encontrada - JSON missing)" };
                    allLanguages = new string[0];
                    localizationData = new Dictionary<string, Dictionary<string, string>>();
                    dataLoaded = true;
                    return;
                }

                try
                {
                    var root = JObject.Parse(jsonAsset.text);

                    localizationData = new Dictionary<string, Dictionary<string, string>>();
                    HashSet<string> keysSet = new HashSet<string>();

                    foreach (var langProperty in root.Properties())
                    {
                        string lang = langProperty.Name;
                        var langDict = new Dictionary<string, string>();

                        JObject translations = langProperty.Value as JObject;
                        if (translations == null)
                            continue;

                        foreach (var keyProperty in translations.Properties())
                        {
                            string key = keyProperty.Name;
                            string val = keyProperty.Value.ToString();

                            langDict[key] = val;
                            keysSet.Add(key);
                        }

                        localizationData[lang] = langDict;
                    }

                    allLanguages = localizationData.Keys.OrderBy(l => l).ToArray();
                    allKeys = keysSet.OrderBy(k => k).ToArray();
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Erro ao parsear JSON de localização: " + e.Message);
                    allKeys = new[] { "(Erro ao parsear JSON)" };
                    allLanguages = new string[0];
                    localizationData = new Dictionary<string, Dictionary<string, string>>();
                }

                dataLoaded = true;
            }
        }

        // Função que faz split CSV respeitando aspas (ex: "a,b",c -> ["a,b", "c"])
        private static string[] SplitCsvLine(string line)
        {
            var pattern = @"
                # Match one value in valid CSV string.
                (?!\s*$)                                      # Don't match empty last value.
                \s*                                           # Strip whitespace.
                (?:                                           # Group for value alternatives.
                  '(?<val>(?:[^']|'')*)'                      # Single quoted string.
                | ""(?<val>(?:[^""]|"""")*)""                 # Double quoted string.
                | (?<val>[^,'""]*)                            # Non-comma, non-quote stuff.
                )                                             # End group of value alternatives.
                \s*                                           # Strip whitespace.
                (?:,|$)                                       # Field ends on comma or EOS.
                ";

            var regex = new Regex(pattern, RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            var matches = regex.Matches(line);

            return matches.Cast<Match>().Select(m => m.Groups["val"].Value.Replace("''", "'").Replace("\"\"", "\"")).ToArray();
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EnsureDataLoaded();

            EditorGUI.BeginProperty(position, label, property);

            string controlName = property.propertyPath;
            GUI.SetNextControlName(controlName);

            Rect textFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.BeginChangeCheck();
            string currentPropertyValue = property.stringValue;
            string newPropertyValue = EditorGUI.TextField(textFieldRect, label, currentPropertyValue);
            if (EditorGUI.EndChangeCheck())
            {
                property.stringValue = newPropertyValue;
                currentSearch = newPropertyValue;
                filteredKeysCache = null;
                currentPage = 0;
            }

            bool focused = GUI.GetNameOfFocusedControl() == controlName;
            bool willShowDropdown = focused;

            if (willShowDropdown && (filteredKeysCache == null || currentSearch != property.stringValue))
            {
                currentSearch = property.stringValue;

                IEnumerable<string> filteredQuery;
                if (string.IsNullOrEmpty(currentSearch))
                {
                    filteredQuery = allKeys;
                }
                else
                {
                    filteredQuery = allKeys.Where(k => k.IndexOf(currentSearch, System.StringComparison.OrdinalIgnoreCase) >= 0);
                }

                filteredKeysCache = filteredQuery.Skip(currentPage * KeysPerPage).Take(KeysPerPage).ToArray();
            }

            if (willShowDropdown)
            {
                float dropdownContentHeight = LineHeight * MaxDropdownSuggestions;
                if (filteredKeysCache != null && filteredKeysCache.Length < MaxDropdownSuggestions)
                    dropdownContentHeight = LineHeight * filteredKeysCache.Length;

                bool hasPagination = string.IsNullOrEmpty(currentSearch) && (allKeys.Length > KeysPerPage);

                float totalDropdownAndPaginationHeight = dropdownContentHeight + DropdownPadding * 2;
                if (hasPagination) totalDropdownAndPaginationHeight += PaginationHeight;

                Rect dropdownOverallRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + DropdownPadding, position.width, totalDropdownAndPaginationHeight);
                GUI.Box(dropdownOverallRect, GUIContent.none, EditorStyles.helpBox);

                Rect scrollViewRect = new Rect(
                    dropdownOverallRect.x + DropdownPadding,
                    dropdownOverallRect.y + DropdownPadding,
                    dropdownOverallRect.width - DropdownPadding * 2,
                    dropdownContentHeight
                );
                Rect scrollContentRect = new Rect(0, 0, scrollViewRect.width - 20, LineHeight * (filteredKeysCache?.Length ?? 0));

                dropdownScroll = GUI.BeginScrollView(scrollViewRect, dropdownScroll, scrollContentRect);

                if (filteredKeysCache != null)
                {
                    for (int i = 0; i < filteredKeysCache.Length; i++)
                    {
                        Rect itemRect = new Rect(0, i * LineHeight, scrollContentRect.width, LineHeight);
                        if (GUI.Button(itemRect, filteredKeysCache[i], EditorStyles.label))
                        {
                            property.stringValue = filteredKeysCache[i];
                            currentSearch = filteredKeysCache[i];
                            filteredKeysCache = null;

                            GUI.FocusControl(null);
                            GUI.changed = true;
                            Event.current.Use();
                            break;
                        }

                        if (itemRect.Contains(Event.current.mousePosition) && Event.current.type == EventType.Repaint)
                        {
                            EditorGUI.DrawRect(itemRect, new Color(0.2f, 0.4f, 0.6f, 0.3f));
                        }
                    }
                }
                GUI.EndScrollView();

                if (hasPagination)
                {
                    Rect paginationRect = new Rect(
                        dropdownOverallRect.x + DropdownPadding,
                        scrollViewRect.y + scrollViewRect.height + DropdownPadding,
                        dropdownOverallRect.width - DropdownPadding * 2,
                        PaginationHeight
                    );

                    float buttonWidth = 70f;
                    float labelWidth = 100f;
                    float spacing = 10f;

                    Rect prevButtonRect = new Rect(paginationRect.x, paginationRect.y, buttonWidth, paginationRect.height);
                    Rect labelRect = new Rect(prevButtonRect.xMax + spacing, paginationRect.y, labelWidth, paginationRect.height);
                    Rect nextButtonRect = new Rect(labelRect.xMax + spacing, paginationRect.y, buttonWidth, paginationRect.height);

                    int totalPages = Mathf.CeilToInt((float)allKeys.Length / KeysPerPage);

                    GUI.enabled = currentPage > 0;
                    if (GUI.Button(prevButtonRect, "<"))
                    {
                        currentPage--;
                        filteredKeysCache = null;
                        dropdownScroll = Vector2.zero;
                        GUI.changed = true;
                    }
                    GUI.enabled = true;

                    GUI.Label(labelRect, $"< {currentPage + 1}/{totalPages} >", EditorStyles.centeredGreyMiniLabel);

                    GUI.enabled = currentPage < totalPages - 1;
                    if (GUI.Button(nextButtonRect, ">"))
                    {
                        currentPage++;
                        filteredKeysCache = null;
                        dropdownScroll = Vector2.zero;
                        GUI.changed = true;
                    }
                    GUI.enabled = true;
                }
            }

            // Mostrar traduções do key selecionado
            float actualDropdownAndPaginationHeight = 0;
            if (willShowDropdown)
            {
                float contentHeight = LineHeight * MaxDropdownSuggestions;
                if (filteredKeysCache != null && filteredKeysCache.Length < MaxDropdownSuggestions)
                    contentHeight = LineHeight * filteredKeysCache.Length;

                actualDropdownAndPaginationHeight += contentHeight + DropdownPadding * 2;

                if (string.IsNullOrEmpty(currentSearch) && (allKeys.Length > KeysPerPage))
                    actualDropdownAndPaginationHeight += PaginationHeight;
            }

            float translationsStartY = position.y + EditorGUIUtility.singleLineHeight + actualDropdownAndPaginationHeight + TranslationsVerticalPadding;
            float translationsHeight = EditorGUIUtility.singleLineHeight * allLanguages.Length;
            Rect translationsRect = new Rect(position.x, translationsStartY, position.width, translationsHeight);

            GUI.BeginGroup(translationsRect);
            float currentLineY = 0;
            string currentKey = property.stringValue;

            if (string.IsNullOrEmpty(currentKey))
            {
                EditorGUI.LabelField(new Rect(0, currentLineY, translationsRect.width, EditorGUIUtility.singleLineHeight), "Nenhuma chave selecionada.");
            }
            else
            {
                foreach (var lang in allLanguages)
                {
                    string val = "N/A";
                    if (localizationData.TryGetValue(lang, out var langDict) && langDict.TryGetValue(currentKey, out var translatedValue))
                    {
                        val = translatedValue;
                    }
                    EditorGUI.LabelField(new Rect(0, currentLineY, translationsRect.width, EditorGUIUtility.singleLineHeight), $"{lang}: {val}");
                    currentLineY += EditorGUIUtility.singleLineHeight;
                }
            }
            GUI.EndGroup();

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            EnsureDataLoaded();

            float height = EditorGUIUtility.singleLineHeight;

            string controlName = property.propertyPath;
            bool focused = GUI.GetNameOfFocusedControl() == controlName;
            bool willShowDropdown = focused;

            if (willShowDropdown)
            {
                IEnumerable<string> tempFilteredQuery;
                if (string.IsNullOrEmpty(property.stringValue))
                {
                    tempFilteredQuery = allKeys;
                }
                else
                {
                    tempFilteredQuery = allKeys.Where(k => k.IndexOf(property.stringValue, System.StringComparison.OrdinalIgnoreCase) >= 0);
                }

                string[] tempFilteredKeys = tempFilteredQuery.Skip(currentPage * KeysPerPage).Take(KeysPerPage).ToArray();

                float dropdownContentHeight = LineHeight * MaxDropdownSuggestions;
                if (tempFilteredKeys.Length < MaxDropdownSuggestions)
                    dropdownContentHeight = LineHeight * tempFilteredKeys.Length;

                if (tempFilteredKeys.Length > 0)
                {
                    height += dropdownContentHeight + DropdownPadding * 2;

                    bool hasPagination = string.IsNullOrEmpty(property.stringValue) && (allKeys.Length > KeysPerPage);
                    if (hasPagination)
                        height += PaginationHeight;
                }
            }

            height += EditorGUIUtility.singleLineHeight * allLanguages.Length;
            height += TranslationsVerticalPadding;

            return height;
        }

        [InitializeOnLoadMethod]
        private static void ClearCacheOnReload()
        {
            localizationData = new Dictionary<string, Dictionary<string, string>>();
            allKeys = new string[0];
            allLanguages = new string[0];
            dataLoaded = false;
        }
    }
}
