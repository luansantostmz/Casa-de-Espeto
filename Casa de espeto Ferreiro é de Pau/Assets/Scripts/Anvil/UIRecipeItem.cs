
public class UIRecipeItem : BaseButton
{
    protected override void OnClick()
    {
        base.OnClick();

        GameEvents.Anvil.OnRecipeItemClicked?.Invoke(GetComponent<CardItem>().Item);
    }
}
