using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ForgeController : MonoBehaviour
{
    private List<UIForgeSlot> _slots = new List<UIForgeSlot>();
    public UIForgeSlot ForgeSlotPrefab;
    public Recipes Recipes;
    public Transform ForgesContainer;
    public Button PurchaseForgeButton;
    public TMP_Text PriceText;
    public Image BurningImage;

    int PurchaseForgePrice => GameManager.Instance.GameplaySettings.NewForgePrice;

    private void Awake()
    {
        PurchaseForgeButton.onClick.AddListener(PurchaseNewForge);
        GameEvents.Economy.OnGoldChanged += RefreshButton;
    }

    private void Start()
    {
        Initialize();
    }

    void Update()
    {
        bool isOn = false;
        foreach (var slot in _slots)
        {
            if (slot.Clock.InProgress)
            {
                isOn = true;
                break;
            }
        }

        BurningImage.gameObject.SetActive(isOn);
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

        if (_slots.Count >= GameManager.Instance.GameplaySettings.MaxForgeCount)
        {
            PurchaseForgeButton.gameObject.SetActive(false);
        }
    }

    private UIForgeSlot InstantiateNewForge()
    {
        UIForgeSlot newForge = Instantiate(ForgeSlotPrefab, ForgesContainer);
        newForge.Initialize(this);
        _slots.Add(newForge);
        return newForge;
    }
}