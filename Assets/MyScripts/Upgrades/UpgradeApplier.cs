using UnityEngine;

public class UpgradeApplier : MonoBehaviour
{

    [SerializeField] GameObject areaToUnlock;
    [SerializeField] CardInfo cardInfo;
    public UpgradeEffect upgradeEffect;
    public UpgradeEffectMerchants upgradeEffectMerchants;
    

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
                upgradeEffect.Apply();
        }
        if(upgradeEffectMerchants != null){
                upgradeEffectMerchants.Apply();
        }
    }
}
