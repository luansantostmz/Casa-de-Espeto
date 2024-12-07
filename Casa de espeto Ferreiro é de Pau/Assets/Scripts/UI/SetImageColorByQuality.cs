using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SetImageColorByQuality : MonoBehaviour
{
    public QualitySettings Quality;

    private void Awake()
    {
        GetComponent<Image>().color = Quality.Color;
    }
}
