using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceFarm : MonoBehaviour
{
    [SerializeField] private UpgradeHandler upgradeHandler;
    
    public List<GameObject> tractors;

    private void Awake(){
        upgradeHandler = GameObject.FindGameObjectWithTag("ShopPage").GetComponent<UpgradeHandler>();
    }


    private void OnEnable(){
        upgradeHandler.OnFarmUpgradeBought += enableUpgrades;
    }
    private void OnDisable(){
        upgradeHandler.OnFarmUpgradeBought -= enableUpgrades;

    }

    private void enableUpgrades(UpgradeIDGlobal id, IsWhatDatatype datatype, CurrencyTypes currencyTypes){
        switch(id){
            case UpgradeIDGlobal.tractorActivation:
            foreach(GameObject tractor in tractors){
                tractor.SetActive(true);
            }
                break;
        }
    }



}
