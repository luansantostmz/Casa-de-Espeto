using System;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private Texture2D _idleCursor;
    [SerializeField] private Texture2D _toDragCursor;
    [SerializeField] private Texture2D _DraggingCursor;

    public static CursorManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Mantém o objeto entre cenas, se necessário
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        GameEvents.Cursor.OnCursorToIdle += OnIdle;
        GameEvents.Cursor.OnCursorDragging += OnDragging;
        GameEvents.Cursor.OnCursorToDrag += OnToDrag;

        GameEvents.Cursor.OnCursorToIdle?.Invoke();
    }

    private void OnDisable()
    {
        GameEvents.Cursor.OnCursorToIdle -= OnIdle;
        GameEvents.Cursor.OnCursorDragging -= OnDragging;
        GameEvents.Cursor.OnCursorToDrag -= OnToDrag;
    }

    private void OnIdle()
    {
        SetCursor(_idleCursor);
    }

    private void OnToDrag()
    {
        SetCursor(_toDragCursor);
    }

    private void OnDragging()
    {
        SetCursor(_DraggingCursor);
    }

    private void SetCursor(Texture2D texture)
    {
        Cursor.SetCursor(texture, Vector2.zero, CursorMode.Auto);
    }
}
public enum CursorType
{
    None, Idle, Dragging, ToDrag
}