using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class UpgradeApplier : MonoBehaviour
{
    [SerializeField] CardInfo cardInfo;
    public UpgradeEffect upgradeEffect;
    public UpgradeEffectMerchants upgradeEffectMerchants;
    [SerializeField] GameObject areaToUnlock;
    [SerializeField] private bool severlAreasToUnlock = false; 
    [SerializeField] private List<GameObject> listOfAreasToUnlock; 
    

    private void OnEnable(){
        if(cardInfo != null){
            cardInfo.OnBought += ApplyUpgrade;
        }
    }
    private void OnDisable(){
        if(cardInfo != null){
            cardInfo.OnBought -= ApplyUpgrade;
        }
    }

    public void ApplyUpgrade(){
        if(upgradeEffect != null){
                if(severlAreasToUnlock){
                    foreach(GameObject area in listOfAreasToUnlock){
                        upgradeEffect.Apply(area);
                    }
                }else{
                upgradeEffect.Apply(areaToUnlock);
                }
            
            // else
            // {
            //     upgradeEffect.Apply();
            // }
        }
        if(upgradeEffectMerchants != null){
                upgradeEffectMerchants.Apply();
        }
    }
}
