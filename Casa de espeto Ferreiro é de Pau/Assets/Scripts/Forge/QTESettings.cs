
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Forge Values", menuName = "Forge/ForgeValues", order = 0)]

public class QTESettings : ScriptableObject
{
	public List<TimeValueRange> valueRanges = new List<TimeValueRange>();  // Lista de faixas definidas no Inspetor	
}
