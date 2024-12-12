
public class RecipeItemButton : BaseButton
{
    ItemDisplay _itemDisplay;
    ScaleDoTween _scaleDoTween;

    protected override void Awake()
    {
        base.Awake();
        _itemDisplay = GetComponent<ItemDisplay>();
        _scaleDoTween = GetComponent<ScaleDoTween>();
    }

    protected override void OnClick()
    {
        base.OnClick();

        _scaleDoTween.PlayTween();
        GameEvents.Anvil.OnRecipeItemClicked?.Invoke(_itemDisplay.Item);
    }
}
