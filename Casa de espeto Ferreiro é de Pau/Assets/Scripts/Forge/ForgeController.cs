using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ForgeController : MonoBehaviour
{
    public UIForgeSlot ForgeSlotPrefab;
    public Recipes Recipes;
    public Transform ForgesContainer;
    public Button PurchaseForgeButton;
    public TMP_Text PriceText;

    int PurchaseForgePrice => GameManager.Instance.GameplaySettings.ForgePrice;

    private void Awake()
    {
        PurchaseForgeButton.onClick.AddListener(PurchaseNewForge);
        GameEvents.Economy.OnGoldChanged += RefreshButton;
    }

    private void Start()
    {
        Initialize();
    }

    private void OnDestroy()
    {
        PurchaseForgeButton.onClick.RemoveListener(PurchaseNewForge);
        GameEvents.Economy.OnGoldChanged -= RefreshButton;
    }

    private void Initialize()
    {
        for (int i = 0; i < GameManager.Instance.GameplaySettings.InitialForgeCount; i++)
        {
            InstantiateNewForge();
        }
    }

    private void RefreshButton()
    {
        PurchaseForgeButton.interactable = EconomyService.CurrentGold >= PurchaseForgePrice;
        PriceText.text = PurchaseForgePrice.ToString();
    }

    public void PurchaseNewForge()
    {
        EconomyService.SubtractGold(PurchaseForgePrice);
        InstantiateNewForge();
    }

    private UIForgeSlot InstantiateNewForge()
    {
        UIForgeSlot newForge = Instantiate(ForgeSlotPrefab, ForgesContainer);
        newForge.Initialize(this);
        return newForge;
    }
}