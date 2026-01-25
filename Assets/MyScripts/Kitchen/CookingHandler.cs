using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Assertions.Must;

    public enum CookingMode{
        idle,
        manual,
        auto,
        transitioning,
    }
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
    [SerializeField] private CookingMode cookingMode = CookingMode.idle;


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
        generatorAdvanced.OnTransitionedToAuto += TransitioningFinished;
    }
    private void OnDisable()
    {
        kitchenManager.OnnewRecipeUnlocked -= UpdateAvailableRecipes;
        generatorAdvanced.OnManualFinish -= ManualFinished;
        energyConsumptionHandler.EnergyExausted -= StopGeneratingAuto;
        energyConsumptionHandler.EnergyReStarted -= ReStartAuto;
        generatorAdvanced.OnTransitionedToAuto -= TransitioningFinished;

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
        if (cookingMode != CookingMode.idle) return;
        {
            cookingMode = CookingMode.manual;
            generatorAdvanced.StartGenerating(recipeState.recipe_datas.defaultCookingTime); //change this out when upgradeable time comes
            // manualActivated = true;
        }
    }

private void ManualFinished()
{
    if (cookingMode != CookingMode.manual){
        Debug.Log("Manual finished ignored! (state = "+ cookingMode + ")");
        return;
    }

        cookingMode = CookingMode.idle;
        print("MANUALFINISHED EVENT INVOKED - COOKINGMODE = Idle");
    
}

private void TransitioningFinished(){
    if(cookingMode == CookingMode.transitioning){
        print("TRANSITION EVENT INVOKED - COOKINGMODE = AUTO");
        cookingMode = CookingMode.auto;
    }
}

public void ToggleAuto(){
    if(cookingMode == CookingMode.manual||cookingMode == CookingMode.transitioning){
        if(cookingMode == CookingMode.transitioning){
            print("inside cancell transition - "+cookingMode);
            generatorAdvanced.CancellTransitionAuto();
            energyConsumptionHandler.OnStopEnergyRoutine();
            cookingMode = CookingMode.manual;
            return;
        }else{
            print("inside transition - "+cookingMode);
            StartAuto(true);
            return;
        }
    }
    if(cookingMode == CookingMode.auto){
        print("inside stop auto - "+cookingMode);
        StopAuto();
            return;
    }else if(cookingMode == CookingMode.idle){
        print("inside start regular auto - "+cookingMode);
        StartAuto(false);
            return;
    }
    print("Outside statements - "+cookingMode);
            return;
}
public void StartAuto(bool fromManual)
{
    energyConsumptionHandler.OnStartEnergyRoutine(eneryConsumptionTimeIntervall);

    if (fromManual){

        generatorAdvanced.TransitionToAuto();
        cookingMode = CookingMode.transitioning;
        generatorAdvanced.stopRequested = false;
    }
    else{
        cookingMode = CookingMode.auto;
        generatorAdvanced.stopRequested = false;
        generatorAdvanced.StartGeneratingAuto(recipeState.recipe_datas.defaultCookingTime);
    }
}

private void StopAuto()
{
    cookingMode = CookingMode.idle;
    generatorAdvanced.stopRequested = true;
    energyConsumptionHandler.OnStopEnergyRoutine();
}



public void ReStartAuto()
{
    if (cookingMode != CookingMode.auto) return;

    energyConsumptionHandler.OnStartEnergyRoutine(eneryConsumptionTimeIntervall);
    generatorAdvanced.StartGeneratingAuto(recipeState.recipe_datas.defaultCookingTime);
}

private void StopGeneratingAuto()
{
    if (cookingMode == CookingMode.auto)
        StopAuto();
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
void LateUpdate()
{
    if (cookingMode == CookingMode.auto && energyConsumptionHandler.IsEnergyRoutineRunning==false)
    {
        Debug.LogError("INVARIANT BROKEN: auto without energy");
        energyConsumptionHandler.OnStartEnergyRoutine(eneryConsumptionTimeIntervall);
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
    data.mode = cookingMode;
    data.selectedRecipe = currentSelectedRecipe;
    data.timeRemaining = generatorAdvanced.GetTimeRemaining();
    }
    public void Load(CookingHandlerSaveData data)
    {
    if (data.uniqueId != uniqueId) return;

    StopAuto();
    generatorAdvanced.StopGenerating();
    cookingMode = CookingMode.idle;

    SelectedRecipe(data.selectedRecipe);

    if (data.mode == CookingMode.auto)
        StartAuto(false);
    else if (data.mode == CookingMode.manual)
        generatorAdvanced.StartGenerating(recipeState.recipe_datas.defaultCookingTime);
}
}
[System.Serializable]
public struct CookingHandlerSaveData
{
    public CookingMode mode;
    public Recipes selectedRecipe;
    public bool manualActivated;
    public bool autoActivated;
    public string uniqueId;
    public float timeRemaining;
}