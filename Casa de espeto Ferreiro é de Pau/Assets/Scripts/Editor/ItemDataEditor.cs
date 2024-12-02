using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemData))]
public class ItemDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ItemData itemData = (ItemData)target;

        if (itemData.Sprite != null)
        {
            GUILayout.Box(itemData.Sprite.texture, GUILayout.Width(128), GUILayout.Height(128));
        }
    }
}
