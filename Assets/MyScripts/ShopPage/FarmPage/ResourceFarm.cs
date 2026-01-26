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
    [SerializeField] private float energyConsumptionTime = 5f;
    public List<GameObject> tractors;
    private void Awake()
    {
        upgradeHandler = GameObject.FindGameObjectWithTag("ShopPage").GetComponent<UpgradeHandler>();
    }


    private void OnEnable()
    {
        upgradeHandler.OnFarmUpgradeBought += enableUpgrades;
        energyConsumptionHandler.EnergyExausted += StopGenerating;
        energyConsumptionHandler.EnergyReStarted += ReStartAuto;
    }
    private void OnDisable()
    {
        upgradeHandler.OnFarmUpgradeBought -= enableUpgrades;
        energyConsumptionHandler.EnergyExausted -= StopGenerating;
        energyConsumptionHandler.EnergyReStarted -= ReStartAuto;

    }

    //add upgrades for farm like unlockable auto generation
    private void enableUpgrades(UpgradeIDGlobal id, IsWhatDatatype datatype, CurrencyTypes currencyTypes)
    {
        switch (id)
        {
            case UpgradeIDGlobal.tractorActivation:
                foreach (GameObject tractor in tractors)
                {
                    tractor.SetActive(true);
                }
                break;
        }
    }

    public void OnGenerateButtonClicked()
    {
        foreach (GeneratorResources generator in generatorResources)
        {
            if (!generator.gameObject.activeInHierarchy)
            {
                continue;
            }
            if (generator != null && !generator.isGeneratorRunning())
            {
                generator.StartGenerating(upgradeHandler.productionTimes[generator.typeToGenerate]);
                return;
            }
        }
    }

    public void OnGenerateAutoClicked()
    {
        if (energyConsumptionHandler.GetEnergyState())
        {
            StopGenerating(false); // boolean is not used here, used in cookinghandler
            energyConsumptionHandler.OnStopEnergyRoutine();
        }
        else
        {
            if(energyConsumptionHandler.CanAfford()){

            energyConsumptionHandler.OnStartEnergyRoutine(energyConsumptionTime);
            foreach (GeneratorResources generator in generatorResources)
            {
                if (generator != null && !generator.isGeneratorRunning())
                {
                    generator.StartGeneratingAuto(upgradeHandler.productionTimes[generator.typeToGenerate]);
                }
            }
            }
        }
    }

    private void StopGenerating(bool isEnergyExhausted)
    {
        foreach (GeneratorResources generator in generatorResources)
        {
            if (generator != null)
            {
                generator.stopRequested = true;
            }
        }
    }
    private void ReStartAuto()
    {
        energyConsumptionHandler.OnStartEnergyRoutine(energyConsumptionTime);
        foreach (GeneratorResources generator in generatorResources)
        {
            if (generator != null)
            {
                generator.StartGeneratingAuto(upgradeHandler.productionTimes[generator.typeToGenerate]);
                generator.stopRequested = false;
            }
        }
    }
}