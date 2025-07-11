using UnityEngine;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PirateSheep.Localization
{
    [RequireComponent(typeof(TMP_Text))]
    public class LocalizationText : MonoBehaviour
    {
        [LocalizationKey]
        public string localizationKey;

        private TMP_Text _text;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
        }

        private void Start()
        {
            UpdateText();
        }

        private void OnEnable()
        {
            LocalizationService.OnLanguageChanged += UpdateText;
            UpdateText();
        }

        private void OnDisable()
        {
            LocalizationService.OnLanguageChanged -= UpdateText;
        }

        public void SetKey(string newKey)
        {
            localizationKey = newKey;
            UpdateText();
        }

        public void UpdateText()
        {
            if (_text == null) _text = GetComponent<TMP_Text>();

            if (!string.IsNullOrEmpty(localizationKey))
            {
                _text.text = LocalizationService.GetLocalizedText(localizationKey);
            }
            else
            {
                _text.text = "";
            }

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                // Marca a cena como modificada e força repaint
                EditorUtility.SetDirty(this);
                EditorApplication.QueuePlayerLoopUpdate(); // força atualização da UI
                SceneView.RepaintAll();
            }
#endif
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Atualiza texto ao mudar a key no editor
            if (LocalizationService.GetAllLanguages().Length == 0)
            {
                LocalizationService.Init();
            }

            UpdateText();
        }
#endif
    }
}
