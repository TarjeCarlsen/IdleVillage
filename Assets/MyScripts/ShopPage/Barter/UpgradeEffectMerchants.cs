using UnityEngine;
using LargeNumbers;
using System;
using UnityEngine.Assertions.Must;
using System.Collections.Generic;
public enum MerchantUpgradeTypes
{
    addRewardFlat,
    addRewardMulti,
    addRewardFlatInt,

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
    public List<Merchants> merchants;
    public List<CurrencyTypes> currencyTypes;
    public UpgradeID upgradeID;
    public UpgradeOperation upgradeOperation;



    [Header("Define the amount of the upgrade")]
    public AlphabeticNotation flat;
    public int flat_forIntUpgrades;
    public float flat_forFloatUpgrades;
    public bool stateForUpgrade = false;
    public void Apply(GameObject target = null)
    {


        Debug.Log("bought upgrade" + merchantUpgradeTypes);
        switch (merchantUpgradeTypes)
        {
            // -------- MERCHANT UPGRADES-------- //
            case MerchantUpgradeTypes.addRewardFlat:
                foreach (Merchants merch in merchants)
                {
                    foreach (CurrencyTypes type in currencyTypes)
                    {
                        MerchantUpgradeManager.Instance.Modify(upgradeID, UpgradeOperation.Add, merch, type, flat);
                    }
                }
                //  MerchantUpgradeManager.Instance.AnyAddRewardFlat(merchant,currencyType,flat);
                break;
            case MerchantUpgradeTypes.addRewardFlatInt:
                foreach (Merchants merch in merchants)
                {
                    foreach (CurrencyTypes type in currencyTypes)
                    {
                        MerchantUpgradeManager.Instance.Modify(upgradeID, UpgradeOperation.Add, merch, type, flat_forIntUpgrades);
                    }
                }
                break;
            case MerchantUpgradeTypes.addRewardMulti:
                foreach (Merchants merch in merchants)
                {
                    foreach (CurrencyTypes type in currencyTypes)
                    {
                        MerchantUpgradeManager.Instance.Modify(upgradeID, UpgradeOperation.Add, merch, type, flat_forFloatUpgrades);
                    }
                }
                // MerchantUpgradeManager.Instance.AnyAddRewardMulti(merchant,currencyType,flat_forFloatUpgrades);
                break;
                // case MerchantUpgradeTypes.expMulti:
                //     MerchantUpgradeManager.Instance.Modify(UpgradeIDmerchant,currencyType,flat_forFloatUpgrades);
                //     break;


        }
    }

}
