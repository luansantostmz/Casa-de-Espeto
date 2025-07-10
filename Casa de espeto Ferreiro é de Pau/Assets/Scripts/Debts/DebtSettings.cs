
using System;
using UnityEngine;

[Serializable]
public class DebtSettings
{
    public string DebtName;
    public int DebtAmount;
    [TextArea] public string DebtDescription;
}