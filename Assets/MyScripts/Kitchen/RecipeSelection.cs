using UnityEngine;

public class RecipeSelection : MonoBehaviour
{
    [SerializeField] private Recipes recipes;
    [SerializeField] private CookingHandler cookingHandler;


    public void OnRecipeSelectionClick(){
        if(cookingHandler.isCooking) return;
            cookingHandler.SelectedRecipe(recipes);
    }
}
