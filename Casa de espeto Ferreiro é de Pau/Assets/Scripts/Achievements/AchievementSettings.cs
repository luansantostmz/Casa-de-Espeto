using UnityEngine;

[CreateAssetMenu(fileName = "New Achievement", menuName = "Achievement")]
public class AchievementSettings : ScriptableObject
{
    public string Key;
    public string Title;
    public string Description;
    public Sprite Icon;
    public Object AuxObject;

    public bool CompareAuxObject(Object obj)
    {
        return AuxObject == obj;
    }

    public void TryAchieve()
    {
        GameEvents.OnTryGetAchievement?.Invoke(this);
    }
}
