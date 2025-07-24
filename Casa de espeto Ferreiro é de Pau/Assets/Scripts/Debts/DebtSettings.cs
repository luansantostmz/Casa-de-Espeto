
using System;
using PirateSheep.Localization;

[Serializable]
public class DebtSettings
{
    public int DebtAmount;
    [LocalizationKey] public string DebtName;
    [LocalizationKey] public string DebtDescription;
}