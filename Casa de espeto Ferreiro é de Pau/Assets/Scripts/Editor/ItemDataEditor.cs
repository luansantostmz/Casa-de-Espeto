using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemSettings))]
public class ItemDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ItemSettings itemData = (ItemSettings)target;

        if (itemData.Sprite != null)
        {
            GUILayout.Box(itemData.Sprite.texture, GUILayout.Width(128), GUILayout.Height(128));
        }
    }
}
