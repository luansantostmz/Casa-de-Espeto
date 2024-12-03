using UnityEngine.SceneManagement;

public class SwitchScene : BaseButton
{
	public string GamePlayScene;
	protected override void OnClick()
	{
		base.OnClick();
		SceneManager.LoadScene(GamePlayScene);
	}
}
