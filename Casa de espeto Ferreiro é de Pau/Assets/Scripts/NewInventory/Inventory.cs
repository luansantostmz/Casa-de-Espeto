using System;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class Inventory
{
    public List<InventoryItem> Items { get; private set; }

    public Action<InventoryItem> OnItemAdded;
    public Action<InventoryItem> OnItemRemoved;

    public Inventory()
    {
        Items = new List<InventoryItem>();
    }

    // Adiciona um item ao invent�rio
    public AddItemResult AddItem(ItemSettings settings, QualitySettings quality, int quantity = 1)
    {
        // Verifica se h� um item id�ntico para empilhar
        foreach (var item in Items)
        {
            if (item.IsIdentical(new InventoryItem(settings, quality)))
            {
                item.Quantity += quantity;
                OnItemAdded?.Invoke(item);
                return new AddItemResult()
                {
                    IsNewItem = false
                };
            }
        }

        // Adiciona como novo item
        var newItem = new InventoryItem(settings, quality, quantity);
        Items.Add(newItem);
        OnItemAdded?.Invoke(newItem);
        return new AddItemResult()
        {
            IsNewItem = true
        };
    }

    // Remove uma quantidade de um item do invent�rio
    public bool RemoveItem(ItemSettings settings, QualitySettings quality, int quantity = 1)
    {
        foreach (var item in Items)
        {
            if (item.IsIdentical(new InventoryItem(settings, quality)))
            {
                if (item.Quantity >= quantity)
                {
                    item.Quantity -= quantity;

                    // Remove o item se a quantidade chegar a zero
                    if (item.Quantity <= 0)
                    {
                        OnItemRemoved?.Invoke(item);
                        Items.Remove(item);
                    }

                    return true;
                }
                return false; // Quantidade insuficiente
            }
        }

        return false; // Item n�o encontrado
    }

    // Obt�m todos os itens do invent�rio
    public List<InventoryItem> GetItems()
    {
        return new List<InventoryItem>(Items);
    }

    // Verifica se um item espec�fico est� no invent�rio
    public bool ContainsItem(ItemSettings settings, QualitySettings quality)
    {
        return Items.Any(item => item.IsIdentical(new InventoryItem(settings, quality)));
    }
}

public class AddItemResult
{
    public bool IsNewItem;
}