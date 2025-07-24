using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions; // Mantido, embora SplitCsvLine não seja mais usado
using Newtonsoft.Json.Linq; // Usado para parsing de JSON

namespace PirateSheep.Localization
{
    [CustomPropertyDrawer(typeof(LocalizationKeyAttribute))]
    public class LocalizationKeyDrawer : PropertyDrawer
    {
        private static volatile bool dataLoaded = false;
        private static readonly object dataLock = new object();

        private static string[] allKeys = new string[0];
        private static string[] allLanguages = new string[0];
        // Agora, este dicionário armazenará TODOS os dados de localização (idioma -> chave -> valor)
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

        // Caminho relativo à pasta Resources onde os arquivos de idioma estão
        private const string LocalesFolderPath = "locales";

        /// <summary>
        /// Garante que todos os dados de localização são carregados a partir dos arquivos JSON separados.
        /// </summary>
        private void EnsureDataLoaded()
        {
            if (dataLoaded) return;

            lock (dataLock)
            {
                if (dataLoaded) return; // Checagem dupla para thread safety

                // Reseta os dados antes de carregar
                localizationData.Clear();
                HashSet<string> keysSet = new HashSet<string>();
                List<string> languagesList = new List<string>();

                // Carrega todos os TextAssets da pasta "Resources/locales"
                TextAsset[] localeFiles = Resources.LoadAll<TextAsset>(LocalesFolderPath);

                if (localeFiles == null || localeFiles.Length == 0)
                {
                    Debug.LogWarning($"Nenhum arquivo JSON de idioma encontrado em Resources/{LocalesFolderPath}. Certifique-se que os arquivos estão lá.");
                    allKeys = new[] { "(Nenhuma chave encontrada - Arquivos JSON missing)" };
                    allLanguages = new string[0];
                    dataLoaded = true;
                    return;
                }

                foreach (TextAsset jsonAsset in localeFiles)
                {
                    string languageCode = jsonAsset.name; // O nome do arquivo é o código do idioma (ex: "en", "pt-br")
                    languagesList.Add(languageCode);

                    try
                    {
                        // Parseia o JSON do arquivo de idioma atual
                        JObject translations = JObject.Parse(jsonAsset.text);
                        var langDict = new Dictionary<string, string>();

                        foreach (var keyProperty in translations.Properties())
                        {
                            string key = keyProperty.Name;
                            string val = keyProperty.Value.ToString();

                            langDict[key] = val;
                            keysSet.Add(key); // Adiciona a chave ao conjunto de todas as chaves
                        }
                        localizationData[languageCode] = langDict;
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"Erro ao parsear o arquivo JSON de idioma '{languageCode}.json': {e.Message}");
                    }
                }

                allLanguages = languagesList.OrderBy(l => l).ToArray();
                allKeys = keysSet.OrderBy(k => k).ToArray();

                if (allKeys.Length == 0 && allLanguages.Length > 0)
                {
                    Debug.LogWarning("Nenhuma chave de localização encontrada em nenhum dos arquivos de idioma carregados.");
                    allKeys = new[] { "(Nenhuma chave encontrada em idiomas carregados)" };
                }
                else if (allKeys.Length == 0 && allLanguages.Length == 0)
                {
                    allKeys = new[] { "(Nenhum idioma ou chave carregada)" };
                }


                dataLoaded = true;
                Debug.Log($"LocalizationKeyDrawer: Carregados {allLanguages.Length} idiomas e {allKeys.Length} chaves únicas.");
            }
        }

        // Removido SplitCsvLine, pois não é mais relevante para o formato JSON.
        // Se a funcionalidade fosse para ler CSV, ela ficaria aqui.

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
                filteredKeysCache = null; // Invalida o cache para recalcular
                currentPage = 0; // Reseta a página ao mudar a pesquisa
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
                    // Filtra as chaves com base na pesquisa atual
                    filteredQuery = allKeys.Where(k => k.IndexOf(currentSearch, System.StringComparison.OrdinalIgnoreCase) >= 0);
                }

                // Aplica paginação diretamente aqui
                filteredKeysCache = filteredQuery.Skip(currentPage * KeysPerPage).Take(KeysPerPage).ToArray();
            }

            if (willShowDropdown)
            {
                float dropdownContentHeight = LineHeight * MaxDropdownSuggestions;
                if (filteredKeysCache != null && filteredKeysCache.Length < MaxDropdownSuggestions)
                    dropdownContentHeight = LineHeight * filteredKeysCache.Length;

                // A paginação só aparece se não houver pesquisa e o número de chaves for maior que KeysPerPage
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
                        // Estilo para o item de sugestão
                        GUIStyle itemStyle = new GUIStyle(EditorStyles.label);
                        itemStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black; // Cor de texto padrão
                        if (filteredKeysCache[i] == property.stringValue) // Destaca a chave atualmente selecionada
                        {
                            EditorGUI.DrawRect(itemRect, new Color(0.2f, 0.4f, 0.6f, 0.5f)); // Cor de destaque
                            itemStyle.normal.textColor = Color.white; // Texto branco para contraste
                        }
                        else if (itemRect.Contains(Event.current.mousePosition)) // Destaque ao passar o mouse
                        {
                            EditorGUI.DrawRect(itemRect, new Color(0.2f, 0.4f, 0.6f, 0.3f));
                        }

                        if (GUI.Button(itemRect, filteredKeysCache[i], itemStyle))
                        {
                            property.stringValue = filteredKeysCache[i];
                            currentSearch = filteredKeysCache[i];
                            filteredKeysCache = null; // Limpa o cache para que ele seja recomputado na próxima vez

                            GUI.FocusControl(null); // Tira o foco do campo de texto
                            GUI.changed = true; // Força uma atualização do Editor
                            Event.current.Use(); // Consome o evento
                            break;
                        }
                    }
                }
                GUI.EndScrollView();

                // Desenha a paginação se aplicável
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
                        filteredKeysCache = null; // Invalida o cache
                        dropdownScroll = Vector2.zero; // Reseta o scroll
                        GUI.changed = true;
                    }
                    GUI.enabled = true;

                    GUI.Label(labelRect, $"Página {currentPage + 1}/{totalPages}", EditorStyles.centeredGreyMiniLabel);

                    GUI.enabled = currentPage < totalPages - 1;
                    if (GUI.Button(nextButtonRect, ">"))
                    {
                        currentPage++;
                        filteredKeysCache = null; // Invalida o cache
                        dropdownScroll = Vector2.zero; // Reseta o scroll
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

            // Altura das traduções: um linha para cada idioma, mais uma linha se não houver chave selecionada
            float translationsContentHeight = string.IsNullOrEmpty(property.stringValue) ? EditorGUIUtility.singleLineHeight : (EditorGUIUtility.singleLineHeight * allLanguages.Length);

            Rect translationsRect = new Rect(position.x, translationsStartY, position.width, translationsContentHeight);

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

            float height = EditorGUIUtility.singleLineHeight; // Altura do campo de texto principal

            string controlName = property.propertyPath;
            bool focused = GUI.GetNameOfFocusedControl() == controlName;
            bool willShowDropdown = focused;

            if (willShowDropdown)
            {
                // Calcula a altura do dropdown para estimar o layout
                IEnumerable<string> tempFilteredQuery;
                if (string.IsNullOrEmpty(property.stringValue))
                {
                    tempFilteredQuery = allKeys;
                }
                else
                {
                    tempFilteredQuery = allKeys.Where(k => k.IndexOf(property.stringValue, System.StringComparison.OrdinalIgnoreCase) >= 0);
                }

                // Considera a paginação para o cálculo da altura
                string[] tempFilteredKeys = tempFilteredQuery.Skip(currentPage * KeysPerPage).Take(KeysPerPage).ToArray();

                float dropdownContentHeight = LineHeight * MaxDropdownSuggestions;
                if (tempFilteredKeys.Length < MaxDropdownSuggestions)
                    dropdownContentHeight = LineHeight * tempFilteredKeys.Length;

                if (tempFilteredKeys.Length > 0) // Só adiciona o dropdown se houver sugestões
                {
                    height += dropdownContentHeight + DropdownPadding * 2;

                    bool hasPagination = string.IsNullOrEmpty(property.stringValue) && (allKeys.Length > KeysPerPage);
                    if (hasPagination)
                        height += PaginationHeight;
                }
            }

            // Altura das traduções da chave selecionada
            if (string.IsNullOrEmpty(property.stringValue))
            {
                height += EditorGUIUtility.singleLineHeight; // "Nenhuma chave selecionada."
            }
            else
            {
                height += EditorGUIUtility.singleLineHeight * allLanguages.Length;
            }
            height += TranslationsVerticalPadding; // Padding entre o campo e as traduções

            return height;
        }

        /// <summary>
        /// Limpa o cache de dados de localização quando o Unity é recarregado (ex: script compilado).
        /// Isso garante que os dados mais recentes sejam carregados.
        /// </summary>
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