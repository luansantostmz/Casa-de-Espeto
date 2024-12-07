using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New QTE Anvil Settings", menuName = "QTE Anvil Settings")]
public class QTEAnvilSettings : ScriptableObject
{
    public float speed = 0.5f;                     // Velocidade inicial
    public float acceleration = 0.1f;              // Aceleração
    public float maxSpeed = 2f;
    public QTESettings QTESettings;
}
