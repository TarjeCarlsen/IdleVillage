using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
public class CookingHandler : MonoBehaviour
{


    [SerializeField] private Image icon_img;
    [SerializeField] private TMP_Text header_txt;
    [SerializeField] private List<TMP_Text> prices;
    [SerializeField] private List<TMP_Text> yields;
    [SerializeField] private List<RecipeSelection> recipes;
    private KitchenManager kitchenManager;
    private KitchenManager.RecipeState recipeState;
    public bool isCooking = false;
    [SerializeField] private GeneratorAdvanced generatorAdvanced;

    [SerializeField] private EnergyConsumptionHandler energyConsumptionHandler;
    [SerializeField] private float eneryConsumptionTimeIntervall = 30f;
    private bool manualActivated = false;
    private bool autoActivated = false;
    private Recipes currentSelectedRecipe;
    [SerializeField] public string uniqueId;


    private void Awake()
    {
        kitchenManager = GameObject.FindGameObjectWithTag("KitchenPage").GetComponent<KitchenManager>();
        uniqueId = gameObject.name;
    }

    private void OnEnable()
    {
        kitchenManager.OnnewRecipeUnlocked += UpdateAvailableRecipes;
        energyConsumptionHandler.EnergyExausted += StopGeneratingAuto;
        energyConsumptionHandler.EnergyReStarted += ReStartAuto;
        generatorAdvanced.OnManualFinish += ManualFinished;
    }
    private void OnDisable()
    {
        kitchenManager.OnnewRecipeUnlocked -= UpdateAvailableRecipes;
        generatorAdvanced.OnManualFinish -= ManualFinished;
        energyConsumptionHandler.EnergyExausted -= StopGeneratingAuto;
        energyConsumptionHandler.EnergyReStarted -= ReStartAuto;

    }
    public void SelectedRecipe(Recipes recipes)
    {
        currentSelectedRecipe = recipes;
        recipeState = kitchenManager.GetRecipe(recipes);
        generatorAdvanced.ClearGenAdvancedInfos();
        generatorAdvanced.EditGenAdvancedInfos(recipeState.recipe_datas.recipeYield.generateInfo, recipeState.recipe_datas.recipeYield.payInfo);
        icon_img.sprite = recipeState.recipe_datas.image;
        UpdateDescription();
    }



    public void OnStartCookingClick()
    {
        if (recipeState == null || recipeState.recipe_datas == null) return;
        if (autoActivated)
        {
        }
        else
        {

            generatorAdvanced.StartGenerating(recipeState.recipe_datas.defaultCookingTime); //change this out when upgradeable time comes
            manualActivated = true;
        }
    }

    private void ManualFinished()
    {
        manualActivated = false;
    }

    public void OnStartAutoCookingClick()
    {
        if (manualActivated)
        {
            if (recipeState == null || recipeState.recipe_datas == null) return;
            print($"manual = {manualActivated} auto = {autoActivated}");
            print("transition");
            generatorAdvanced.TransitionToAuto();
            energyConsumptionHandler.OnStartEnergyRoutine(eneryConsumptionTimeIntervall);
        }
        else
        {

            if (energyConsumptionHandler.GetEnergyState())
            {
                print("stopauto");
                StopGeneratingAuto();
                energyConsumptionHandler.OnStopEnergyRoutine();
                autoActivated = false;
            }
            else
            {
                if (energyConsumptionHandler.CanAfford())
                {
                    print($"inside start auto routine");
                    if (recipeState == null || recipeState.recipe_datas == null) return; // add popup for "Chose recipe"
                    autoActivated = true;
                    energyConsumptionHandler.OnStartEnergyRoutine(eneryConsumptionTimeIntervall);
                    generatorAdvanced.StartGeneratingAuto(recipeState.recipe_datas.defaultCookingTime);
                }
            }
        }
    }


    // if (manualActivated)
    // {
    //     generatorAdvanced.stopRequested = true;
    // }
    // if (isCooking == true)
    // {
    //     generatorAdvanced.stopRequested = true;
    //     energyConsumptionHandler.OnStopEnergyRoutine();
    //     autoActivated = false;
    //     isCooking = false;
    // }
    // else
    // {

    //     if (recipeState == null || recipeState.recipe_datas == null) return;
    //     autoActivated = true;
    //     isCooking = true;
    //     generatorAdvanced.StartGeneratingAuto(recipeState.recipe_datas.defaultCookingTime); //change this out when upgradeable time comes
    // }



    private void StopGeneratingAuto()
    {
        generatorAdvanced.stopRequested = true;
    }

    public void ReStartAuto()
    {
        energyConsumptionHandler.OnStartEnergyRoutine(eneryConsumptionTimeIntervall);
        generatorAdvanced.stopRequested = false;
        generatorAdvanced.StartGeneratingAuto(recipeState.recipe_datas.defaultCookingTime);
    }


    private void UpdateAvailableRecipes(Recipes _recipe, bool state)
    {
        foreach (RecipeSelection selected in recipes)
        {
            if (selected.selectedRecipe == _recipe)
            {
                selected.gameObject.SetActive(state);
            }
        }
    }

    private void UpdateDescription()
    {
        foreach (TMP_Text payDescription in prices)
        {
            payDescription.gameObject.SetActive(false);
        }
        foreach (TMP_Text genDescription in yields)
        {
            genDescription.gameObject.SetActive(false);
        }
        if (recipeState.recipe_datas != null)
        {

            header_txt.text = recipeState.recipe_datas.recipeName;
            int lengthOfPayments = recipeState.recipe_datas.recipeYield.payInfo.Count;
            int lengtOfYield = recipeState.recipe_datas.recipeYield.generateInfo.Count;
            generatorAdvanced.UpdateTime(recipeState.recipe_datas.defaultCookingTime);

            for (int i = 0; i < lengthOfPayments; i++)
            {
                prices[i].gameObject.SetActive(true);
                prices[i].text = recipeState.recipe_datas.recipeYield.payDescription[i];
            }
            for (int i = 0; i < lengtOfYield; i++)
            {
                yields[i].gameObject.SetActive(true);
                yields[i].text = recipeState.recipe_datas.recipeYield.generateDescription[i];
            }

        }
    }


    public void Save(ref CookingHandlerSaveData data)
    {
        data.selectedRecipe = currentSelectedRecipe;
        data.autoActivated = autoActivated;
        data.manualActivated = manualActivated;
        data.uniqueId = uniqueId;
        data.timeRemaining = generatorAdvanced.GetTimeRemaining();
    }
    public void Load(CookingHandlerSaveData data)
    {
        if (data.uniqueId != uniqueId) return;
        generatorAdvanced.StopGenerating();
        energyConsumptionHandler.OnStopEnergyRoutine();
        autoActivated = data.autoActivated;
        manualActivated = data.manualActivated;
        foreach (KitchenManager.RecipeState recipeState in kitchenManager.allRecipes)
        {
            if (recipeState.recipe_datas.recipe == data.selectedRecipe)
            {
                if (recipeState.isUnlocked)
                {
                    SelectedRecipe(data.selectedRecipe);
                }
            }
        }


            if(data.autoActivated)
            {
                if (recipeState == null || recipeState.recipe_datas == null) return; // add popup for "Chose recipe"
                print($"inside start auto routine");
                energyConsumptionHandler.OnStartEnergyRoutine(eneryConsumptionTimeIntervall);
                generatorAdvanced.ResumeGeneration(recipeState.recipe_datas.defaultCookingTime,data.timeRemaining,true);
            }
            else if (data.manualActivated)
            {
                if (recipeState == null || recipeState.recipe_datas == null) return;
                print("manual");
                generatorAdvanced.StartGenerating(recipeState.recipe_datas.defaultCookingTime);
            }
                else
                {
                    print("stopauto");
                    StopGeneratingAuto();
                    energyConsumptionHandler.OnStopEnergyRoutine();
                }
        
        // else
        // {
        //     print($"inside else statement");
        //     generatorAdvanced.StopGenerating();
        //     energyConsumptionHandler.OnStopEnergyRoutine();
        //     autoActivated = false;
        //     isCooking = false;
        // }

    }
}

[System.Serializable]
public struct CookingHandlerSaveData
{
    public Recipes selectedRecipe;
    public bool manualActivated;
    public bool autoActivated;
    public string uniqueId;
    public float timeRemaining;
}