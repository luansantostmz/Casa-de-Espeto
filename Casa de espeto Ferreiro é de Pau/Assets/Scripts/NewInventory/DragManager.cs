using UnityEngine;

public class DragManager : MonoBehaviour
{
    public Transform DragTransform;

    public static DragManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            return;
        }

        Destroy(gameObject);
    }
}
