using UnityEngine;

public class InputManager : MonoBehaviour
{    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) )
        {
			GameEvents.Input.OnDown?.Invoke();
		}

		if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
		{
			GameEvents.Input.OnUp?.Invoke();
		}

		if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
		{
			GameEvents.Input.OnRight?.Invoke();
		}

		if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
		{
			GameEvents.Input.OnLeft?.Invoke();
		}
	}
}
