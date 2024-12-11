using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Icons;

public class LanguageButton : BaseButton
{
	private static int currentLanguageIndex = 0;
	public string language;
	protected override void OnClick()
	{
		base.OnClick();	

		LocalizationService.ChangeLanguage(language);
	}
}
