using System.Collections.Generic;
using UnityEngine;
using System;



public enum Recipes{
    grain,
    dough,
    bread,

}
public class KitchenManager : MonoBehaviour
{
    public List<recipeData> allRecipes;
    



public recipeData ChoseRecipe(List<CurrencyTypes> resourceList){
    foreach (var recipe in allRecipes)
    {
        if (ListsMatchSorted(resourceList, recipe.requiredResources))
        {
            print("found recipe! : " + recipe.recipeName);
            return recipe;
        }
    }
    // print("no match found");
    return null; // no match found
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
}
