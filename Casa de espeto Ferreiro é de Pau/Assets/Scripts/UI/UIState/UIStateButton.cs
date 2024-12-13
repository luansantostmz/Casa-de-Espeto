using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIStateButton : MonoBehaviour, IPointerDownHandler
{
	[SerializeField] bool _isDisabled;
    [SerializeField] UIState _activeOnClick;
    [SerializeField] UIState _deactiveOnClick;

	public void OnPointerDown(PointerEventData eventData)
	{
		if (_isDisabled)
			return;

		if (_activeOnClick != null)
			_activeOnClick.Activate();

		if (_deactiveOnClick != null)
			_deactiveOnClick.Deactivate();
	}

	public void SetEnable(bool disable)
	{
		_isDisabled = !disable;
	}
}
