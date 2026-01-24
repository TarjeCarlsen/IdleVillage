using UnityEngine;
using System.Collections.Generic;
using LargeNumbers;

[System.Serializable]
public class RecipeInfo{

    public List<GenerateInfo> payInfo;
    public List<GenerateInfo> generateInfo;
    public List<string> payDescription;
    public List<string> generateDescription;
    }


[CreateAssetMenu(menuName = "KitchenManager/Recipe")]
public class recipeData : ScriptableObject
{
    public string recipeName;
    public Recipes recipe;
    public List <CurrencyTypes> requiredResources;
    public float defaultTimeToResearch = 10f;
    public float defaultCookingTime = 10f;
    public float defaultChance = 0.05f;
    public Sprite image;
    public string descriptionText;
    public RecipeInfo recipeYield;



}
