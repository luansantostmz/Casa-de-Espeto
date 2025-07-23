using UnityEngine;

public class ForgeButtonAlert : MonoBehaviour
{
    [SerializeField] ForgeController _forge;
    [SerializeField] GameObject _alert;
    bool _onForgeScreen;

    public void SetOnForge(bool onForge)
    {
        _onForgeScreen = onForge;
    }

    void Update()
    {
        if (_onForgeScreen)
        {
            _alert.SetActive(false);
            return;
        }

        bool activate = false;

        foreach (var forgeSlot in _forge.Slots)
        {
            if (!forgeSlot.Clock.InProgress && forgeSlot.Items.Count > 0)
            {
                activate = true;
            }
        }

        _alert.SetActive(activate);
    }
}
