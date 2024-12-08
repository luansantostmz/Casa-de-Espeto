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
            Instance = this;
            DontDestroyOnLoad(gameObject); // Mant�m o objeto entre cenas, se necess�rio
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        OnCursorToIdle += OnIdle;
        OnCursorToHand += OnHand;
        OnCursorToHandDragging += OnHandDragging;

        // Chama o cursor padr�o ap�s garantir que o evento esteja registrado
        OnCursorToIdle?.Invoke();
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
