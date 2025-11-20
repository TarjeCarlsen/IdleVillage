using UnityEngine;
using LargeNumbers;
using System;
using UnityEngine.Assertions.Must;
public enum MerchantUpgradeTypes{
    addFlatValue,
    addMultiValue,
    addFlatValueInt,

}

public interface IUpgradeEffectMerchants
{
    void Apply(GameObject target = null);
}

[CreateAssetMenu(fileName = "UpgradeEffectMerchants", menuName = "Upgrade/Upgrade effect merchants")]
public class UpgradeEffectMerchants : ScriptableObject
{
    //NEW SYSTEM
    [Header("NEW SYSTEM USE THIS! Chose the merchant on the upgrade that is to be upgraded!")]
    public MerchantUpgradeTypes merchantUpgradeTypes;
    public Merchants merchant;
    public CurrencyTypes currencyType;



    [Header("Define the amount of the upgrade")]
    public AlphabeticNotation flat;
    public int flat_forIntUpgrades;
    public float flat_forFloatUpgrades;
    public bool stateForUpgrade = false;
    public void Apply(GameObject target = null){


        switch(merchantUpgradeTypes){
            // -------- MERCHANT UPGRADES-------- //
            case MerchantUpgradeTypes.addFlatValue:
                MerchantUpgradeManager.Instance.AnyAddRewardFlat(merchant,currencyType,flat);
                break;
            case MerchantUpgradeTypes.addFlatValueInt:
                MerchantUpgradeManager.Instance.AnyAddRewardFlatInt(merchant,currencyType,flat_forIntUpgrades);
                break;
            case MerchantUpgradeTypes.addMultiValue:
                MerchantUpgradeManager.Instance.AnyAddRewardMulti(merchant,currencyType,flat_forFloatUpgrades);
                break;

        }
    }
    
}
