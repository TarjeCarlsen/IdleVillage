using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using LargeNumbers;
using Mono.Cecil.Cil;
using Unity.VisualScripting;
using UnityEngine;


public enum UpgradeOperation
{
    Add,
    Get,
    Subtract,
    Set,
}

public enum UpgradeValueType
{
    Float,
    Int,
    Alphabetic,
    Bool,
}

public class UpgradeValue{
    public UpgradeValueType type;
    public float floatValue;
    public int intValue;
    public AlphabeticNotation alphabetic;
    public bool boolState;

    public object Get () =>
    type switch
     {
        UpgradeValueType.Alphabetic => alphabetic,
        UpgradeValueType.Float => floatValue,
        UpgradeValueType.Bool => boolState,
        UpgradeValueType.Int => intValue,
        _ => null
    };

    public void Add(object amount){
        switch(type){
            case UpgradeValueType.Alphabetic:
            alphabetic += (AlphabeticNotation)amount;
            break;
            case UpgradeValueType.Bool:
            boolState = (bool)amount;
            break;
            case UpgradeValueType.Float:
            floatValue += (float )amount;
            break;
            case UpgradeValueType.Int:
            intValue += (int)amount;
            break;
        }
    }

    public void Sub(object amount){
        switch(type){
            case UpgradeValueType.Alphabetic:
            alphabetic -= (AlphabeticNotation)amount;
            break;
            case UpgradeValueType.Float:
            floatValue -= (float)amount;
            break;
            case UpgradeValueType.Int:
            intValue -= (int)amount;
            break;
        }
    }
        public void Set(object amount)
    {
        switch (type)
        {
            case UpgradeValueType.Float:
                floatValue = (float)amount;
                break;
            case UpgradeValueType.Int:
                intValue = (int)amount;
                break;
            case UpgradeValueType.Alphabetic:
                alphabetic = (AlphabeticNotation)amount;
                break;
            case UpgradeValueType.Bool:
                boolState = (bool)amount;
                break;
        }
    }
}
public enum UpgradeIDGlobal
{
    productionPower,
    market_time_between_customers,
    tractorActivation,
}



public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance {get;private set;}
    [SerializeField] public Dictionary<(UpgradeIDGlobal,CurrencyTypes),UpgradeValue> upgrades;



public void Awake(){
   if(Instance == null) Instance = this;
   else{Destroy (gameObject);}
    InitializeUpgradeTypes();
}


private void InitializeUpgradeTypes(){
upgrades = new Dictionary<(UpgradeIDGlobal, CurrencyTypes), UpgradeValue>();

 foreach (CurrencyTypes currency in Enum.GetValues(typeof(CurrencyTypes))){
    //add upgrades like so:
    //upgrades[(UpgradeIDGlobal.rewardmulti, currency)] = new Upgradevalue {type = UpgradeValueType.Float, floatValue = 1f};
    upgrades[(UpgradeIDGlobal.productionPower,currency)] = new UpgradeValue {type = UpgradeValueType.Alphabetic, alphabetic = new AlphabeticNotation(1f)};
 }
    upgrades[(UpgradeIDGlobal.market_time_between_customers, CurrencyDummy.Dummy)] = new UpgradeValue{type =  UpgradeValueType.Float, floatValue = 1f};
    upgrades[(UpgradeIDGlobal.tractorActivation, CurrencyDummy.Dummy)] = new UpgradeValue{type =  UpgradeValueType.Bool, boolState = true};
}

public UpgradeValue Modify(
    UpgradeIDGlobal id,
    UpgradeOperation op,
    CurrencyTypes currencyTypes,
    object amount = null
){
var value = upgrades[(id,currencyTypes)];

switch(op){
    case UpgradeOperation.Add:
    value.Add(amount);
    return value;

    case UpgradeOperation.Get:
    return value;

    case UpgradeOperation.Subtract:
    value.Sub(amount);
    return value;

    case UpgradeOperation.Set:
    value.Set(amount);
    return value;

    default:
    return value;
}

}


public float GetFloat(UpgradeIDGlobal id,CurrencyTypes currencyTypes) => upgrades[(id,currencyTypes)].floatValue;
public int GetInt(UpgradeIDGlobal id,CurrencyTypes currencyTypes) => upgrades[(id,currencyTypes)].intValue;
public AlphabeticNotation GetAlphabetic(UpgradeIDGlobal id,CurrencyTypes currencyTypes) => upgrades[(id,currencyTypes)].alphabetic;
public bool GetBool(UpgradeIDGlobal id,CurrencyTypes currencyTypes) => upgrades[(id,currencyTypes)].boolState;

}
