using UnityEngine;

[CreateAssetMenu(fileName = "New Quality", menuName = "Quality")]
public class QualitySettings : ScriptableObject
{
    public string QualityName;
    public int Points;
    public float PriceModifier;
    public Color Color;
}
