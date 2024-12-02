using System.Collections.Generic;
using UnityEngine;

public class UIStateOrchestrator
{
    public static List<UIState> ActiveStates = new List<UIState>();

    public static void ActiveState(UIState state)
    {
        ActiveStates.Add(state);

		foreach (var obj in GameObject.FindObjectsOfType<UIStateObject>(true))
        {
            if (obj.ReverseState == state)
				obj.gameObject.SetActive(false);

			if (obj.State == state)
            {
                obj.gameObject.SetActive(true);
            }
            else if (state.Group != null) 
            {
                if (state.Group.States.Contains(obj.State))
                {
                    obj.gameObject.SetActive(false);
                }
            }
        }
	}

	public static void DeactiveState(UIState state)
	{
		ActiveStates.Remove(state);

		foreach (var obj in GameObject.FindObjectsOfType<UIStateObject>(true))
		{
			if (obj.State == state)
			{
				obj.gameObject.SetActive(false);
			}
			else if (obj.ReverseState == state)
			{
				obj.gameObject.SetActive(true);
			}
		}
	}
}