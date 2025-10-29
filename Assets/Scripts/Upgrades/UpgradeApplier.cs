using UnityEngine;

public class UpgradeApplier : MonoBehaviour
{

    [SerializeField] GameObject areaToUnlock;
    [SerializeField] CardInfo cardInfo;
    public UpgradeEffect upgradeEffect;
    

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
            if (upgradeEffect.upgradeTypes == UpgradeTypes.UnlockArea)
            {
                upgradeEffect.Apply(areaToUnlock);
            }
            else
            {
                upgradeEffect.Apply();
            }
        }
    }
}
