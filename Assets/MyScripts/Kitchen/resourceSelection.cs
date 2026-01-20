using UnityEngine;

public class resourceSelection : MonoBehaviour
{
    [SerializeField] private CurrencyTypes currencyType;
    [SerializeField] private newRecipeHandler newRecipeHandler;
    private bool isActive = false;

    public void OnSelectClick()
    {
        if (isActive)
        {
            newRecipeHandler.UnSelectResource(currencyType);
            isActive = !isActive;
        }
        else
        {
            newRecipeHandler.SelectedResource(currencyType);
            isActive = !isActive;
        }
    }
}
