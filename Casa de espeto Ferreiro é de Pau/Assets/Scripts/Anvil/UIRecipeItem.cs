using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRecipeItem : BaseButton
{
    protected override void OnClick()
    {
        base.OnClick();

        GameEvents.Anvil.OnRecipeItemClicked?.Invoke(GetComponent<UICardItem>().Item.Settings);
    }
}
