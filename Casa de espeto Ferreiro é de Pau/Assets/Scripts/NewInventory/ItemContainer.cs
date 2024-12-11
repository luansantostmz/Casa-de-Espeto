using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    public Inventory Inventory;

    public List<InventoryItem> InitialItems = new List<InventoryItem>();
    public UIItem UIItemPrefab; // Referência ao prefab do UIItem
    public Transform Container; // Transform onde os UIItems serão instanciados (por exemplo, um painel)
    public bool IsBlocked;
    public UIDropHandler DropHandler;

    public List<UIItem> UIItems = new List<UIItem>();

    private void Awake()
    {
        Inventory = new Inventory();

        Inventory.OnItemAdded += OnItemAdded;
        Inventory.OnItemRemoved += OnItemRemoved;

        foreach (InventoryItem item in InitialItems)
        {
            // Adiciona o item ao inventário
            Inventory.AddItem(item.Settings, item.Quality, item.Quantity);
        }
    }

    private void OnDestroy()
    {
        Inventory.OnItemAdded -= OnItemAdded;
        Inventory.OnItemRemoved -= OnItemRemoved;
    }

    private void OnItemAdded(InventoryItem item)
    {
        foreach (var uiItem in UIItems)
        {
            if (uiItem.InventoryItem.IsIdentical(item))
            {
                uiItem.UpdateText();
                return;
            }
        }

        InstantiateUIItem(item);
    }

    private void OnItemRemoved(InventoryItem item)
    {
        foreach (var uiItem in UIItems)
        {
            if (uiItem.InventoryItem.IsIdentical(item))
            {
                if (uiItem.InventoryItem.Quantity > 0)
                    uiItem.UpdateText();
                else
                    Destroy(uiItem.gameObject);

                return;
            }
        }
    }

    private void InstantiateUIItem(InventoryItem item)
    {
        // Instancia o UIItem a partir do prefab e define o pai no UI
        if (UIItemPrefab != null && Container != null)
        {
            UIItem uiItem = Instantiate(UIItemPrefab, Container);

            if (uiItem != null)
            {
                // Aqui você pode configurar o UIItem com os dados do InventoryItem
                uiItem.Setup(item); // Supondo que o UIItem tenha um método Setup que configura o item
                uiItem.GetComponent<UIDragHandler>().CurrentDropHandler = DropHandler;
            }

            UIItems.Add(uiItem);
        }
    }

    public bool TryRemoveItem(InventoryItem item)
    {
        // Tenta remover o item do inventário
        if (Inventory.RemoveItem(item.Settings, item.Quality, item.Quantity))
        {
            // Verifica se algum UIItem correspondente existe
            foreach (var uiItem in UIItems)
            {
                if (uiItem.InventoryItem.Settings == item.Settings && uiItem.InventoryItem.Quality == item.Quality)
                {
                    // Ajusta a quantidade ou remove o UIItem
                    uiItem.AdjustQuantity(-item.Quantity);

                    // Se a quantidade do UIItem for 0, remove o UIItem
                    if (uiItem.InventoryItem.Quantity <= 0)
                    {
                        UIItems.Remove(uiItem);
                        Destroy(uiItem.gameObject); // Remove o objeto da UI
                    }

                    return true;
                }
            }
        }

        return false; // Retorna falso se o item não foi encontrado ou não pôde ser removido
    }
}
