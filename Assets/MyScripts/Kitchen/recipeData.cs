using UnityEngine;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;

[CreateAssetMenu(menuName = "KitchenManager/Recipe")]
public class recipeData : ScriptableObject
{
    public string recipeName;
    public Recipes recipe;
    public List <CurrencyTypes> requiredResources;
    public float defaultTimeToResearch;
    public float defaultChance;
    public Sprite image;
}
