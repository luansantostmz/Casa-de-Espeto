using UnityEngine;

public class QuitButton : BaseButton
{
	protected override void OnClick()
	{
		base.OnClick();
		Application.Quit();
	}
}
