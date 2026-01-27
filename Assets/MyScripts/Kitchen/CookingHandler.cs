using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Assertions.Must;


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
    private Recipes currentSelectedRecipe;
    [SerializeField] public string uniqueId;
    [SerializeField] private GenerationMode cookingMode = GenerationMode.idle;
    private bool isExhausted;

    private void Awake()
    {
        kitchenManager = GameObject.FindGameObjectWithTag("KitchenPage").GetComponent<KitchenManager>();
        uniqueId = gameObject.name;
    }

    private void OnEnable()
    {
        kitchenManager.OnnewRecipeUnlocked += UpdateAvailableRecipes;
        energyConsumptionHandler.EnergyExausted += StopAuto;
        energyConsumptionHandler.EnergyReStarted += ReStartAuto;
        generatorAdvanced.OnManualFinish += ManualFinished;
        generatorAdvanced.OnTransitionedToAuto += TransitioningFinished;
    }
    private void OnDisable()
    {
        kitchenManager.OnnewRecipeUnlocked -= UpdateAvailableRecipes;
        generatorAdvanced.OnManualFinish -= ManualFinished;
        energyConsumptionHandler.EnergyExausted -= StopAuto;
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
        if (cookingMode != GenerationMode.idle) return;
        {
            cookingMode = GenerationMode.manual;
            generatorAdvanced.StartGenerating(recipeState.recipe_datas.defaultCookingTime); //change this out when upgradeable time comes
            // manualActivated = true;
        }
    }

private void ManualFinished()
{
    if (cookingMode != GenerationMode.manual){
        Debug.Log("Manual finished ignored! (state = "+ cookingMode + ")");
        return;
    }

        cookingMode = GenerationMode.idle;
    
}

private void TransitioningFinished(){
    if(cookingMode == GenerationMode.transitioning){
        cookingMode = GenerationMode.auto;
    }else{
        //FOR DEBUGGING
    }
}




/// <summary>
/// State 1: manual activated
/// State 2: Auto activated
/// State 3: Transition from manual to auto
/// 
/// Fix 1: Cancelling from manual transition
/// </summary>
public void ToggleAuto(){

    switch(cookingMode){
        case GenerationMode.transitioning:
            generatorAdvanced.CancellTransitionAuto();
            energyConsumptionHandler.OnStopEnergyRoutine();
            cookingMode = GenerationMode.manual;
        break;
        case GenerationMode.manual:
            StartAuto(true);
        break;

        case GenerationMode.auto:
        StopAuto(false);
        break;

        case GenerationMode.idle:
        StartAuto(false);
        break;
    }
}
public void StartAuto(bool fromManual)
{
    if(!energyConsumptionHandler.CanAfford()) return;
    energyConsumptionHandler.OnStartEnergyRoutine(eneryConsumptionTimeIntervall);

    if (fromManual){

        generatorAdvanced.TransitionToAuto();
        cookingMode = GenerationMode.transitioning;
        generatorAdvanced.stopRequested = false;
    }
    else{
        cookingMode = GenerationMode.auto;
        generatorAdvanced.stopRequested = false;
        generatorAdvanced.StartGeneratingAuto(recipeState.recipe_datas.defaultCookingTime);
    }
}

private void StopAuto(bool isEnergyExhausted)
{
    if(isEnergyExhausted){
        generatorAdvanced.stopRequested = true;
        cookingMode = GenerationMode.auto;
        isExhausted = true;
    }else{
        isExhausted = false;
        generatorAdvanced.stopRequested = true;
        cookingMode = GenerationMode.idle;
        energyConsumptionHandler.OnStopEnergyRoutine();

    }
}



public void ReStartAuto()
{
    if (cookingMode == GenerationMode.auto ||cookingMode == GenerationMode.idle ){

    energyConsumptionHandler.OnStartEnergyRoutine(eneryConsumptionTimeIntervall);
    generatorAdvanced.StartGeneratingAuto(recipeState.recipe_datas.defaultCookingTime);
    }
}

private void StopGeneratingAuto()
{
    if (cookingMode == GenerationMode.auto)
        StopAuto(false);
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
    if (cookingMode == GenerationMode.auto && energyConsumptionHandler.IsEnergyRoutineRunning==false)
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
        data.uniqueId = uniqueId;
    data.mode = cookingMode;
    data.selectedRecipe = currentSelectedRecipe;
    data.timeRemaining = generatorAdvanced.GetTimeRemaining();
    data.transitionRequested = generatorAdvanced.GetTransitionRequrested();
    data.resumeGeneration = generatorAdvanced.GetResumeGeneration();
    data.isExhausted = isExhausted;
    }

    
    public void Load(CookingHandlerSaveData data)
    {
    if (data.uniqueId != uniqueId)
    {
     return;
    }
    cookingMode = data.mode;
    
    generatorAdvanced.StopGenerating();

    SelectedRecipe(data.selectedRecipe);
    if(data.isExhausted){
        energyConsumptionHandler.OnResumeExhaustedStage(eneryConsumptionTimeIntervall);
        generatorAdvanced.StartGenerating(recipeState.recipe_datas.defaultCookingTime);
    }else{

    switch(data.mode){
        case GenerationMode.idle:
        generatorAdvanced.StopGenerating();
        energyConsumptionHandler.OnStopEnergyRoutine();
        break;
        case GenerationMode.auto:
        generatorAdvanced.ResumeGeneration(recipeState.recipe_datas.defaultCookingTime,data.timeRemaining,true);
        energyConsumptionHandler.OnStartEnergyRoutine(eneryConsumptionTimeIntervall);
        break;
        case GenerationMode.transitioning:
        generatorAdvanced.StartGenerating(recipeState.recipe_datas.defaultCookingTime);
        energyConsumptionHandler.OnStartEnergyRoutine(eneryConsumptionTimeIntervall);
        break;
        case GenerationMode.manual:
        energyConsumptionHandler.OnStopEnergyRoutine();
        generatorAdvanced.StartGenerating(recipeState.recipe_datas.defaultCookingTime);
        break;
    }
    }
}
}
[System.Serializable]
public struct CookingHandlerSaveData
{
    public GenerationMode mode;
    public Recipes selectedRecipe;
    public string uniqueId;
    public float timeRemaining;
    public bool transitionRequested;
    public bool resumeGeneration;
    public bool isExhausted;
}