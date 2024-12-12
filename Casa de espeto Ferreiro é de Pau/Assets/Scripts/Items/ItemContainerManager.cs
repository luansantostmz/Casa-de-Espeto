using UnityEngine;

public class ItemContainerManager : MonoBehaviour
{
    public ItemContainer Inventory;
    public ItemContainer Anvil;

    public static ItemContainerManager Instance;

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
