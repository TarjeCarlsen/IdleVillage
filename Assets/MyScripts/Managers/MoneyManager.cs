using System;
using System.Collections.Generic;
using UnityEngine;
using LargeNumbers;


public enum CurrencyTypes{
    money,
    wheat,
    flour,
    dough,
    bread,
    corn,
    tomato,
    carrot,
    pumpkin,
    energy,
    
}

public static class CurrencyDummy
{
    public const CurrencyTypes Dummy = CurrencyTypes.money;
}

[System.Serializable]
public struct TESTINCURRENCIES{
    public CurrencyTypes currencyType;
    public AlphabeticNotation testNumber;
}
public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance {get; private set;}

    public Dictionary<CurrencyTypes, AlphabeticNotation> currency;
    [SerializeField] private List<TESTINCURRENCIES> SpecificStartValue_TESTING;
    [SerializeField] private AlphabeticNotation defaultStartMoney = new AlphabeticNotation(1);

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
        currency = new Dictionary<CurrencyTypes, AlphabeticNotation>();

        foreach (CurrencyTypes type in Enum.GetValues(typeof(CurrencyTypes)))
        {
            // Find if there’s a test value in the serialized list
            var testEntry = SpecificStartValue_TESTING.Find(e => e.currencyType == type);

            if (testEntry.testNumber != 0){

                currency[type] = testEntry.testNumber;
                currency[type] = currency[type] * 1; // normalize the number with multiply by 1
            }
            else
                currency[type] = defaultStartMoney;
                currency[type] = currency[type] * 1; // normalize the number with multiply by 1
        }
    }

    public AlphabeticNotation GetCurrency(CurrencyTypes type) => currency[type];
    public void UpdateCurrencies(){
            foreach (CurrencyTypes types in Enum.GetValues(typeof(CurrencyTypes)))
        {
            AddCurrency(types,new AlphabeticNotation(0));
        }
    }

    public void AddCurrency(CurrencyTypes type, AlphabeticNotation amount){ // --------- NEW
        var current = currency[type];
        AlphabeticNotation maxStorage = StorageManager.Instance.GetMaxStorage(type); // -------- ADD BACK WHEN HAVE STORAGE MANAGER
        // AlphabeticNotation maxStorage = 2000;
        if((currency[type] + amount) > maxStorage){
            currency[type] = maxStorage;
        }else{
            currency[type] += amount;
        }
        OnCurrencyChanged?.Invoke(type);
    }

    public void SubtractCurrency(CurrencyTypes type, AlphabeticNotation amount){
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
                currency[type] =new AlphabeticNotation(Double.Parse(element.amount));
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
