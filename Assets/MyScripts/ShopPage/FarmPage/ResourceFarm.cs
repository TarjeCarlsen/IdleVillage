using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using UnityEngine;

public class ResourceFarm : MonoBehaviour
{
    [SerializeField] private UpgradeHandler upgradeHandler;
    [SerializeField] private List<GeneratorResources> generatorResources;
    
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

    public void OnGenerateButtonClicked(){
        foreach(GeneratorResources generator in generatorResources){
            if(!generator.gameObject.activeInHierarchy){ // skip inactive generators
                continue;
            }
            if(generator != null && !generator.isGeneratorRunning() ){
                generator.StartGenerating(upgradeHandler.productionTimes[generator.typeToGenerate]);
                return;
            }
        }
    }

}
