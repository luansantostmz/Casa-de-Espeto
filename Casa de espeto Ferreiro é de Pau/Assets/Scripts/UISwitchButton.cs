using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public enum DirectionType
{
	Up,
	Down,
	Left,
	Right
}

public class UISwitchButton : MonoBehaviour
{
	public DirectionType Direction;

	Button Button;

	private void OnEnable()
	{
		switch (Direction)
		{
			case DirectionType.Up:
				GameEvents.Input.OnUp += Move;
				break;
			case DirectionType.Down:
				GameEvents.Input.OnDown += Move;
				break;
			case DirectionType.Left:
				GameEvents.Input.OnLeft += Move;
				break;
			case DirectionType.Right:
				GameEvents.Input.OnRight += Move;
				break;
		}
	}
	private void OnDisable()
	{
		switch (Direction)
		{
			case DirectionType.Up:
				GameEvents.Input.OnUp -= Move;
				break;
			case DirectionType.Down:
				GameEvents.Input.OnDown -= Move;
				break;
			case DirectionType.Left:
				GameEvents.Input.OnLeft -= Move;
				break;
			case DirectionType.Right:
				GameEvents.Input.OnRight -= Move;
				break;
		}
	}
	private void Start()
	{		
		Button = GetComponent<Button>();		
	}

	void Move() 
	{
		if (!IsOnScreen()) return;

		switch (Direction)
		{
			case DirectionType.Up:
				Button.onClick?.Invoke();
				break;
			case DirectionType.Down:
				Button.onClick?.Invoke();
				break;
			case DirectionType.Left:
				Button.onClick?.Invoke();
				break;
			case DirectionType.Right:
				Button.onClick?.Invoke();
				break;
		}
	}

	private bool IsOnScreen()
	{
		Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);

		if (screenPosition.x > 0 && screenPosition.x < Screen.width &&
			screenPosition.y > 0 && screenPosition.y < Screen.height)
		{
			return true;
		}		
		return false;
		
	}
}

