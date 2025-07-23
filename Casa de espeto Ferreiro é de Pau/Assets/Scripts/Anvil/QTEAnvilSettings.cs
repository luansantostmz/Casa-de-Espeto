using UnityEngine;

[CreateAssetMenu(fileName = "New QTE Anvil Settings", menuName = "QTE Anvil Settings")]
public class QTEAnvilSettings : ScriptableObject
{
    public float InitialPointerSpeed = 200f;
    public float SpeedIncreasePerHit = 50f;
    public int MaxChances = 4;
    public float MinAreaWidth = 50f;
    public float MaxAreaWidth = 150f;
    public float AreaPadding = 10f;
}
