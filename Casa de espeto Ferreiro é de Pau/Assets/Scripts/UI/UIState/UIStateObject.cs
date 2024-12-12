using UnityEngine;

public class UIStateObject : MonoBehaviour
{
	public UIState State;

	public virtual void Activate(bool activate)
	{
		gameObject.SetActive(activate);
	}
}
