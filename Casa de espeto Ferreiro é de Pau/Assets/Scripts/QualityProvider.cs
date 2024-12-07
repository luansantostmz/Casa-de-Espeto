using System.Collections.Generic;
using UnityEngine;

public class QualityProvider : MonoBehaviour
{
    public List<QualitySettings> Qualities = new List<QualitySettings>();

    public static QualityProvider Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            return;
        }

        Destroy(gameObject);
    }

    public QualitySettings GetFirstQuality()
    {
        return Qualities[0];
    }

    public QualitySettings GetQualityByPoints(int points)
    {
        float smallestDifference = float.MaxValue;

        QualitySettings closest = null;

        foreach (var quality in Qualities)
        {
            float difference = Mathf.Abs(quality.Points - points);

            if (difference < smallestDifference)
            {
                smallestDifference = difference;
                closest = quality;
            }
        }

        return closest;
    }

    public QualitySettings GetQualityByIngredients(List<UICardItem> ingredients)
    {
        int sum = 0;
        foreach(var ingredient in ingredients)
        {
            sum += ingredient.Item.Quality.Points;
        }

        int average = sum / Qualities.Count;

        return GetQualityByPoints(average);
    }
}
