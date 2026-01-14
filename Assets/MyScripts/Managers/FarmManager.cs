using System;
using UnityEngine;
using UnityEngine.Scripting;

public class FarmManager : MonoBehaviour
{

public event Action <UpgradeIDGlobal,IsWhatDatatype,CurrencyTypes> OnFarmUpgradeBought;

private void Awake(){
    
}

    public void OnUpgradeBought(UpgradeIDGlobal id, IsWhatDatatype datatype, CurrencyTypes currencyTypes){
        switch(id){
            case UpgradeIDGlobal.tractorActivation:
            OnFarmUpgradeBought?.Invoke(id,datatype,currencyTypes);
            break;
        }
    }



}
