using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using LargeNumbers;
using Mono.Cecil.Cil;
using UnityEngine;


public enum UpgradeOperation
{
    Add,
    Get,
    Subtract,
    Set,
}

public enum UpgradeValueType
{
    Float,
    Int,
    Alphabetic,
    Bool,
}

[System.Serializable]
public class UpgradeValue
{
    public UpgradeValueType type;
    public float floatValue;
    public int intValue;
    public AlphabeticNotation alphabetic;
    public bool boolState;

    public object Get() =>
        type switch
        {
            UpgradeValueType.Float => floatValue,
            UpgradeValueType.Int => intValue,
            UpgradeValueType.Alphabetic => alphabetic,
            UpgradeValueType.Bool => boolState,
            _ => null
        };

    public void Add(object amount)
    {
        switch (type)
        {
            case UpgradeValueType.Float:
                floatValue += (float)amount;
                break;
            case UpgradeValueType.Int:
                intValue += (int)amount;
                break;
            case UpgradeValueType.Alphabetic:
                alphabetic += (AlphabeticNotation)amount;
                break;
        }
    }

    public void Sub(object amount)
    {
        switch (type)
        {
            case UpgradeValueType.Float:
                floatValue -= (float)amount;
                break;
            case UpgradeValueType.Int:
                intValue -= (int)amount;
                break;
            case UpgradeValueType.Alphabetic:
                alphabetic -= (AlphabeticNotation)amount;
                break;
        }
    }
    public void Set(object amount)
    {
        switch (type)
        {
            case UpgradeValueType.Float:
                floatValue = (float)amount;
                break;
            case UpgradeValueType.Int:
                intValue = (int)amount;
                break;
            case UpgradeValueType.Alphabetic:
                alphabetic = (AlphabeticNotation)amount;
                break;
            case UpgradeValueType.Bool:
                boolState = (bool)amount;
                break;
        }
    }
}
public enum UpgradeID
{
    RewardFlat,
    RewardMulti,
    RewardWeight,
    XpGainMulti,
    XpGainFlat,
    stackingMulti,
    specialBarterChance,
    specialBarterRewardMulti,
    multiRewardBasedOnFavor,
    merchantAppearWeigth,
    extraRefreshAmount,
    chanceToNotConsumeRefresh,
    refreshTimeReduction,
    flatFavorGain,
    favorGainMulti,
    priceMulti,
    bonusGiveCurrency,
    chanceToNotConsumeBarterOffer,
    bonusBasedOnPrevActivationState,
    chanceForNothing,
    oneTimeFavorModifyLose,
    oneTimeFavorModifyGain,
    // SpecialOfferChance,
    // FreeRefreshChance,
    // PriceMultiplier,
    // etc...
}
// public enum MerchantUpgradeType{

// }
// public enum UnifiedRewardCurrencyWeigths{
//     bobCurrAppearChance,
//     carlCurrAppearChance,
//     chloeCurrAppearChance,
//     fredCurrAppearChance,
//     samCurrAppearChance,
//     rogerCurrAppearChance,    
// }



public class MerchantUpgradeManager : MonoBehaviour
{
    public static MerchantUpgradeManager Instance { get; private set; }

    // ---------------------------- UNIFIED ----------------------------//

    //TESTING NEW SYSTEM:
    [SerializeField] public Dictionary<Merchants, MerchantUpgrades> merchantUpgrades;
    // [SerializeField] public Dictionary<Merchants,UpgradeOperation> upgradeOperations;
    //TESTING NEW SYSTEM:



    [System.Serializable]
    public class MerchantUpgrades
    {
        public Dictionary<(UpgradeID, CurrencyTypes), UpgradeValue> upgrades = new();
        public Dictionary<(UpgradeID, CurrencyTypes),UpgradeValue> thresholdUpgrades= new();
        public Dictionary<(UpgradeID, CurrencyTypes), UpgradeValue> thresholdMulties = new();
        // public Dictionary<CurrencyTypes, AlphabeticNotation> rewardFlat = new();
        // public Dictionary<CurrencyTypes, float> rewardMulti = new();
        // public Dictionary<CurrencyTypes, int> rewardWeigths = new();
        // public float xpGain;

        public void InitializeDefaults()
        {
            foreach (CurrencyTypes type in Enum.GetValues(typeof(CurrencyTypes)))
            {
                upgrades[(UpgradeID.RewardFlat, type)] = new UpgradeValue { type = UpgradeValueType.Alphabetic, alphabetic = new AlphabeticNotation(0) };
                upgrades[(UpgradeID.RewardMulti, type)] = new UpgradeValue { type = UpgradeValueType.Float, floatValue = 1f };
                upgrades[(UpgradeID.stackingMulti, type)] = new UpgradeValue { type = UpgradeValueType.Float, floatValue = 1f };
                upgrades[(UpgradeID.RewardWeight, type)] = new UpgradeValue { type = UpgradeValueType.Int, intValue = 1 };
                upgrades[(UpgradeID.specialBarterRewardMulti, type)] = new UpgradeValue { type = UpgradeValueType.Float, floatValue = 100 };
                upgrades[(UpgradeID.multiRewardBasedOnFavor, type)] = new UpgradeValue { type = UpgradeValueType.Float, floatValue = 0f };
                upgrades[(UpgradeID.priceMulti, type)] = new UpgradeValue { type = UpgradeValueType.Float, floatValue = 1f };
                upgrades[(UpgradeID.bonusGiveCurrency, type)] = new UpgradeValue { type = UpgradeValueType.Float,floatValue =0f };
            }
            upgrades[(UpgradeID.XpGainMulti, CurrencyDummy.Dummy)] = new UpgradeValue { type = UpgradeValueType.Float, floatValue = 1f };
            upgrades[(UpgradeID.specialBarterChance, CurrencyDummy.Dummy)] = new UpgradeValue { type = UpgradeValueType.Float, floatValue = 0f };
            upgrades[(UpgradeID.merchantAppearWeigth, CurrencyDummy.Dummy)] = new UpgradeValue { type = UpgradeValueType.Int, intValue = 10 }; //start weigth for all merchants
            upgrades[(UpgradeID.extraRefreshAmount, CurrencyDummy.Dummy)] = new UpgradeValue { type = UpgradeValueType.Int, intValue = 0 };
            upgrades[(UpgradeID.chanceToNotConsumeRefresh, CurrencyDummy.Dummy)] = new UpgradeValue { type = UpgradeValueType.Float, floatValue = 0 };
            upgrades[(UpgradeID.refreshTimeReduction, CurrencyDummy.Dummy)] = new UpgradeValue { type = UpgradeValueType.Float, floatValue = 0f };
            upgrades[(UpgradeID.flatFavorGain, CurrencyDummy.Dummy)] = new UpgradeValue { type = UpgradeValueType.Int, intValue = 0 };
            upgrades[(UpgradeID.favorGainMulti, CurrencyDummy.Dummy)] = new UpgradeValue { type = UpgradeValueType.Float, floatValue = 1f };
            upgrades[(UpgradeID.chanceToNotConsumeBarterOffer, CurrencyDummy.Dummy)] = new UpgradeValue { type = UpgradeValueType.Float, floatValue = 0f };
            upgrades[(UpgradeID.bonusBasedOnPrevActivationState, CurrencyDummy.Dummy)] = new UpgradeValue { type = UpgradeValueType.Bool, boolState = false };
            upgrades[(UpgradeID.chanceForNothing, CurrencyDummy.Dummy)] = new UpgradeValue { type = UpgradeValueType.Float, floatValue = 0f };
            // upgrades[(UpgradeID.oneTimeFavorModifyLose, CurrencyDummy.Dummy)] = new UpgradeValue { type = UpgradeValueType.Int, intValue = 0 }; //removed 03.01.26 - not working correctly
            // upgrades[(UpgradeID.oneTimeFavorModifyGain, CurrencyDummy.Dummy)] = new UpgradeValue { type = UpgradeValueType.Int, intValue = 0 }; //removed 03.01.26 - not working correctly
            // upgrades[(UpgradeID.favorThreshold_02, CurrencyDummy.Dummy)] = new UpgradeValue { type = UpgradeValueType.Int, intValue = 0 };
            // upgrades[(UpgradeID.favorThreshold_03, CurrencyDummy.Dummy)] = new UpgradeValue { type = UpgradeValueType.Int, intValue = 0 };


        }
    }



    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        InitializeMerchantUpgradeTypes();
    }



    private void InitializeMerchantUpgradeTypes()
    {
        merchantUpgrades = new Dictionary<Merchants, MerchantUpgrades>();

        foreach (Merchants merchant in Enum.GetValues(typeof(Merchants)))
        {
            MerchantUpgrades info = new MerchantUpgrades();
            info.InitializeDefaults();
            merchantUpgrades.Add(merchant, info);
        }
    }


    public UpgradeValue Modify(
        UpgradeID id,
        UpgradeOperation op,
        Merchants merchant,
        CurrencyTypes currencyType,
        object amount = null)
    {
        var value = merchantUpgrades[merchant].upgrades[(id, currencyType)];

        switch (op)
        {
            case UpgradeOperation.Get:
                return value;

            case UpgradeOperation.Add:
                value.Add(amount);
                return value;

            case UpgradeOperation.Subtract:
                value.Sub(amount);
                return value;
            case UpgradeOperation.Set:
                value.Set(amount);
                return value;

            default:
                return value;
        }
    }

    public float GetFloat(UpgradeID id, Merchants merchant, CurrencyTypes currencyType)
    {
        return merchantUpgrades[merchant].upgrades[(id, currencyType)].floatValue;
    }

    public int GetInt(UpgradeID id, Merchants merchant, CurrencyTypes currencyType)
    {
        return merchantUpgrades[merchant].upgrades[(id, currencyType)].intValue;
    }

    public AlphabeticNotation GetAlphabetic(UpgradeID id, Merchants merchant, CurrencyTypes currencyType)
    {
        return merchantUpgrades[merchant].upgrades[(id, currencyType)].alphabetic;
    }
    public bool GetBool(UpgradeID id, Merchants merchant, CurrencyTypes currencyType)
    {
        return merchantUpgrades[merchant].upgrades[(id, currencyType)].boolState;
    }

    // ---------------------------- UNIFIED MULTIES ----------------------------//

    // ------------------- rewards ------------------ //
    // public float AnyGetRewardMulti(Merchants merchant, CurrencyTypes type) => merchantUpgrades[merchant].rewardMulti[type];
    // public float AnyAddRewardMulti(Merchants merchant, CurrencyTypes type, float amount) => merchantUpgrades[merchant].rewardMulti[type] += amount;
    // public float AnySubtractRewardMulti(Merchants merchant, CurrencyTypes type, float amount) => merchantUpgrades[merchant].rewardMulti[type] -= amount;

    // public AlphabeticNotation AnyGetRewardFlat(Merchants merchant, CurrencyTypes type)=> merchantUpgrades[merchant].rewardFlat[type];
    // public AlphabeticNotation AnyAddRewardFlat(Merchants merchant, CurrencyTypes type, AlphabeticNotation amount) => merchantUpgrades[merchant].rewardFlat[type] += amount;
    // public AlphabeticNotation AnySubtractRewardFlat(Merchants merchant, CurrencyTypes type, AlphabeticNotation amount) => merchantUpgrades[merchant].rewardFlat[type] -= amount;

    // public int AnyGetRewardFlatInt(Merchants merchant, CurrencyTypes type)=> merchantUpgrades[merchant].rewardWeigths[type];
    // public int AnyAddRewardFlatInt(Merchants merchant, CurrencyTypes type, int amount) => merchantUpgrades[merchant].rewardWeigths[type] += amount;
    // public int AnySubtractRewardFlatInt(Merchants merchant, CurrencyTypes type, int amount) => merchantUpgrades[merchant].rewardWeigths[type] -= amount;

    //     // ------------------- EXP ------------------ //
    // public float AnyGetExpMulti(Merchants merchant, CurrencyTypes type) => merchantUpgrades[merchant].xpGain;
    // public float AnyAddExpMulti(Merchants merchant, CurrencyTypes type, float amount) => merchantUpgrades[merchant].xpGain += amount;
    // public float AnySubtractExpMulti(Merchants merchant, CurrencyTypes type, float amount) => merchantUpgrades[merchant].xpGain -= amount;

}



//------------------------------------------------ FLOAT DATATYPE ------------------------------------------ //
// private float GetDefaultValueForrewardMulti(rewardMulti type)
// {
//     return type switch
//     {
//         _ => 1f
//     };
// }