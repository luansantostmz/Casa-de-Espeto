using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIAchievements : MonoBehaviour
{
    public TMP_Text Title;
    public TMP_Text Description;
    public Image Icon;

    TweenObject tweenObject;

    private void Awake()
    {
        tweenObject = GetComponent<TweenObject>();
    }

    public void Show(AchievementSettings achievement)
    {
        Title.text = achievement.Title;
        Description.text = achievement.Description;
        Icon.sprite = achievement.Icon;
        gameObject.SetActive(true);
        tweenObject.PlayTween();
    }

    public void Hide()
    {
        tweenObject.PlayReverse();
    }
}
