using System.Data.Common;
using UnityEngine;

public class resourceSelection : MonoBehaviour
{
    [SerializeField] private CurrencyTypes currencyType;
    [SerializeField] private newRecipeHandler newRecipeHandler;
    [SerializeField] private SetGameobjectActive setGameobjectActive;
    private bool isActive = false;

    public void OnSelectClick()
    {
        if(newRecipeHandler.isResearching) return;
        setGameobjectActive.OnButtonToggleActive();
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

    // public void Save(ref ResourceSelectionSaveData data){
    //     data.isActive = isActive;
    //     data.currencyTypes = currencyType;
    // }
    // public void Load(ResourceSelectionSaveData data){
    //     if(data.isActive){
    //         if(data.currencyTypes == currencyType)
    //             setGameobjectActive()
    //     }
    // }
}

// [System.Serializable]
// public struct ResourceSelectionSaveData{
//     public CurrencyTypes currencyTypes;
//     public bool isActive;
// }