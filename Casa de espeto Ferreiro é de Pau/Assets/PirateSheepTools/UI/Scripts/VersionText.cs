using TMPro;
using UnityEngine;

namespace PirateSheep.UI
{
    public class VersionText : MonoBehaviour
    {
        TMP_Text _text;

        private void Awake()
        {
            GetComponent<TMP_Text>().text = $"v{Application.version}";
        }
    }
}