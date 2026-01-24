using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using UnityEngine;

public class ResourceFarm : MonoBehaviour
{
    [SerializeField] private UpgradeHandler upgradeHandler;
    [SerializeField] private List<GeneratorResources> generatorResources;
    
    public List<GameObject> tractors;
    [SerializeField] private GeneratorAdvanced energyComsumption;
    private bool autoRunning;
    private bool manualIsRunning;
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
}
