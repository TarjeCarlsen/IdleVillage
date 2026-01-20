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
    [SerializeField] private List<GameObject> payDescriptionObjects;
    [SerializeField] private List<GameObject> generateDescriptionObjects;
    private KitchenManager kitchenManager;
    private KitchenManager.RecipeState recipeState;
    public bool isCooking = false;
    [SerializeField] private GeneratorAdvanced generatorAdvanced;
    private GenAdvancedInfo generatorInfo;

private void Start(){
    kitchenManager = GameObject.FindGameObjectWithTag("KitchenPage").GetComponent<KitchenManager>();
    recipeState.recipe_datas = null;
}
public void SelectedRecipe(Recipes recipes){
    recipeState = kitchenManager.GetRecipe(recipes);
    generatorAdvanced.ClearGenAdvancedInfos();
    generatorAdvanced.EditGenAdvancedInfos(recipeState.recipe_datas.recipeYield.generateInfo,recipeState.recipe_datas.recipeYield.payInfo);
    icon_img.sprite = recipeState.recipe_datas.image;
    UpdateDescription();
}



public void OnStartCookingClick(){
    if(recipeState.recipe_datas == null) return;
        generatorAdvanced.StartGenerating(recipeState.recipe_datas.defaultCookingTime); //change this out when upgradeable time comes
}

public void OnStartCookingAutoClick(){
    if(recipeState.recipe_datas == null) return;
        generatorAdvanced.StartGeneratingAuto(recipeState.recipe_datas.defaultCookingTime); //change this out when upgradeable time comes

}
private void OnStopCookingClick(){

}



private void UpdateDescription()
{
    if(recipeState.recipe_datas == null){
        foreach(TMP_Text payDescription in prices){
            payDescription.gameObject.SetActive(false);
        }
        foreach(TMP_Text genDescription in yields){
            genDescription.gameObject.SetActive(false);
        }

    }else{
    header_txt.text = recipeState.recipe_datas.recipeName;
    int lengthOfPayments = recipeState.recipe_datas.recipeYield.payInfo.Count;
    int lengtOfYield = recipeState.recipe_datas.recipeYield.generateInfo.Count;
    Debug.Log("Payments: " + lengthOfPayments);

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
}
