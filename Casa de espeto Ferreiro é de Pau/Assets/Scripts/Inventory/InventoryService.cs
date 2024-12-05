using System.Collections.Generic;

public class InventoryService
{
    public static List<InventoryItem> Items { get; private set; } = new List<InventoryItem>();

    public static void AddItem(ItemSettings itemData, QualityType qualityType = QualityType.Bom)
    {
        // Cria um novo item caso não exista um correspondente na pilha
        InventoryItem newItem = new InventoryItem(itemData, qualityType);
        Items.Add(newItem);
        GameEvents.Inventory.OnItemAdded?.Invoke(newItem);
    }

    public static void RemoveItem(InventoryItem item)
    {
        // Procura o item na lista com a mesma qualidade
        if (Items.Contains(item))
        {
            Items.Remove(item);

            GameEvents.Inventory.OnItemRemoved?.Invoke(item);
        }
    }
}
