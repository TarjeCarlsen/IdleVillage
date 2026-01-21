using UnityEngine;

public class RecipeSelection : MonoBehaviour
{
    [SerializeField] public Recipes selectedRecipe;
    [SerializeField] private CookingHandler cookingHandler;


    public void OnRecipeSelectionClick(){
        if(cookingHandler.isCooking) return;
            cookingHandler.SelectedRecipe(selectedRecipe);
    }
}
