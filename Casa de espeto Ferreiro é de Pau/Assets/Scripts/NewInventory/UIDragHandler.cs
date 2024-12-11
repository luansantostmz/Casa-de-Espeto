using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 originalPosition;
    private Transform originalParent;
    private Transform mainParent;

    public UIDropHandler CurrentDropHandler;

    public Action<UIDropHandler> OnDrop;
    public Action OnDragStart;

    public AudioClip _onDragClip;
    public AudioClip _onDropClip;

    private UIItem _uiItem;
    private Image _image;

    private void Awake()
    {
        originalPosition = transform.position;
        originalParent = transform.parent;
        mainParent = transform.parent;
        _uiItem = GetComponent<UIItem>();
        _image = GetComponent<Image>();
    }

    private void ActivateRaycastTarget(bool activate)
    {
        _image.raycastTarget = activate;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        CursorManager.OnCursorToHandDragging?.Invoke();

        OnDragStart?.Invoke();
        originalPosition = transform.position;
        originalParent = transform.parent;

        if (_uiItem.InventoryItem.Quantity > 1)
        {
            UIItem clone = Instantiate(_uiItem, originalParent);
            clone.transform.SetSiblingIndex(transform.GetSiblingIndex());

            clone.Setup(_uiItem.InventoryItem.GetClone());
            clone.AdjustQuantity(-1);

            CurrentDropHandler.ItemContainer.Inventory.AddItem(
                clone.InventoryItem.Settings,
                clone.InventoryItem.Quality,
                clone.InventoryItem.Quantity);

            _uiItem.SetQuantity(1);
        }

        _image.raycastTarget = false;

        // Eleva o objeto na hierarquia para evitar sobreposi��o visual
        transform.SetParent(DragManager.Instance.DragTransform, true);

        GameEvents.DragAndDrop.OnDragStarted?.Invoke(this);

        if (_onDragClip)
            AudioManager.Instance.PlaySFX(_onDragClip);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Atualiza a posi��o do objeto com o mouse
        var newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        newPos.z = 0;
        transform.position = newPos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        CursorManager.OnCursorToIdle?.Invoke();

        if (_onDropClip)
            AudioManager.Instance.PlaySFX(_onDropClip);

        if (eventData.pointerEnter != null)
        {
            UIDropHandler dropHandler = eventData.pointerEnter.GetComponent<UIDropHandler>();

            if (dropHandler != null && !dropHandler.IsBlocked)
            {
                // Soltar no container correto
                transform.SetParent(dropHandler.Container ? dropHandler.Container : dropHandler.transform, true);
                transform.localPosition = Vector3.zero; // Fixa na posi��o do container
                OnDrop?.Invoke(dropHandler);
                dropHandler.OnDropHere?.Invoke(this);
                GameEvents.DragAndDrop.OnDragEnded?.Invoke(this, dropHandler);
                CurrentDropHandler = dropHandler;
            }
            else
            {
                // Soltar em algo que n�o � um item container
                ResetPosition();
                OnDrop?.Invoke(null);
                GameEvents.DragAndDrop.OnDragEnded?.Invoke(this, null);
            }
        }
        else
        {
            // N�o soltou sobre nada
            ResetPosition();
            GameEvents.DragAndDrop.OnDragEnded?.Invoke(this, null);
        }

        _image.raycastTarget = true;
    }

    private void ResetPosition()
    {
        // Voltar � posi��o original
        transform.SetParent(originalParent, true);
        transform.position = originalPosition;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        CursorManager.OnCursorToHand?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CursorManager.OnCursorToIdle?.Invoke();
    }
}
