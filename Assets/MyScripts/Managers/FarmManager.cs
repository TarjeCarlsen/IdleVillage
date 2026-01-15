using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

public class FarmManager : MonoBehaviour
{
    public Dictionary<CurrencyTypes,float> defaultProdTimes;
    public Dictionary<CurrencyTypes, float> productionTimes;

public event Action <UpgradeIDGlobal,IsWhatDatatype,CurrencyTypes> OnFarmUpgradeBought;
public event Action <UpgradeIDGlobal, IsWhatDatatype,CurrencyTypes> OnAnyUpgrade;

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
            CalculateTime(id, currencyTypes);
            break;
        }
            OnAnyUpgrade?.Invoke(id,datatype,currencyTypes);
    }

    private void CalculateTime(UpgradeIDGlobal id,CurrencyTypes type){
        float time = 0f;
        // print("multiplier = "+  UpgradeManager.Instance.GetFloat(id,type));
        time = defaultProdTimes[type] / UpgradeManager.Instance.GetFloat(id,type);
        print($"prodtime = { defaultProdTimes[type]} multi = {UpgradeManager.Instance.GetFloat(id,type)} time = {time}");
        productionTimes[type]  = time;

    }


}
