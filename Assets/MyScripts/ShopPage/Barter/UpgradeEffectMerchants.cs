using UnityEngine;
using LargeNumbers;
using System;
using UnityEngine.Assertions.Must;
using System.Collections.Generic;
public enum MerchantUpgradeTypes
{
    addRewardAlpha,
    addRewardFloat,
    addRewardInt,
    subRewardAlpha,
    subRewardFloat,
    subRewardInt,
    setRewardAlpha,
    setRewardFloat,
    setRewardInt,
    setRewardBool,

}

public interface IUpgradeEffectMerchants
{
    void Apply(GameObject target = null);
}

[CreateAssetMenu(fileName = "UpgradeEffectMerchants", menuName = "Upgrade/Upgrade effect merchants")]
public class UpgradeEffectMerchants : ScriptableObject
{

    public List<upgradeTypeInfo> merchantUpgradeTypeInfo;


    [System.Serializable]
    public class upgradeTypeInfo{
        public List<MerchantUpgradeTypes> merchantUpgradeTypes;
        public List<Merchants> merchants;
        public List<CurrencyTypes> currencyTypes;
        public UpgradeID upgradeID;
        public AlphabeticNotation flat_alpha;
        public int flat_forIntUpgrades;
        public float flat_forFloatUpgrades;
        public bool stateForUpgrade = false;        

    }
    public void Apply(GameObject target = null)

    {


        // Debug.Log("bought upgrade" + merchantUpgradeTypeInfo);
        foreach (var info in merchantUpgradeTypeInfo)
        {
            foreach(MerchantUpgradeTypes upgradeType in info.merchantUpgradeTypes){

            switch (upgradeType)
            {
                // -------- MERCHANT UPGRADES-------- //
                case MerchantUpgradeTypes.addRewardAlpha:
                    foreach (Merchants merch in info.merchants)
                    {
                        foreach (CurrencyTypes type in info.currencyTypes)
                        {
                            MerchantUpgradeManager.Instance.Modify(info.upgradeID, UpgradeOperation.Add, merch, type, info.flat_alpha);
                        }
                    }
                    break;
                case MerchantUpgradeTypes.subRewardAlpha:
                    foreach (Merchants merch in info.merchants)
                    {
                        foreach (CurrencyTypes type in info.currencyTypes)
                        {
                            MerchantUpgradeManager.Instance.Modify(info.upgradeID, UpgradeOperation.Subtract, merch, type, info.flat_alpha);
                        }
                    }
                    break;
                case MerchantUpgradeTypes.addRewardFloat:
                    foreach (Merchants merch in info.merchants)
                    {
                        foreach (CurrencyTypes type in info.currencyTypes)
                        {
                            MerchantUpgradeManager.Instance.Modify(info.upgradeID, UpgradeOperation.Add, merch, type, info.flat_forFloatUpgrades);
                        }
                    }
                    break;
                case MerchantUpgradeTypes.subRewardFloat:
                    foreach (Merchants merch in info.merchants)
                    {
                        foreach (CurrencyTypes type in info.currencyTypes)
                        {
                            MerchantUpgradeManager.Instance.Modify(info.upgradeID, UpgradeOperation.Subtract, merch, type, info.flat_forFloatUpgrades);
                        }
                    }
                    break;
                case MerchantUpgradeTypes.addRewardInt:
                    foreach (Merchants merch in info.merchants)
                    {
                        foreach (CurrencyTypes type in info.currencyTypes)
                        {
                            MerchantUpgradeManager.Instance.Modify(info.upgradeID, UpgradeOperation.Add, merch, type, info.flat_forIntUpgrades);
                        }
                    }
                    break;
                case MerchantUpgradeTypes.subRewardInt:
                    foreach (Merchants merch in info.merchants)
                    {
                        foreach (CurrencyTypes type in info.currencyTypes)
                        {
                            MerchantUpgradeManager.Instance.Modify(info.upgradeID, UpgradeOperation.Subtract, merch, type, info.flat_forIntUpgrades);
                        }
                    }
                    break;
                case MerchantUpgradeTypes.setRewardAlpha:
                    foreach (Merchants merch in info.merchants)
                    {
                        foreach (CurrencyTypes type in info.currencyTypes)
                        {
                            MerchantUpgradeManager.Instance.Modify(info.upgradeID, UpgradeOperation.Set, merch, type, info.flat_alpha);
                        }
                    }
                    break;
                case MerchantUpgradeTypes.setRewardFloat:
                    foreach (Merchants merch in info.merchants)
                    {
                        foreach (CurrencyTypes type in info.currencyTypes)
                        {
                            MerchantUpgradeManager.Instance.Modify(info.upgradeID, UpgradeOperation.Set, merch, type, info.flat_forFloatUpgrades);
                        }
                    }
                    break;
                case MerchantUpgradeTypes.setRewardInt:
                    foreach (Merchants merch in info.merchants)
                    {
                        foreach (CurrencyTypes type in info.currencyTypes)
                        {
                            MerchantUpgradeManager.Instance.Modify(info.upgradeID, UpgradeOperation.Set, merch, type, info.flat_forIntUpgrades);
                        }
                    }
                    break;
                case MerchantUpgradeTypes.setRewardBool:
                    foreach (Merchants merch in info.merchants)
                    {
                        foreach (CurrencyTypes type in info.currencyTypes)
                        {
                            MerchantUpgradeManager.Instance.Modify(info.upgradeID, UpgradeOperation.Set, merch, type, info.stateForUpgrade);
                        }
                    }
                    break;


            }
            }

        }
    }

}
