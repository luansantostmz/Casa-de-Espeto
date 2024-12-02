using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UI Group", menuName = "UI/Group")]
public class UIStateGroup : ScriptableObject
{
    public List<UIState> States = new List<UIState>();
}
