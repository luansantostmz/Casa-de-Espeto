using System;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private Texture2D _idle;
    [SerializeField] private Texture2D _hand;
    [SerializeField] private Texture2D _handDragging;

    public static Action OnCursorToIdle;
    public static Action OnCursorToHand;
    public static Action OnCursorToHandDragging;

    public static CursorManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            OnCursorToIdle?.Invoke();
            Instance = this;
            DontDestroyOnLoad(gameObject); // Opcional: Mantém o objeto entre cenas, se necessário
            return;
        }

        Destroy(gameObject);
    }

    private void OnEnable()
    {
        OnCursorToIdle += OnIdle;
        OnCursorToHand += OnHand;
        OnCursorToHandDragging += OnHandDragging;
    }

    private void OnDisable()
    {
        OnCursorToIdle -= OnIdle;
        OnCursorToHand -= OnHand;
        OnCursorToHandDragging -= OnHandDragging;
    }

    private void OnIdle()
    {
        SetCursor(_idle);
    }

    private void OnHand()
    {
        SetCursor(_hand);
    }

    private void OnHandDragging()
    {
        SetCursor(_handDragging);
    }

    private void SetCursor(Texture2D texture)
    {
        Cursor.SetCursor(texture, Vector2.zero, CursorMode.Auto);
    }
}
