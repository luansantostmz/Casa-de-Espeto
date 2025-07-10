using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CustomPropertyDrawer(typeof(LocalizationKeyAttribute))]
public class LocalizationKeyDrawer : PropertyDrawer
{
    private static volatile bool dataLoaded = false;
    private static readonly object dataLock = new object();

    private static string[] allKeys;
    private static string[] allLanguages;
    private static Dictionary<string, Dictionary<string, string>> localizationData;

    private string currentSearch = "";
    private string[] filteredKeysCache;

    private Vector2 dropdownScroll;

    private int currentPage = 0;
    private const int KeysPerPage = 10;

    private float LineHeight = EditorGUIUtility.singleLineHeight + 2;
    private const int MaxDropdownSuggestions = 10;
    private const float DropdownPadding = 2f;
    private const float TranslationsVerticalPadding = 5f;
    private float PaginationHeight = EditorGUIUtility.singleLineHeight + 5;

    private void EnsureDataLoaded()
    {
        if (dataLoaded) return;

        lock (dataLock)
        {
            if (dataLoaded) return;

            TextAsset csv = Resources.Load<TextAsset>("locales");

            if (csv == null)
            {
                Debug.LogWarning("Localization CSV (locales.csv) não encontrado em Resources. Certifique-se de que o arquivo está lá.");
                allKeys = new[] { "(Nenhuma chave encontrada - CSV missing)" };
                allLanguages = new string[0];
                localizationData = new Dictionary<string, Dictionary<string, string>>();
                dataLoaded = true;
                return;
            }

            var lines = csv.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length < 2)
            {
                Debug.LogWarning("Localization CSV (locales.csv) está vazio ou mal formatado.");
                allKeys = new[] { "(CSV vazio ou inválido)" };
                allLanguages = new string[0];
                localizationData = new Dictionary<string, Dictionary<string, string>>();
                dataLoaded = true;
                return;
            }

            var headers = lines[0].Split(',')
                                 .Select(h => h.Trim())
                                 .ToArray();
            allLanguages = headers.Skip(1).ToArray();

            localizationData = new Dictionary<string, Dictionary<string, string>>();
            HashSet<string> keysSet = new HashSet<string>();

            for (int i = 1; i < lines.Length; i++)
            {
                var cols = lines[i].Split(',')
                                  .Select(c => c.Trim())
                                  .ToArray();

                if (cols.Length < 2)
                {
                    Debug.LogWarning($"Linha {i + 1} do CSV de localização inválida: '{lines[i]}'. Pulando.");
                    continue;
                }

                string key = cols[0];
                if (string.IsNullOrEmpty(key))
                {
                    Debug.LogWarning($"Linha {i + 1} do CSV de localização tem uma chave vazia. Pulando.");
                    continue;
                }

                if (!keysSet.Add(key))
                {
                    Debug.LogWarning($"Chave de localização duplicada encontrada: '{key}' na linha {i + 1}. A primeira ocorrência será usada.");
                }

                for (int langIndex = 1; langIndex < headers.Length; langIndex++)
                {
                    string lang = headers[langIndex];
                    string val = langIndex < cols.Length ? cols[langIndex] : "";

                    if (!localizationData.ContainsKey(lang))
                        localizationData[lang] = new Dictionary<string, string>();

                    localizationData[lang][key] = val;
                }
            }

            allKeys = keysSet.OrderBy(k => k).ToArray();
            dataLoaded = true;
        }
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
                filteredQuery = allKeys
                    .Where(k => k.ToLower().Contains(currentSearch.ToLower()));
            }

            filteredKeysCache = filteredQuery
                                .Skip(currentPage * KeysPerPage)
                                .Take(KeysPerPage)
                                .ToArray();
        }

        if (willShowDropdown)
        {
            float dropdownContentHeight = LineHeight * MaxDropdownSuggestions;
            if (filteredKeysCache != null && filteredKeysCache.Length < MaxDropdownSuggestions)
            {
                dropdownContentHeight = LineHeight * filteredKeysCache.Length;
            }

            bool hasPagination = string.IsNullOrEmpty(currentSearch) && (allKeys.Length > KeysPerPage);

            float totalDropdownAndPaginationHeight = dropdownContentHeight + DropdownPadding * 2;
            if (hasPagination)
            {
                totalDropdownAndPaginationHeight += PaginationHeight;
            }

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

                Rect prevButtonRect = new Rect(
                    paginationRect.x,
                    paginationRect.y,
                    buttonWidth,
                    paginationRect.height
                );

                Rect labelRect = new Rect(
                    prevButtonRect.xMax + spacing,
                    paginationRect.y,
                    labelWidth,
                    paginationRect.height
                );

                Rect nextButtonRect = new Rect(
                    labelRect.xMax + spacing,
                    paginationRect.y,
                    buttonWidth,
                    paginationRect.height
                );

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

        float actualDropdownAndPaginationHeight = 0;
        if (willShowDropdown)
        {
            float contentHeight = LineHeight * MaxDropdownSuggestions;
            if (filteredKeysCache != null && filteredKeysCache.Length < MaxDropdownSuggestions)
            {
                contentHeight = LineHeight * filteredKeysCache.Length;
            }

            actualDropdownAndPaginationHeight += contentHeight + DropdownPadding * 2;

            if (string.IsNullOrEmpty(currentSearch) && (allKeys.Length > KeysPerPage))
            {
                actualDropdownAndPaginationHeight += PaginationHeight;
            }
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
                tempFilteredQuery = allKeys
                    .Where(k => k.ToLower().Contains(property.stringValue.ToLower()));
            }

            string[] tempFilteredKeys = tempFilteredQuery
                                        .Skip(currentPage * KeysPerPage)
                                        .Take(KeysPerPage)
                                        .ToArray();

            float dropdownContentHeight = LineHeight * MaxDropdownSuggestions;
            if (tempFilteredKeys.Length < MaxDropdownSuggestions)
            {
                dropdownContentHeight = LineHeight * tempFilteredKeys.Length;
            }

            if (tempFilteredKeys.Length > 0)
            {
                height += dropdownContentHeight + DropdownPadding * 2;

                bool hasPagination = string.IsNullOrEmpty(property.stringValue) && (allKeys.Length > KeysPerPage);
                if (hasPagination)
                {
                    height += PaginationHeight;
                }
            }
        }

        height += EditorGUIUtility.singleLineHeight * allLanguages.Length;
        height += TranslationsVerticalPadding;

        return height;
    }

    [InitializeOnLoadMethod]
    private static void ClearCacheOnReload()
    {
        localizationData = null;
        allKeys = null;
        allLanguages = null;
        dataLoaded = false;
    }
}