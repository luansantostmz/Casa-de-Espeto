using UnityEngine;

public class UIDebug : MonoBehaviour
{
    public GameObject[] DebugObjects;

    public bool Active;

    void Start()
    {
        ActivateObjects(Active);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F8))
        {
            Active = !Active;
            ActivateObjects(Active);
        }
    }

    void ActivateObjects(bool active)
    {
        foreach (var obj in DebugObjects)
        {
            obj.gameObject.SetActive(active);
        }
    }
}
