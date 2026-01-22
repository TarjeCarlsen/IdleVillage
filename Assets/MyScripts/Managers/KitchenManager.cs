using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;



public enum Recipes // list all recipes in the enum to use when cooking
{
    grain, // wheat 
    chunkyFlour, // wheat + flour 
    wheatSprinkelledDough, // wheat + dough
    CornBread, // cornflour + water
    flour, // grain
    dough, // water + flour
    tomatoSoup, // water + tomato
    bread, // dough + flour 
    cornFlour, // corn + flour
    tomatoCornSandwich, // bread + tomato + corn
    pumpkinPie, // pumpkin + dough 
    cheeselessPizza, // dough + flour + tomato 
    vegetableSoup, // water + carrot + corn + tomato
    CountryBreadPot, //bread + water + corn + carrot + pumpkin -- // finished to here - 21.01.2026
    pumpkinTortellini, //dough + pumpkin
    flourSmeredCorn, // corn + flour
    tomatoPure, // tomato + flour
    poorMansSoup, // water + grain
    tortella, //cornflour + water
    veggieTaco, // tortella + corn + carrot + tomato




}
public class KitchenManager : MonoBehaviour
{
    public List<RecipeState> allRecipes;
    public event Action<Recipes, bool> OnnewRecipeUnlocked;
    [System.Serializable]
    public class RecipeState
    {
        public recipeData recipe_datas;
        public bool isUnlocked = false;
    }

    public RecipeState ChoseRecipe(List<CurrencyTypes> resourceList)
    {
        foreach (RecipeState recipe in allRecipes)
        {
            if (ListsMatchSorted(resourceList, recipe.recipe_datas.requiredResources))
            {
                print("found recipe! : " + recipe.recipe_datas.recipeName);
                return recipe;
            }
        }
        // print("no match found");
        return null; // no match found
    }

    public RecipeState GetRecipe(Recipes _recipes)
    {
        foreach (RecipeState recipe in allRecipes)
        {
            if (_recipes == recipe.recipe_datas.recipe)
            {
                print("found recipe! : " + recipe.recipe_datas.recipeName);
                return recipe;
            }
        }
        return null;
    }

    public void ForwardEventRaisedRecipeUnlocked(Recipes _recipe)
    {
        OnnewRecipeUnlocked?.Invoke(_recipe, true);
    }

    private bool ListsMatchSorted(
        List<CurrencyTypes> a,
        List<CurrencyTypes> b
    )
    {
        if (a.Count != b.Count)
            return false;

        var aSorted = new List<CurrencyTypes>(a);
        var bSorted = new List<CurrencyTypes>(b);

        aSorted.Sort();
        bSorted.Sort();

        for (int i = 0; i < aSorted.Count; i++)
        {
            if (aSorted[i] != bSorted[i])
                return false;
        }

        return true;
    }



    public void Save(ref KitchenManagerSaveData data)
    {
        data.recipeSaveDatas = new List<RecipeSaveData>();

        foreach (RecipeState recipe in allRecipes)
        {
                data.recipeSaveDatas.Add(new RecipeSaveData
                {
                    recipeName = recipe.recipe_datas.recipeName,
                    isUnlocked = recipe.isUnlocked,
                    recipe = recipe.recipe_datas.recipe,
                });
            
        }
    }
    public void Load(KitchenManagerSaveData data)
    {
        foreach (RecipeState recipe in allRecipes){
            foreach(RecipeSaveData recipeSave in data.recipeSaveDatas){
                if(recipeSave.recipeName == recipe.recipe_datas.recipeName){
                    recipe.isUnlocked = recipeSave.isUnlocked;
                    if(recipe.isUnlocked){
                        OnnewRecipeUnlocked?.Invoke(recipe.recipe_datas.recipe,true);
                    }else{
                        OnnewRecipeUnlocked?.Invoke(recipe.recipe_datas.recipe,false);
                    }
                }
            }
        }
        
    }
}

[System.Serializable]
public struct KitchenManagerSaveData
{
    public List<RecipeSaveData> recipeSaveDatas;
}

[System.Serializable]
public struct RecipeSaveData
{
    public string recipeName;
    public Recipes recipe;
    public bool isUnlocked;

}
