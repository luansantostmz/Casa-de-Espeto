using UnityEngine;

public class UIStateInitializer : MonoBehaviour
{
    [SerializeField] UIState _initialState;

    void Start()
    {
        _initialState.Activate();
    }
}