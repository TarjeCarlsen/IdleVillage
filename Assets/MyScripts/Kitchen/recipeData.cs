using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "KitchenManager/Recipe")]
public class recipeData : ScriptableObject
{
    public string recipeName;
    public Recipes recipe;
    public List <CurrencyTypes> requiredResources;
    public bool unlocked = false;
    public int difficulty;
    public float defaultTimeToResearch;
}
