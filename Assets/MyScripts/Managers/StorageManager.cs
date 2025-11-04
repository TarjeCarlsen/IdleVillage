using System;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using LargeNumbers;

public enum SpecialStorageType{
    furnaceStorageCap,
    flourPerDoughCap,
    shopAmountListings,
}


[System.Serializable]
public struct TESTINGSTORAGE{
    public CurrencyTypes currencyType;
    public AlphabeticNotation testNumber;
}
[System.Serializable]
public struct TESTINGSPECIALSTORAGE{
    public SpecialStorageType specialStorageType;
    public AlphabeticNotation testNumber;
}

public class StorageManager : MonoBehaviour
{
    public static StorageManager Instance {get;private set;}


    public event Action <CurrencyTypes> OnStorageChange;
    [SerializeField] private List<TESTINGSTORAGE> SpecificStartStorage_TESTING;
    [SerializeField] private List<TESTINGSPECIALSTORAGE> SpecificSpecialStartStorage_TESTING;
    public Dictionary<CurrencyTypes, AlphabeticNotation> storageAmount = new();
    public Dictionary<CurrencyTypes, AlphabeticNotation> storageUnit = new();
    public Dictionary<SpecialStorageType, AlphabeticNotation> specialStorageAmount = new();
    [SerializeField] private AlphabeticNotation defaultStartStorageAmount;
    [SerializeField] private AlphabeticNotation defaultStartStorageUnits;
    [SerializeField] private AlphabeticNotation defaultSpecialStorageAmount;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        InitializeStorage();
        InitializeSpecialStorage();
        UpdateStorage();
    }

    private void InitializeStorage()
    {
        foreach (CurrencyTypes type in Enum.GetValues(typeof(CurrencyTypes)))
        {
            var testEntry = SpecificStartStorage_TESTING.Find(e => e.currencyType == type);

            if (testEntry.testNumber != 0){

                storageAmount[type] = testEntry.testNumber;
                storageUnit[type] = defaultStartStorageUnits;
            }
            else{

                storageAmount[type] = defaultStartStorageAmount;
                storageUnit[type] = defaultStartStorageUnits;
            }
        }
    }
    public void InitializeSpecialStorage(){
        foreach(SpecialStorageType type in Enum.GetValues(typeof(SpecialStorageType))){
                        var testEntry = SpecificSpecialStartStorage_TESTING.Find(e => e.specialStorageType == type);

            if (testEntry.testNumber != 0){

                specialStorageAmount[type] = testEntry.testNumber;
            }
            else{

                specialStorageAmount[type] = defaultSpecialStorageAmount;
            }
        }
    }

    public void UpdateStorage(){
        foreach(CurrencyTypes type in Enum.GetValues(typeof(CurrencyTypes))){
            AddStorageAmount(type,new AlphabeticNotation(0));
            AddStorageUnits(type, new AlphabeticNotation(0));
        }
    }

    public AlphabeticNotation GetMaxStorage (CurrencyTypes type) => storageAmount[type] * storageUnit[type];

    public AlphabeticNotation GetStorageUnits(CurrencyTypes type) => storageUnit[type];

    public void AddStorageAmount(CurrencyTypes type, AlphabeticNotation amount){
        storageAmount[type] += amount;
        OnStorageChange?.Invoke(type);
    }

    public void AddStorageUnits(CurrencyTypes type, AlphabeticNotation amount){
        storageUnit[type] += amount;
        OnStorageChange?.Invoke(type);
    }

    public void SetStorageUnits(CurrencyTypes type, AlphabeticNotation amount){
        storageUnit[type] = amount;
        OnStorageChange?.Invoke(type);
    }

    public AlphabeticNotation GetMaxSpecialStorage(SpecialStorageType type) => specialStorageAmount[type];
    public void AddSpecialStorageAmount(SpecialStorageType type, AlphabeticNotation amount){
        specialStorageAmount[type] += amount;
    }
    public void SetSpecialStorageAmount(SpecialStorageType type,AlphabeticNotation amount) => specialStorageAmount[type] = amount;


    public void Save(ref StorageManagerSaveData data)
    {
        data.storageData = new List<StorageDataList>();

        foreach (var entry in storageAmount)
        {
            CurrencyTypes type = entry.Key;
            AlphabeticNotation amount = entry.Value;

            AlphabeticNotation units = storageUnit.ContainsKey(type) ? storageUnit[type] : new AlphabeticNotation(0);

            data.storageData.Add(new StorageDataList
            {
                type = type.ToString(),
                amount = amount.ToString(),
                units = units
            });
        }

        Debug.Log($"[StorageManager] Saved {data.storageData.Count} entries.");

    }
    public void Load(StorageManagerSaveData data)
    {
        storageAmount.Clear();
        storageUnit.Clear();

        foreach (var entry in data.storageData)
        {
            if (Enum.TryParse(entry.type, out CurrencyTypes type))
            {
                AlphabeticNotation.GetAlphabeticNotationFromString(entry.amount, out var parsed);
                storageAmount[type] = parsed;
                storageUnit[type] = entry.units;
                OnStorageChange?.Invoke(type);
            }
        }

        // Ensure new currencies exist even if save is old
        foreach (CurrencyTypes type in Enum.GetValues(typeof(CurrencyTypes)))
        {
            if (!storageAmount.ContainsKey(type)) storageAmount[type] = defaultStartStorageAmount;
            if (!storageUnit.ContainsKey(type)) storageUnit[type] = defaultStartStorageUnits;
        }

        Debug.Log($"[StorageManager] Loaded {storageAmount.Count} entries.");
    }

    }




[System.Serializable]
public struct StorageManagerSaveData
{
    public List<StorageDataList> storageData;

}

[Serializable]
public struct StorageDataList{
    public string type;
    public string amount;
    public AlphabeticNotation units;
}
