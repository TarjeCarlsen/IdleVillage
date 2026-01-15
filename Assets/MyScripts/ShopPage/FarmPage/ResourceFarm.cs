using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceFarm : MonoBehaviour
{
    [SerializeField] private FarmManager farmManager;
    
    public List<GameObject> tractors;

    private void Awake(){
        farmManager = GameObject.FindGameObjectWithTag("ShopPage").GetComponent<FarmManager>();
    }


    private void OnEnable(){
        farmManager.OnFarmUpgradeBought += enableUpgrades;
    }
    private void OnDisable(){
        farmManager.OnFarmUpgradeBought -= enableUpgrades;

    }

    private void enableUpgrades(UpgradeIDGlobal id, IsWhatDatatype datatype, CurrencyTypes currencyTypes){
        switch(id){
            case UpgradeIDGlobal.tractorActivation:
            foreach(GameObject tractor in tractors){
                tractor.SetActive(true);
            }
                break;
            case UpgradeIDGlobal.resourceGenerationTime_Multiplier:
                
                break;
        }
    }



}
