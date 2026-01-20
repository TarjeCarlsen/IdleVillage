using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Collections;

public class newRecipeHandler : MonoBehaviour
{
    private List<CurrencyTypes> selectedResources;
    [SerializeField] private KitchenManager kitchenManager;
    [SerializeField] private TMP_Text time_txt;
    private float timeRemaining;

private void Awake(){
    selectedResources = new List<CurrencyTypes>();
}

    public void SelectedResource(CurrencyTypes type){
        selectedResources.Add(type);
        kitchenManager.ChoseRecipe(selectedResources);
    }

    public void UnSelectResource(CurrencyTypes type){
        selectedResources.Remove(type);
        kitchenManager.ChoseRecipe(selectedResources);

    }

    private void StartResearch(){

    }

    private void StopResearch(){

    }
    private IEnumerator researchCoroutine(){

        while (true)
        {
            time_txt.text = HelperFunctions.Instance.ConvertSecondsToTime(timeRemaining);
            yield return new WaitForSeconds(1f);
            timeRemaining -= 1f;
            if (timeRemaining <= 0f)
            {
                timeRemaining = 0f;
                print("timer done!");
            }
        }
    }



}
