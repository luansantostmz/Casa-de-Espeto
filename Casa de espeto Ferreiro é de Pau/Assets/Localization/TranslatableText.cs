using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class TranslatableText : MonoBehaviour
{
	public string translationID;  // ID do texto a ser traduzido

	private TMP_Text tmpText;

	void Awake()
	{
		tmpText = GetComponent<TMP_Text>();  // Usando TMP_Text ao invés de Text
	}

	// Método para atualizar o texto
	public void UpdateText(string translatedText)
	{
		tmpText.text = translatedText;
	}
}
