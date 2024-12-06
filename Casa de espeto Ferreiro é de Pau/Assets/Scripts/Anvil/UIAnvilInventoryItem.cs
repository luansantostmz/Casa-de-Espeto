using UnityEngine;

public class UIAnvilInventoryItem : MonoBehaviour
{
    UICardItem _uiInventory;
    DragAndDrop _dragNDrop;

    DropZone _currentDropZone;

    private void Awake()
    {
        _dragNDrop = GetComponent<DragAndDrop>();
        _uiInventory = GetComponent<UICardItem>();

        _dragNDrop.OnDrop += OnDrop;
    }

    private void OnDestroy()
    {
        _dragNDrop.OnDrop -= OnDrop;
    }

    void OnDrop(DropZone dropZone)
    {
        if (!dropZone)
            return;

        if (dropZone.TryGetComponent(out UIAnvil anvil))
        {
            GameEvents.Inventory.OnCardItemRemovedFromInventory?.Invoke(_uiInventory);
            GameEvents.Anvil.OnItemAddedToAnvil?.Invoke(_uiInventory);
        }
        else if (dropZone.TryGetComponent(out UIInventory inventory))
        {
            GameEvents.Inventory.OnCardItemAddedToInventory?.Invoke(_uiInventory);
            GameEvents.Anvil.OnItemRemovedFromAnvil?.Invoke(_uiInventory);
        }
    }
}
