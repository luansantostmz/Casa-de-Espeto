using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    public UIItem UIItemPrefab; // Referência ao prefab do UIItem
    public Transform Container; // Transform onde os UIItems serão instanciados (por exemplo, um painel)
    public UIDropHandler DropHandler;
    public AudioClip OnAddItemSfx;

    public List<UIItem> Items = new List<UIItem>();

    public Action<UIItem> OnItemAdded;
    public Action<UIItem> OnItemRemoved;

    protected virtual void Awake()
    {
        DropHandler.Initialize(this);
    }

    protected virtual void OnDestroy()
    {

    }

    public void InstantiateNewItem(ItemSettings item, QualitySettings quality, int quantity)
    {
        if (UIItemPrefab != null && Container != null)
        {
            UIItem uiItem = Instantiate(UIItemPrefab, Container);

            uiItem.Setup(item, quality, quantity);
            uiItem.GetComponent<UIDragHandler>().CurrentDropHandler = DropHandler;

            AddItem(uiItem);
            return;
        }

        Debug.LogError($"It was not possible to instantiate {item.ItemName}");
    }

    public virtual void AddItem(UIItem uiItem)
    {
        AudioManager.Instance.PlaySFX(OnAddItemSfx);

        foreach (var ui in Items)
        {
            if (ui.IsIdentical(uiItem))
            {
                ui.Quantity += uiItem.Quantity;
                Destroy(uiItem.gameObject);
                ui.UpdateVisual();
                OnItemAdded?.Invoke(ui);
                return;
            }
        }

        Items.Add(uiItem);
        uiItem.ShowBackground();
        uiItem.TweenScale.PlayTween();
        OnItemAdded?.Invoke(uiItem);
    }

    public virtual void RemoveItem(UIItem uiItem)
    {
        Items.Remove(uiItem);
    }

    public virtual bool TryRemoveItem(ItemSettings item, QualitySettings quality, int quantity = 1)
    {
        // Verifica se algum UIItem correspondente existe
        foreach (var uiItem in Items)
        {
            if (uiItem.Item == item && uiItem.Quality == quality)
            {
                // Ajusta a quantidade ou remove o UIItem
                uiItem.AdjustQuantity(-quantity);
                uiItem.UpdateVisual();

                // Se a quantidade do UIItem for 0, remove o UIItem
                if (uiItem.Quantity <= 0)
                {
                    Items.Remove(uiItem);
                    Destroy(uiItem.gameObject); // Remove o objeto da UI
                }

                return true;
            }
        }

        return false; // Retorna falso se o item não foi encontrado ou não pôde ser removido
    }
}