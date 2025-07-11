using UnityEngine;
using UnityEngine.UI;

namespace PirateSheep.Localization
{
    [RequireComponent(typeof(Button))]
    public class SwitchLanguageButton : BaseButton
    {
        [SerializeField] string _langCode;

        public void SetLanguage(string languageCode)
        {
            LocalizationService.SetLanguage(languageCode);
        }

        protected override void OnClick()
        {
            base.OnClick();
            SetLanguage(_langCode);
        }
    }
}