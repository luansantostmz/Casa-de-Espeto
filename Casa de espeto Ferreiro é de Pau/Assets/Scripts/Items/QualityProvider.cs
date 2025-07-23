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
        return Qualities[points];
    }

    public QualitySettings GetQualityByIngredients(List<UIItem> ingredients)
    {
        int sum = 0;
        foreach (var ingredient in ingredients)
        {
            sum += ingredient.Quality.Points;
        }

        int average = sum / Qualities.Count;

        return GetQualityByPoints(average);
    }
}
