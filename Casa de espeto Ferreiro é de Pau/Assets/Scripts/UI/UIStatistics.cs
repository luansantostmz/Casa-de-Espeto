using TMPro;
using UnityEngine;

public class UIStatistics : MonoBehaviour
{
    [SerializeField] TMP_Text _totalDeliveredOrders;
    [SerializeField] TMP_Text _totalFailedOrders;
    [SerializeField] TMP_Text _totalPlayedTimes;

    private void OnEnable()
    {
        var data = DataService.LoadData();

        _totalDeliveredOrders.text = data.TotalDeliveredOrders.ToString();
        _totalFailedOrders.text = data.TotalFailedOrders.ToString();
        _totalPlayedTimes.text = FormatSecondsToTime(data.TotalSecondsPlayed);
    }

    public static string FormatSecondsToTime(int totalSeconds)
    {
        int hours = totalSeconds / 3600; 
        int minutes = (totalSeconds % 3600) / 60; 
        return $"{hours:D2}:{minutes:D2}"; 
    }
}
