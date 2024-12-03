using System.Collections.Generic;

public class InventoryService
{
    public static List<InventoryItem> Items { get; private set; } = new List<InventoryItem>();

    public static void AddItem(ItemSettings itemData, int quantity = 1, QualityType qualityType = QualityType.Common)
    {
        if (itemData.IsStackable)
        {
            // Procura um item existente com a mesma qualidade
            InventoryItem existingItem = Items.Find(i => i.Settings == itemData && i.Quality == qualityType);

            if (existingItem != null)
            {
                // Atualiza a quantidade do item existente
                existingItem.Quantity += quantity;
                GameEvents.Inventory.OnItemAdded?.Invoke(existingItem);
                return;
            }
        }

        // Cria um novo item caso não exista um correspondente na pilha
        InventoryItem newItem = new InventoryItem(itemData, quantity, qualityType);
        Items.Add(newItem);
        GameEvents.Inventory.OnItemAdded?.Invoke(newItem);
    }

    public static void RemoveItem(ItemSettings itemData, int quantity = 1, QualityType qualityType = QualityType.Common)
    {
        // Procura o item na lista com a mesma qualidade
        InventoryItem existingItem = Items.Find(i => i.Settings == itemData && i.Quality == qualityType);

        if (existingItem != null)
        {
            if (existingItem.Settings.IsStackable)
            {
                existingItem.Quantity -= quantity;
                if (existingItem.Quantity <= 0)
                {
                    Items.Remove(existingItem);
                }
            }
            else
            {
                Items.Remove(existingItem);
            }

            GameEvents.Inventory.OnItemRemoved?.Invoke(existingItem);
        }
    }
}
