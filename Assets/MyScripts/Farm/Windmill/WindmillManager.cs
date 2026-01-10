using System;
using LargeNumbers;
using TMPro;
using UnityEngine;

public class WindmillManager : MonoBehaviour// REMOVED GRAIN, SCRIPT DOES NOT WORK ANYMORE
{
    [SerializeField] private TMP_Text outputPerMinGrain_txt;
    [SerializeField] private TMP_Text outputPerMinFlour_txt;
    private int amountOfActiveGenerators;
    private AlphabeticNotation totalAmountGeneration;
    [SerializeField] private GeneratorSimple[] generatorSimple;
    private AlphabeticNotation grainOutput;
    private AlphabeticNotation flourOutput;
    [SerializeField] private Animator windmillAnimator;

    private void OnEnable()
    {
        foreach (var generator in generatorSimple)
        {

            generator.OnAutoGenerationStarted += HandleGeneratorStarted;
            generator.OnAutoGenerationStopped += HandleGeneratorStopped;
        }

    }
    private void OnDisable()
    {
        foreach (var generator in GetComponentsInChildren<GeneratorSimple>())
        {
            generator.OnAutoGenerationStarted -= HandleGeneratorStarted;
            generator.OnAutoGenerationStopped -= HandleGeneratorStopped;
        }

    }

    private void HandleGeneratorStarted(CurrencyTypes generatedType)
    {
        amountOfActiveGenerators++;
        var production = UpgradeManager.Instance.GetProductionPower(generatedType);

        totalAmountGeneration += production;

        // if (generatedType == CurrencyTypes.grain) // REMOVED GRAIN, SCRIPT DOES NOT WORK ANYMORE
        //     grainOutput += production;
        // else if (generatedType == CurrencyTypes.flour)
        //     flourOutput += production;

        UpdateUI();
    }

    private void HandleGeneratorStopped(CurrencyTypes type)
    {
        amountOfActiveGenerators--;
        var production = UpgradeManager.Instance.GetProductionPower(type);

        totalAmountGeneration -= production;

        // if (type == CurrencyTypes.grain) // REMOVED GRAIN, SCRIPT DOES NOT WORK ANYMORE
        //     grainOutput -= production;
        // else if (type == CurrencyTypes.flour)
        //     flourOutput -= production;

        UpdateUI();
    }

    private void UpdateUI()
    {
        outputPerMinGrain_txt.text = "Output:"+ HelperFunctions.Instance.CalculateOutputPerMin(grainOutput, UpgradeManager.Instance.GetTimePower(TimeUpgradeTypes.windmillGrainTime)).ToString()+"/min";
        outputPerMinFlour_txt.text ="Output: " +HelperFunctions.Instance.CalculateOutputPerMin(flourOutput, UpgradeManager.Instance.GetTimePower(TimeUpgradeTypes.windmillGrainTime)).ToString()+"/min";
        if(flourOutput > 0 || grainOutput > 0){
            windmillAnimator.SetBool("isGenerating", true);
        }else{
            windmillAnimator.SetBool("isGenerating", false);
        }
    }
}
