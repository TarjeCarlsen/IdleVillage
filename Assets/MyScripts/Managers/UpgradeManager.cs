using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UIElements;
using LargeNumbers;
using System.Runtime.CompilerServices;


public enum TimeUpgradeTypes
{
    wheatPlantGrowTime,
    doughPressCycleTime,
    furnaceBakingTime,
    windmillGrainTime,
    windmillFlourTime,
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

public enum SpecialUpgradeTypes{
    flourDragAmount,
    flourToDoughClickPower,
    doughDragAmount,
    doughCreatePower,

}

public enum ActivationUnlocks{
    doughpress,
}

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    public Dictionary<CurrencyTypes, AlphabeticNotation> productionPower;
    public Dictionary<CurrencyTypes, AlphabeticNotation> storagePower;
    public Dictionary<SpecialUpgradeTypes, AlphabeticNotation> specialProductionPower;
    public Dictionary<ActivationUnlocks, bool> activationUnlocks;
    public Dictionary<TimeUpgradeTypes, float> upgradeTime;
    [SerializeField] private AlphabeticNotation defaultProductionPower = new AlphabeticNotation(1);
    [SerializeField] private AlphabeticNotation defaultSpecialProductionPower = new AlphabeticNotation(1);
    [SerializeField] private AlphabeticNotation defaultStoragePower = new AlphabeticNotation(100);
    [SerializeField] private bool defaultActivationUnlock = false;
    [SerializeField] private float defaultTimePower = 2;

    public event Action OnActivationUnlock;


    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        InitializeUnlocks();
        InitializePowers();
        InitializeTime();
        InitializeSpecialPowers();
    }


    private void InitializePowers(){
        productionPower = new Dictionary<CurrencyTypes, AlphabeticNotation>();
        storagePower = new Dictionary<CurrencyTypes, AlphabeticNotation>();
        foreach(CurrencyTypes type in Enum.GetValues(typeof(CurrencyTypes))){
            productionPower[type] = defaultProductionPower;
            storagePower[type] = defaultStoragePower;
        }
    }

    private void InitializeSpecialPowers(){
        specialProductionPower = new Dictionary<SpecialUpgradeTypes, AlphabeticNotation>();
        foreach(SpecialUpgradeTypes type in Enum.GetValues(typeof(SpecialUpgradeTypes))){
            specialProductionPower[type] = defaultSpecialProductionPower;
        }

    }
    private void InitializeTime(){
        upgradeTime = new Dictionary<TimeUpgradeTypes, float>();
        foreach(TimeUpgradeTypes type in Enum.GetValues(typeof(TimeUpgradeTypes))){
            upgradeTime[type] = defaultTimePower;
        }
    }

    private void InitializeUnlocks(){
        activationUnlocks = new Dictionary<ActivationUnlocks, bool>();
        foreach(ActivationUnlocks type in Enum.GetValues(typeof(ActivationUnlocks))){
            activationUnlocks[type] = defaultActivationUnlock;
        }
    }



    // ------------------------ STORAGE UPGRADE TYPES --------------------- //
    public AlphabeticNotation GetProductionPower(CurrencyTypes type) => productionPower[type];
    public AlphabeticNotation GetStoragePower(CurrencyTypes type) => storagePower[type];

    // ------------------------ POWER UPGRADE TYPES --------------------- //
    public void AddPowerFlat(CurrencyTypes type, AlphabeticNotation amount) => productionPower[type] += amount;
    public void AddPowerMulti(CurrencyTypes type, AlphabeticNotation multiplier)
    {
        AlphabeticNotation result = (AlphabeticNotation)productionPower[type] * multiplier;
        productionPower[type] = result;
    }
    public void AddStorageAmountFlat(CurrencyTypes type, AlphabeticNotation amount)
    {
        storagePower[type] += amount;
        StorageManager.Instance.AddStorageAmount(type, amount);
    }
    public void AddStorageAmountMulti(CurrencyTypes type, AlphabeticNotation multiplier)
    {
        AlphabeticNotation result = (AlphabeticNotation)storagePower[type] * multiplier;
        storagePower[type] = result;
        StorageManager.Instance.AddStorageAmount(type, storagePower[type]);
    }
    // ------------------------ SPECIAL UPGRADE TYPES --------------------- //
     public AlphabeticNotation GetSpecialProductionAmount(SpecialUpgradeTypes type) => specialProductionPower[type];

    public void AddSpecialPowerFlat(SpecialUpgradeTypes type, AlphabeticNotation amount) => specialProductionPower[type] += amount;

    public void AddSpecialPowerMulti(SpecialUpgradeTypes type, double multiplier)
    {
        AlphabeticNotation result = specialProductionPower[type] * multiplier;
        specialProductionPower[type] = result;
    }
    // ------------------------ TIME UPGRADE TYPES --------------------- //
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
    // ------------------------ ACTIVATION UNLOCK TYPES --------------------- //
    public bool GetActivationUnlock(ActivationUnlocks type) => activationUnlocks[type];
    public void SetActivationUnlock(ActivationUnlocks type, bool state)
    {
        activationUnlocks[type] = state;
        OnActivationUnlock?.Invoke();
    }
    public void Save(ref upgradeManagerSaveData data)
    {
        data.activationUnlocks = new List<ActivationDataList>();
        data.productionData = new List<UpgradeDataList>();
        data.storageData = new List<UpgradeDataList>();
        data.timeData = new List<UpgradeDataList>();

        foreach (var pair in activationUnlocks){
            data.activationUnlocks.Add(new ActivationDataList{
                type = pair.Key.ToString(),
                state = pair.Value,
            });
        }
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
                productionPower[type] = new AlphabeticNotation(Double.Parse(entry.amount));
        }

        foreach (var entry in data.storageData)
        {
            if (Enum.TryParse(entry.type, out CurrencyTypes type))
                storagePower[type] = new AlphabeticNotation(Double.Parse(entry.amount));
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
    public List<ActivationDataList> activationUnlocks;
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
