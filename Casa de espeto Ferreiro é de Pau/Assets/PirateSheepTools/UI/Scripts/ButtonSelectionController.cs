using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSelectionController : MonoBehaviour
{
    [SerializeField] Color _selectedColor;
    [SerializeField] Color _defaultColor;
    [SerializeField] Button _startButton;

    [SerializeField] List<Button> _buttons = new List<Button>();

    void Start()
    {
        SelectButton(_startButton);
    }

    public void SelectButton(Button button)
    {
        foreach (var btn in _buttons)
        {
            btn.GetComponent<Image>().color = _defaultColor;
        }

        button.GetComponent<Image>().color = _selectedColor;
    }
}
