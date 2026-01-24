using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceFarm : MonoBehaviour
{
    [SerializeField] private UpgradeHandler upgradeHandler;
    [SerializeField] private EnergyConsumptionHandler energyConsumptionHandler;
    [SerializeField] private List<GeneratorResources> generatorResources;
    [SerializeField] private float timeStartupAuto;    
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
            if(!generator.gameObject.activeInHierarchy){ 
                continue;
            }
            if(generator != null && !generator.isGeneratorRunning() ){
                generator.StartGenerating(upgradeHandler.productionTimes[generator.typeToGenerate]);
                return;
            }
        }
    }
    
    public void OnGenerateAutoClicked(){
        if(energyConsumptionHandler.GetEnergyState()){
            foreach(GeneratorResources generator in generatorResources){
            if(!generator.gameObject.activeInHierarchy){ 
                continue;
            }
            if(generator != null && !generator.isGeneratorRunning() ){
                generator.StopGenerating();
                return;
            }
        }
            
        }else{
            foreach(GeneratorResources generator in generatorResources){
            if(!generator.gameObject.activeInHierarchy){ 
                continue;
            }
            if(generator != null && !generator.isGeneratorRunning() ){
                generator.StartGenerating(upgradeHandler.productionTimes[generator.typeToGenerate]);
                return;
            }
        }
    }
}
}
    

    

        //     if (manualActivated)
        // {
        //     generatorAdvanced.StopGenerating();
        // }
        // if (isCooking == true)
        // {
        //     generatorAdvanced.StopGenerating();
        //     energyConsumptionGenerator.StopGenerating();
        //     autoActivated = false;
        //     isCooking = false;
        // }
        // else
        // {

        //     if (recipeState == null || recipeState.recipe_datas == null) return;
        //     autoActivated = true;
        //     isCooking = true;
        //     StartEnergyConsumption();
        //     generatorAdvanced.StartGeneratingAuto(recipeState.recipe_datas.defaultCookingTime); //change this out when upgradeable time comes
        // }
