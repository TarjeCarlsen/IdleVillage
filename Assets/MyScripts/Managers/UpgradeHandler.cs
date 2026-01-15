using System;
using System.Collections.Generic;
using LargeNumbers;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Scripting;

public class UpgradeHandler : MonoBehaviour
{
    public Dictionary<CurrencyTypes,float> defaultProdTimes;
    public Dictionary<CurrencyTypes, float> productionTimes;

public event Action <UpgradeIDGlobal,IsWhatDatatype,CurrencyTypes> OnFarmUpgradeBought;
public event Action <UpgradeIDGlobal,IsWhatDatatype,CurrencyTypes> OnMarketUpgradeBought;

public event Action <UpgradeIDGlobal, IsWhatDatatype,CurrencyTypes> OnAnyUpgrade;
public event Action  notfiyUpdate;

private void Awake(){
    InitializeProductionTimes();
}

private void InitializeProductionTimes(){
    productionTimes = new Dictionary<CurrencyTypes, float>();
    defaultProdTimes = new Dictionary<CurrencyTypes, float>();

    foreach(CurrencyTypes types in Enum.GetValues(typeof(CurrencyTypes))){
        productionTimes[types] = UpgradeManager.Instance.GetFloat(UpgradeIDGlobal.resourceGenerationTime,types);
        defaultProdTimes[types] = UpgradeManager.Instance.GetFloat(UpgradeIDGlobal.resourceGenerationTime,types);
    }
}

    public void OnUpgradeBought(UpgradeIDGlobal id, IsWhatDatatype datatype, CurrencyTypes currencyTypes){
        switch(id){
            case UpgradeIDGlobal.tractorActivation:
            OnFarmUpgradeBought?.Invoke(id,datatype,currencyTypes);
            break;
            case UpgradeIDGlobal.resourceGenerationTime_Multiplier:
            CalculateTimeFarm(id, currencyTypes);
            break;
            case UpgradeIDGlobal.listingInterestedUnlock:
            OnMarketUpgradeBought?.Invoke(id,datatype,currencyTypes);
            break;
        }
            notfiyUpdate?.Invoke();
            OnAnyUpgrade?.Invoke(id,datatype,currencyTypes);
    }

    private void CalculateTimeFarm(UpgradeIDGlobal id,CurrencyTypes type){
        float time = 0f;
        // print("multiplier = "+  UpgradeManager.Instance.GetFloat(id,type));
        time = defaultProdTimes[type] / UpgradeManager.Instance.GetFloat(id,type);
        productionTimes[type]  = time;

    }

    public AlphabeticNotation CalculateProduction(CurrencyTypes type){
        AlphabeticNotation result = new AlphabeticNotation(0f);
        AlphabeticNotation farmPower = UpgradeManager.Instance.GetAlphabetic(UpgradeIDGlobal.farmProductionPower,type);
        AlphabeticNotation currencyPower = UpgradeManager.Instance.GetAlphabetic(UpgradeIDGlobal.productionPower,type);
        float farmPowerMulti = UpgradeManager.Instance.GetFloat(UpgradeIDGlobal.farmProductionPowerMulti, type);
        float currencyMulti = UpgradeManager.Instance.GetFloat(UpgradeIDGlobal.productionPowerMulti, type);


        result = ((farmPower + currencyPower) * farmPowerMulti) * currencyMulti;
        return result;
    }


}
