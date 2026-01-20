using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class newRecipeHandler : MonoBehaviour
{
    private List<CurrencyTypes> selectedResources;
    [SerializeField] private KitchenManager kitchenManager;
    [SerializeField] private TMP_Text time_txt;
    [SerializeField] private TMP_Text percentage_txt;
    [SerializeField] private Image recipeImage;
    [SerializeField] private Sprite notUnlockedImage;
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color notUnlockedColor;
    [SerializeField] private GameObject newRecipeDiscoveredText_img;

    private Coroutine researchCoroutine;
    private float timeRemaining;
    private KitchenManager.RecipeState recipeData;
    public bool isResearching;




    private void Awake()
    {
        selectedResources = new List<CurrencyTypes>();
        recipeImage.sprite = notUnlockedImage;
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

    public void StartResearch()
    {
        print("called startresearch!");
        if (researchCoroutine == null)
        {
            isResearching = true;
            timeRemaining = recipeData.recipe_datas.defaultTimeToResearch;
            researchCoroutine = StartCoroutine(Research());
        }
    }

    public void StopResearch()
    {
        if (researchCoroutine != null)
        {
            StopCoroutine(researchCoroutine);
            researchCoroutine = null;
        }
    }
    private IEnumerator Research()
    {

        while (true)
        {
            time_txt.text = HelperFunctions.Instance.ConvertSecondsToTime(timeRemaining);
            yield return new WaitForSeconds(1f);
            timeRemaining -= 1f;
            if (timeRemaining <= 0f)
            {
                print("setting researching false");
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

        roll = Random.Range(0, 1f);

        if (roll <= chance)
        {
            print("recipe unlocked!");
            print($"roll = {roll} chance was {chance}");
            recipeData.isUnlocked = true;
            UpdateUI();
            newRecipeDiscoveredText_img.SetActive(true);
            return true;
        }
        else
        {
            print("not unlocked!");
            print($"roll = {roll} chance was {chance}");
            UpdateUI();
            return false;
        }
        
    }

    private void UpdateUI()
    {
            newRecipeDiscoveredText_img.SetActive(false);
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
            }
            else
            {
                recipeImage.color = notUnlockedColor;
                recipeImage.sprite = notUnlockedImage;
            }
        }
        else
        {
            time_txt.text = "00:00:00";
            percentage_txt.text = "0";

        }
    }


}
