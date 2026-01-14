using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

public class FarmManager : MonoBehaviour
{
    public Dictionary<CurrencyTypes,float> defaultProdTimes;
    public Dictionary<CurrencyTypes, float> productionTimes;

public event Action <UpgradeIDGlobal,IsWhatDatatype,CurrencyTypes> OnFarmUpgradeBought;

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
    }

    private void CalculateTime(UpgradeIDGlobal id,CurrencyTypes type){
        float time = 0f;
        time = defaultProdTimes[type] * UpgradeManager.Instance.GetFloat(id,type);
        productionTimes[type]  = time;
        print("inside calculate" + time);
        print($"multiplier = {UpgradeManager.Instance.GetFloat(UpgradeIDGlobal.resourceGenerationTime_Multiplier, type)}");
    }


}
