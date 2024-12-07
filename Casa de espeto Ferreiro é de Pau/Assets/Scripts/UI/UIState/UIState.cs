using UnityEngine;

[CreateAssetMenu(fileName = "UI State", menuName = "UI/State")]
public class UIState : ScriptableObject
{
    public UIStateGroup Group;

    public void Activate()
    {
        UIStateOrchestrator.ActiveState(this);
    }

    public void Deactive()
    {
		UIStateOrchestrator.DeactiveState(this);
	}
}
