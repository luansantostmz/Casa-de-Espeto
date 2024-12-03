using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BaseButton : MonoBehaviour
{
    protected Button _button;

    protected virtual void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClick);
    }

    protected virtual void OnDestroy()
    {
        _button.onClick.RemoveListener(OnClick);
    }

    protected virtual void OnClick()
    {

    }
}
