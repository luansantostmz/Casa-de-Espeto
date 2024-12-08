using System;
using System.Collections.Generic;
using UnityEngine;

public class DataService
{
    public static RunData CurrentRunData;
    public static Data CachedData;

    public static void InitializeNewRun()
    {
        CurrentRunData = new RunData();
        CurrentRunData.GameplayDate = DateTime.Now;
        CurrentRunData.DeliveredOrders = new List<SavedOrderData>();
        CurrentRunData.FailedOrders = new List<SavedOrderData>();
    }

    public static void EndRun()
    {
        CachedData.TotalSecondsPlayed += (int)(DateTime.Now - CurrentRunData.GameplayDate).TotalSeconds;
        CurrentRunData.GameplayTime = (int)(DateTime.Now - CurrentRunData.GameplayDate).TotalSeconds;
        SaveCurrentRunData();
        CurrentRunData = null;
    }

    public static void SaveCurrentRunData()
    {
        CachedData.RunsData.Add(CurrentRunData);
        SaveData(CachedData);
    }

    public static void SaveData(Data data)
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(nameof(Data), json);
        Debug.Log("SAVED DATA: " + json);
    }

    public static Data LoadData()
    {
        CachedData = JsonUtility.FromJson<Data>(PlayerPrefs.GetString(nameof(Data)));
        if (CachedData == null)
            CachedData = new Data();

        Debug.Log("LOADED DATA: " + PlayerPrefs.GetString(nameof(Data)));
        return CachedData;
    }
}

[System.Serializable]
public class Data
{
    public int TotalDeliveredOrders;
    public int TotalFailedOrders;
    public int TotalSecondsPlayed;
    public List<RunData> RunsData = new List<RunData>();
}

[System.Serializable]
public class RunData
{
    public int GameplayTime;
    public int GoldEarned;
    public DateTime GameplayDate;
    public List<SavedOrderData> DeliveredOrders = new List<SavedOrderData>();
    public List<SavedOrderData> FailedOrders = new List<SavedOrderData>();
}

[System.Serializable]
public class SavedOrderData
{
    public int Reward;
    public List<ItemData> Items;
}

[System.Serializable]
public class ItemData
{
    public string ItemName;
    public string ItemQuality;
}