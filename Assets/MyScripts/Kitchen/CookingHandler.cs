using System.Collections;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.Analytics;
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

    [SerializeField] private GeneratorAdvanced energyConsumptionGenerator;
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
    private void Start()
    {
    }

    private void OnEnable()
    {
        kitchenManager.OnnewRecipeUnlocked += UpdateAvailableRecipes;
        energyConsumptionGenerator.OnCantAfford += StopGeneratingAuto;
        generatorAdvanced.OnManualFinish += ManualFinished;
    }
    private void OnDisable()
    {
        kitchenManager.OnnewRecipeUnlocked -= UpdateAvailableRecipes;
        energyConsumptionGenerator.OnCantAfford -= StopGeneratingAuto;
        generatorAdvanced.OnManualFinish -= ManualFinished;

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
        generatorAdvanced.StartGenerating(recipeState.recipe_datas.defaultCookingTime); //change this out when upgradeable time comes
        manualActivated = true;
    }

    private void ManualFinished()
    {
        manualActivated = false;
    }

    public void OnStartAutoCookingClick()
    {
        if (manualActivated)
        {
            generatorAdvanced.StopGenerating();
        }
        if (isCooking == true)
        {
            generatorAdvanced.StopGenerating();
            energyConsumptionGenerator.StopGenerating();
            autoActivated = false;
            isCooking = false;
        }
        else
        {

            if (recipeState == null || recipeState.recipe_datas == null) return;
            autoActivated = true;
            isCooking = true;
            StartEnergyConsumption();
            generatorAdvanced.StartGeneratingAuto(recipeState.recipe_datas.defaultCookingTime); //change this out when upgradeable time comes
        }
    }

    private void StartEnergyConsumption()
    {
        energyConsumptionGenerator.StartGeneratingAuto(eneryConsumptionTimeIntervall);
    }

    private void StopGeneratingAuto()
    {
        generatorAdvanced.StopGenerating();
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
        data.uniqueId = uniqueId;
    }
    public void Load(CookingHandlerSaveData data)
    {
        if(data.uniqueId != uniqueId) return;
        foreach(KitchenManager.RecipeState recipeState in kitchenManager.allRecipes){
            if(recipeState.recipe_datas.recipe == data.selectedRecipe){
                if(recipeState.isUnlocked){
                    SelectedRecipe(data.selectedRecipe);
                }
            }
        }

        if (data.autoActivated)
        {
            if (recipeState == null || recipeState.recipe_datas == null) return;
            autoActivated = true;
            isCooking = true;
            StartEnergyConsumption();
            generatorAdvanced.StartGeneratingAuto(recipeState.recipe_datas.defaultCookingTime); //change this out when upgradeable time comes
        }else{
            generatorAdvanced.StopGenerating();
            energyConsumptionGenerator.StopGenerating();
            autoActivated = false;
            isCooking = false;
        }

    }
}

[System.Serializable]
public struct CookingHandlerSaveData
{
    public Recipes selectedRecipe;
    public bool manualActivated;
    public bool autoActivated;
    public string uniqueId;
}