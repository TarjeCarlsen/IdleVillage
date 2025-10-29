using System;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;


[System.Serializable]
public struct TESTINGSTORAGE{
    public CurrencyTypes currencyType;
    public BigNumber testNumber;
}

public class StorageManager : MonoBehaviour
{
    public static StorageManager Instance {get;private set;}


    public event Action <CurrencyTypes> OnStorageChange;
    [SerializeField] private List<TESTINGSTORAGE> SpecificStartStorage_TESTING;
    public Dictionary<CurrencyTypes, BigNumber> storageAmount = new();
    public Dictionary<CurrencyTypes, BigNumber> storageUnit = new();
    [SerializeField] private BigNumber defaultStartStorageAmount;
    [SerializeField] private BigNumber defaultStartStorageUnits;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        InitializeStorage();

        UpdateStorage();
    }

    private void InitializeStorage()
    {
        foreach (CurrencyTypes type in Enum.GetValues(typeof(CurrencyTypes)))
        {
            var testEntry = SpecificStartStorage_TESTING.Find(e => e.currencyType == type);

            if (testEntry.testNumber.number != 0){

                storageAmount[type] = testEntry.testNumber;
                storageUnit[type] = defaultStartStorageUnits;
            }
            else{

                storageAmount[type] = defaultStartStorageAmount;
                storageUnit[type] = defaultStartStorageUnits;
            }
        }
    }

    public void UpdateStorage(){
        foreach(CurrencyTypes type in Enum.GetValues(typeof(CurrencyTypes))){
            AddStorageAmount(type, 0);
            AddStorageUnits(type, 0);
        }
    }

    public BigNumber GetMaxStorage (CurrencyTypes type) => storageAmount[type] * storageUnit[type];

    public BigNumber GetStorageUnits(CurrencyTypes type) => storageUnit[type];

    public void AddStorageAmount(CurrencyTypes type, BigNumber amount){
        storageAmount[type] += amount;
        OnStorageChange?.Invoke(type);
    }

    public void AddStorageUnits(CurrencyTypes type, BigNumber amount){
        storageUnit[type] += amount;
        OnStorageChange?.Invoke(type);
    }

    public void SetStorageUnits(CurrencyTypes type, BigNumber amount){
        storageUnit[type] = amount;
        OnStorageChange?.Invoke(type);
    }



    public void Save(ref StorageManagerSaveData data)
    {
        data.storageData = new List<StorageDataList>();

        foreach (var entry in storageAmount)
        {
            CurrencyTypes type = entry.Key;
            BigNumber amount = entry.Value;

            BigNumber units = storageUnit.ContainsKey(type) ? storageUnit[type] : 0;

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
                storageAmount[type] = BigNumber.Parse(entry.amount);
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
    public BigNumber units;
}
