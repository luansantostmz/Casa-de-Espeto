using UnityEngine;
using UnityEngine.EventSystems;

public class CursorChanger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public CursorType Enter;
    public CursorType Exit;
    public CursorType BeginDrag;
    public CursorType Drag;
    public CursorType EndDrag;

    public void SetCursor(CursorType cursorType)
    {
        switch (cursorType)
        {
            case CursorType.Idle:
                GameEvents.Cursor.OnCursorToIdle?.Invoke(); break;
            case CursorType.Dragging:
                GameEvents.Cursor.OnCursorDragging?.Invoke(); break;
            case CursorType.ToDrag:
                GameEvents.Cursor.OnCursorToDrag?.Invoke(); break;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        SetCursor(BeginDrag);
    }

    public void OnDrag(PointerEventData eventData)
    {
        SetCursor(Drag);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        SetCursor(EndDrag);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetCursor(Enter);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetCursor(Exit);
    }
}
