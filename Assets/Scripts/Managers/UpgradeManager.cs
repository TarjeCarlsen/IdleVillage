using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public enum UpgradeTypes{
    globalPower,
    moneyPower,
    farmPower,

}

public enum TimeUpgradeTypes
{
    wheatPlantGrowTime,
    furnaceBakingTime,
    schoolChildFarmerTime,
    schoolChildElectricianTime,
    schoolChildMinerTime,
    schoolChildAdventurerTime,
    schoolMaxTime,
    farmerTimePerTask,
    electricianTimePerTask,
    minerTimePerTask,
    AdventurerTimePerTask,

}


public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    public Dictionary<CurrencyTypes, BigNumber> productionPower;
    public Dictionary<CurrencyTypes, BigNumber> storagePower;
    public Dictionary<TimeUpgradeTypes, float> upgradeTime;
    [SerializeField] private BigNumber defaultProductionPower = 1;
    [SerializeField] private BigNumber defaultStoragePower = 100;
    [SerializeField] private float defaultTimePower = 2;


    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

    }


    private void InitializePowers(){
        productionPower = new Dictionary<CurrencyTypes, BigNumber>();
        storagePower = new Dictionary<CurrencyTypes, BigNumber>();
        foreach(CurrencyTypes type in Enum.GetValues(typeof(CurrencyTypes))){
            productionPower[type] = defaultProductionPower;
            storagePower[type] = defaultStoragePower;
        }
    }

    private void InitializeTime(){
        upgradeTime = new Dictionary<TimeUpgradeTypes, float>();
        foreach(TimeUpgradeTypes type in Enum.GetValues(typeof(TimeUpgradeTypes))){
            upgradeTime[type] = defaultTimePower;
        }

    }

    public BigNumber GetProductionPower(CurrencyTypes type) => productionPower[type];
    public BigNumber GetStoragePower(CurrencyTypes type) => storagePower[type];


    public void AddPowerFlat(CurrencyTypes type, BigNumber amount) => productionPower[type] += amount;

    public void AddPowerMulti(CurrencyTypes type, BigNumber multiplier)
    {
        BigNumber result = (BigNumber)productionPower[type] * multiplier;
        productionPower[type] = result;
    }
    public void AddStorageAmountFlat(CurrencyTypes type, BigNumber amount)
    {
        storagePower[type] += amount;
        StorageManager.Instance.AddStorageAmount(type, amount);
    }
    public void AddStorageAmountMulti(CurrencyTypes type, BigNumber multiplier)
    {
        BigNumber result = (BigNumber)storagePower[type] * multiplier;
        storagePower[type] = result;
        StorageManager.Instance.AddStorageAmount(type, storagePower[type]);
    }
    private const float MIN_TIME = 0.1f; // Clamp time. MAXIMUM REDUCTION REACHING 0.1f
    public float GetTimePower(TimeUpgradeTypes type) {
    float time = upgradeTime[type];
    return Math.Max(MIN_TIME, time); // CLAMPING TIME TO MAXIMUM REDUCTION REACHING MIN_TIME
    } 
    public float IncreaseTimeFlat(TimeUpgradeTypes type, float amount) => upgradeTime[type] + amount;

    public float SetTime(TimeUpgradeTypes type, float amount)
    {
     float time = upgradeTime[type] = amount;
    return Math.Max(MIN_TIME,time);
    }
    public void ReduceTime(TimeUpgradeTypes type, float multi)
    { // diminishing returns
        upgradeTime[type] *= multi;
        upgradeTime[type] = Math.Max(MIN_TIME, upgradeTime[type]);
    }

    public void Save(ref upgradeManagerSaveData data)
    {
        data.productionData = new List<UpgradeDataList>();
        data.storageData = new List<UpgradeDataList>();
        data.timeData = new List<UpgradeDataList>();

        foreach (var pair in productionPower)
        {
            data.productionData.Add(new UpgradeDataList
            {
                type = pair.Key.ToString(),
                amount = pair.Value.ToString()
            });
        }

        foreach (var pair in storagePower)
        {
            data.storageData.Add(new UpgradeDataList
            {
                type = pair.Key.ToString(),
                amount = pair.Value.ToString()
            });
        }


        foreach (var pair in upgradeTime)
        {
            data.timeData.Add(new UpgradeDataList
            {
                type = pair.Key.ToString(),
                amount = pair.Value.ToString("R") // "R" = round-trip float format
            });
        }


    }

    public void Load(upgradeManagerSaveData data)
    {
        // Initialize first to ensure no null issues
        InitializePowers();
        InitializeTime();

        foreach (var entry in data.productionData)
        {
            if (Enum.TryParse(entry.type, out CurrencyTypes type))
                productionPower[type] = BigNumber.Parse(entry.amount);
        }

        foreach (var entry in data.storageData)
        {
            if (Enum.TryParse(entry.type, out CurrencyTypes type))
                storagePower[type] = BigNumber.Parse(entry.amount);
        }
        if (data.timeData != null)
        {
            foreach (var entry in data.timeData)
            {
                if (Enum.TryParse(entry.type, out TimeUpgradeTypes type))
                    upgradeTime[type] = float.Parse(entry.amount);
            }
        }

        Debug.Log("[UpgradeManager] All data loaded successfully!");
    }
}

[System.Serializable]
public struct upgradeManagerSaveData
{
    public List<UpgradeDataList> productionData;
    public List<UpgradeDataList> storageData;
    public List<UpgradeDataList> timeData;
}

[Serializable]
public struct UpgradeDataList
{
    public string type;
    public string amount;
}
[System.Serializable]
public struct ActivationDataList
{
    public string type;
    public bool state;
}
