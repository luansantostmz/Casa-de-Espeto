using UnityEngine;
using TMPro;

namespace PirateSheep.Localization
{
    [RequireComponent(typeof(TMP_Text))]
    public class LocalizedText : MonoBehaviour
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
        }
    }
}