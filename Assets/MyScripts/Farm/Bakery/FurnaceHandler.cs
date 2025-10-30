using System.Collections;
using UnityEngine;
using UnityEngine.Animations;

public class FurnaceHandler : MonoBehaviour
{

[SerializeField] private BakeryManager bakeryManager;

    [SerializeField] private ProgressBarHandler progressBarHandler;
    private Coroutine cookingCoroutine;
    private bool breadDone = false;
    public BigNumber breadDoneAmount;
    public BigNumber GetBreadDone() => breadDoneAmount;


    public void StartFurnace(){
        if(cookingCoroutine == null && bakeryManager.doughInsideFurCounter > 0){
            cookingCoroutine = StartCoroutine(FurnaceCooking());
        }
    }
    public void CollectFromFurnace(){
        if(breadDone){
            bakeryManager.SetBreadDoneAmount(0);
            MoneyManager.Instance.AddCurrency(CurrencyTypes.bread,breadDoneAmount);
            bakeryManager.HarvestBreadAnim();
            breadDone = false;
            bakeryManager.UpdateUI();
            progressBarHandler.ResetProgress();
        }
    }


    private IEnumerator FurnaceCooking(){
        print("coroutine started");
        progressBarHandler.StartProgress(UpgradeManager.Instance.GetTimePower(TimeUpgradeTypes.furnaceBakingTime));
        bakeryManager.StartCookingAnim();
        breadDone = false;
        // cookingAnimator.SetBool("ChimneyOn",true);
        yield return new WaitForSeconds(UpgradeManager.Instance.GetTimePower(TimeUpgradeTypes.furnaceBakingTime));
        print("stopcooking");
        breadDone = true;
            bakeryManager.StopCookingAnim();
            // CollectFromFurnace();
            breadDoneAmount = bakeryManager.doughInsideFurCounter; 
            bakeryManager.SetBreadDoneAmount(breadDoneAmount);
            bakeryManager.doughInsideFurCounter = 0;
            bakeryManager.UpdateUI();
            StopCoroutine(cookingCoroutine);
            cookingCoroutine= null;
    }   
}
