using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 originalPosition;
    private Transform originalParent;
    private Transform mainParent;

    public UIDropHandler CurrentDropHandler;

    public Action<UIDropHandler> OnDrop;
    public Action OnDragStart;
    public Action OnDragEnd;

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
        OnDragStart?.Invoke();
        originalPosition = transform.position;
        originalParent = transform.parent;

        CurrentDropHandler.ItemContainer.RemoveItem(_uiItem);
        if (_uiItem.Quantity > 1)
        {
            UIItem clone = Instantiate(_uiItem, originalParent);
            clone.ShowBackground();
            clone.transform.SetSiblingIndex(transform.GetSiblingIndex());

            clone.Setup(_uiItem.Item, _uiItem.Quality, _uiItem.Quantity - 1);

            CurrentDropHandler.ItemContainer.AddItem(clone);

            _uiItem.SetQuantity(1);
        }

        _image.raycastTarget = false;

        // Eleva o objeto na hierarquia para evitar sobreposição visual
        transform.SetParent(DragManager.Instance.DragTransform, true);

        GameEvents.DragAndDrop.OnDragStarted?.Invoke(this);

        if (_onDragClip)
            AudioManager.Instance.PlaySFX(_onDragClip);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Atualiza a posição do objeto com o mouse
        var newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        newPos.z = 0;
        transform.position = newPos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OnDragEnd?.Invoke();

        if (_onDropClip)
            AudioManager.Instance.PlaySFX(_onDropClip);

        if (eventData.pointerEnter != null)
        {
            UIDropHandler dropHandler = eventData.pointerEnter.GetComponent<UIDropHandler>();

            if (dropHandler != null && !dropHandler.IsBlocked)
            {
                // Soltar no container correto
                transform.SetParent(dropHandler.Container ? dropHandler.Container : dropHandler.transform, true);
                transform.localPosition = Vector3.zero; // Fixa na posição do container
                OnDrop?.Invoke(dropHandler);
                dropHandler.OnDropHere?.Invoke(this);
                dropHandler.ItemContainer.AddItem(_uiItem);
                GameEvents.DragAndDrop.OnDragEnded?.Invoke(this, dropHandler);
                CurrentDropHandler = dropHandler;
            }
            else
            {
                // Soltar em algo que não é um item container
                CurrentDropHandler.ItemContainer.AddItem(_uiItem);
                ResetPosition();
                OnDrop?.Invoke(null);
                GameEvents.DragAndDrop.OnDragEnded?.Invoke(this, null);
            }
        }
        else
        {
            // Não soltou sobre nada
            CurrentDropHandler.ItemContainer.AddItem(_uiItem);
            ResetPosition();
            GameEvents.DragAndDrop.OnDragEnded?.Invoke(this, null);
        }

        _image.raycastTarget = true;
    }

    private void ResetPosition()
    {
        // Voltar à posição original
        transform.SetParent(originalParent, true);
        transform.position = originalPosition;
    }
}
