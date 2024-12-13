using TMPro;
using UnityEngine;

public class VersionText : MonoBehaviour
{
    TMP_Text _text;

    private void Awake()
    {
        GetComponent<TMP_Text>().text = $"v{Application.version}";
    }
}
