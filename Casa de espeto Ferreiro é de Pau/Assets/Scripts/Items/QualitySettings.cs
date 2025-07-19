using UnityEngine;

[CreateAssetMenu(fileName = "New Quality", menuName = "Quality")]
public class QualitySettings : ScriptableObject
{
    public string QualityName;
    public int Points;
    public float PriceModifier;
    public float DeliverReputationModifier;
    public float DeliverGoldModifier;
    public Color Color;
    public Sprite ItemBackground;
    public bool IsSpecial;
}
