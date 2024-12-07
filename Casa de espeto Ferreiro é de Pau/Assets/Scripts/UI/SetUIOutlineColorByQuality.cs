using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(UIOutline))]
public class SetUIOutlineColorByQuality : MonoBehaviour
{
    public QualitySettings Quality;

    private void Awake()
    {
        GetComponent<UIOutline>().color = Quality.Color;
    }
}
