using System;
using System.Collections.Generic;
using System.Numerics;
using NUnit.Framework.Constraints;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;



public enum CurrencyTypes{
    money,
    wheat,
    grain,
    dough,
    bread,
    child,
    farmer,
    electrician,
    miner,
    adventurer,
}

[System.Serializable]
public struct TESTINCURRENCIES{
    public CurrencyTypes currencyType;
    public BigNumber testNumber;
}
public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance {get; private set;}

    public Dictionary<CurrencyTypes, BigNumber> currency;
    [SerializeField] private List<TESTINCURRENCIES> SpecificStartValue_TESTING;
    [SerializeField] private BigNumber defaultStartMoney = 1;

    public event Action<CurrencyTypes>OnCurrencyChanged;

    private void Awake(){
        if(Instance != null && Instance != this){
            Destroy(gameObject);
            return;
        }
        Instance = this;    
        InitializeCurrencies();
    }

    private void Start(){
        foreach(CurrencyTypes type in Enum.GetValues(typeof(CurrencyTypes))){
            OnCurrencyChanged?.Invoke(type);
        }
    }

    private void InitializeCurrencies()
    {
        currency = new Dictionary<CurrencyTypes, BigNumber>();

        foreach (CurrencyTypes type in Enum.GetValues(typeof(CurrencyTypes)))
        {
            // Find if there’s a test value in the serialized list
            var testEntry = SpecificStartValue_TESTING.Find(e => e.currencyType == type);

            if (testEntry.testNumber.number != 0)
                currency[type] = testEntry.testNumber;
            else
                currency[type] = defaultStartMoney;
        }
    }


    public BigNumber GetCurrency(CurrencyTypes type) => currency[type];
    public void UpdateCurrencies(){
            foreach (CurrencyTypes types in Enum.GetValues(typeof(CurrencyTypes)))
        {
            AddCurrency(types,0);
        }
    }

    public void AddCurrency(CurrencyTypes type, BigNumber amount){ // --------- NEW
        var current = currency[type];
        BigNumber maxStorage = StorageManager.Instance.GetMaxStorage(type); // -------- ADD BACK WHEN HAVE STORAGE MANAGER
        // BigNumber maxStorage = 2000;
        if((currency[type] + amount) > maxStorage){
            currency[type] = maxStorage;
        }else{
            currency[type] += amount;
        }
        OnCurrencyChanged?.Invoke(type);
    }

    public void SubtractCurrency(CurrencyTypes type, BigNumber amount){
        currency[type] -= amount;
        OnCurrencyChanged?.Invoke(type);

    }


    public void Save(ref CurrencySaveData data)
    {
        data.currencyData = new List<CurrencySaveList>();
        
        foreach( var curr in currency){
            data.currencyData.Add(new CurrencySaveList{
                type = curr.Key.ToString(),
                amount = curr.Value.ToString()
            });
        }
    }

    public void Load(CurrencySaveData data)
    {
        foreach(var element in data.currencyData){
            if(Enum.TryParse(element.type, out CurrencyTypes type)){
                currency[type] = BigNumber.Parse(element.amount);
                OnCurrencyChanged?.Invoke(type);
            }
        }
    }

}

[System.Serializable]
public struct CurrencySaveData
{
    public List<CurrencySaveList> currencyData;
}

[Serializable]
public struct CurrencySaveList{
    public string type;
    public string amount;
}
