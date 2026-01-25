using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;
using System.Xml;

public class newRecipeHandler : MonoBehaviour
{
    private List<CurrencyTypes> selectedResources;
     private KitchenManager kitchenManager;
    [SerializeField] private TMP_Text time_txt;
    [SerializeField] private TMP_Text percentage_txt;
    [SerializeField] private Image recipeImage;
    [SerializeField] private Sprite notUnlockedImage;
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color notUnlockedColor;
    [SerializeField] private GameObject newRecipeDiscoveredText_img;
    [SerializeField] private GameObject faileResearchText_img;
    [SerializeField] private GameObject unlockedInfo;
    [SerializeField] private TMP_Text description_text;
    [SerializeField] private TMP_Text header_txt;
    [SerializeField] public string uniqueId; // each card should be named this exactly "RecipeCard_00" then next will be "RecipeCard_01" ... and so on
                                              // easiest way to add unique id for statically added gameobjects that will not be instantiated at runtime

    private Coroutine researchCoroutine;
    private float timeRemaining;
    private KitchenManager.RecipeState recipeData;
    public bool isResearching;



    private void Awake()
    {
        kitchenManager = GameObject.FindGameObjectWithTag("KitchenPage").GetComponent<KitchenManager>();
        selectedResources = new List<CurrencyTypes>();
        recipeImage.sprite = notUnlockedImage;
        uniqueId = gameObject.name;
    }

    private void Start(){
        UpdateUI();
    }
    public void SelectedResource(CurrencyTypes type)
    {
        selectedResources.Add(type);
        recipeData = kitchenManager.ChoseRecipe(selectedResources);
        UpdateUI();
    }

    public void UnSelectResource(CurrencyTypes type)
    {
        selectedResources.Remove(type);
        recipeData = kitchenManager.ChoseRecipe(selectedResources);
        UpdateUI();

    }

    public void StartResearch(){
        StartResearchInternal(null);
    }
    public void StartResearchInternal(float? _time = null)
    {
        if(recipeData == null) return;
        if (researchCoroutine == null && !recipeData.isUnlocked)
        {
            UpdateUI();
            isResearching = true;

            timeRemaining = _time ?? recipeData.recipe_datas.defaultTimeToResearch;
            researchCoroutine = StartCoroutine(Research());
        }
    }

    public void StopResearch()
    {
        if (researchCoroutine != null)
        {
            StopCoroutine(researchCoroutine);
            researchCoroutine = null;
            UpdateUI();
        }
    }
    private IEnumerator Research()
    {
        while (true)
        {
            time_txt.text = HelperFunctions.Instance.ConvertSecondsToTime(timeRemaining);
            yield return new WaitForSeconds(1f);
            timeRemaining -= 1f;
            UpdateUI();
            if (timeRemaining <= 0f)
            {
                timeRemaining = 0f;
                StopResearch();
                isResearching = false;
                isRecipeUnlocked();
            }

        }
    }

    private bool isRecipeUnlocked()
    {
        float roll = 0f;
        float chance = recipeData.recipe_datas.defaultChance;// when upgradeable chance is implemented this has to be redone

        roll = UnityEngine.Random.Range(0, 1f);

        if (roll <= chance)
        {
            recipeData.isUnlocked = true;
            UpdateUI();
            newRecipeDiscoveredText_img.SetActive(true);
            kitchenManager.ForwardEventRaisedRecipeUnlocked(recipeData.recipe_datas.recipe);
            return true;
        }
        else
        {
            UpdateUI();
            faileResearchText_img.gameObject.SetActive(true);
            return false;
        }
        
    }

    private void UpdateUI()
    {
            newRecipeDiscoveredText_img.SetActive(false);
            faileResearchText_img.gameObject.SetActive(false);
        if (recipeData != null)
        {
            if(recipeData.recipe_datas != null) 
            {
            time_txt.text = recipeData.recipe_datas.defaultTimeToResearch.ToString();
            percentage_txt.text = (recipeData.recipe_datas.defaultChance * 100).ToString() + "%"; // when upgradeable chance is implemented this has to be redone. might also remove or 
                                                                                                  // make it appear as a upgrade
            }else{
                time_txt.text = "00:00:00";
                percentage_txt.text = "0";
            }
            if (recipeData.isUnlocked)
            {
                recipeImage.color = defaultColor;
                recipeImage.sprite = recipeData.recipe_datas.image;
                unlockedInfo.SetActive(true);
                description_text.text = recipeData.recipe_datas.descriptionText;
                header_txt.text = recipeData.recipe_datas.recipeName;
            }
            else
            {
                recipeImage.color = notUnlockedColor;
                recipeImage.sprite = notUnlockedImage;
                unlockedInfo.SetActive(false);
            }
        }
        else
        {
            time_txt.text = "00:00:00";
            percentage_txt.text = "0";
                recipeImage.color = notUnlockedColor;
                recipeImage.sprite = notUnlockedImage;
                unlockedInfo.SetActive(false);

        }
    }


public void Save(ref NewRecipeHandlerSaveData data){
    data.isResearching = isResearching;
    data.timeRemaining = timeRemaining;
    data.recipeData = recipeData;
    data.uniqueId = uniqueId;
}
public void Load(NewRecipeHandlerSaveData data){
    if(data.uniqueId != uniqueId) return;
    if(data.isResearching){
        StopResearch();
        recipeData = data.recipeData;
        StartResearchInternal(data.timeRemaining);
    }
}


}


/// <summary>
/// save data for:
/// is it researching? 
/// time remaining
/// has it finished and showing unlocked or not unlocked?
/// what recipe is beign recearched
/// what resources are selected
/// </summary>
[System.Serializable]
public struct NewRecipeHandlerSaveData{
    public string uniqueId;
public bool isResearching;
public float timeRemaining;
public KitchenManager.RecipeState recipeData;
}
