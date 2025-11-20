using System;
using System.Collections.Generic;
using LargeNumbers;
using UnityEngine;



public enum UnifiedFlats{
    bobRewardFlat,
    carlRewardFlat,
    chloeRewardFlat,
    fredRewardFlat,
    samRewardFlat,
    rogerRewardFlat,
}
public enum UnifiedMulties{
    bobRewardMulti,
    carlRewardMulti,
    chloeRewardMulti,
    fredRewardMulti,
    samRewardMulti,
    rogerRewardMulti,
}
public enum UnifiedRewardCurrencyWeigths{
    bobCurrAppearChance,
    carlCurrAppearChance,
    chloeCurrAppearChance,
    fredCurrAppearChance,
    samCurrAppearChance,
    rogerCurrAppearChance,    
}


// public enum BobUpgradeTypes{
//     rewardMultiBob,
//     rewardFlatBob,
//     rewardFlatBob_2,
//     multiAll_resetOnOther,
// }
// public enum BobUpgradeTypesInt{
//     moneyWeightChanceBob,
//     appearChanceWeigthMulti,
// }
// public enum BobUpgradeTypesFloats{
//     increaseAllXpBonusMulti,
//     xpGainBonusMulti,
//     increaseAllSpecialBarterChance,
//     chanceForSpecialBarter,
//     priceMulti,

// }
// public enum CarlUpgradeTypes{
//     rewardMultiCarl,
//     rewardFlatCarl,
// }
// public enum CarlUpgradeTypesInt{
//     refreshCountCarl,
//     appearChanceWeigthMulti,
// }
// public enum CarlUpgradeTypesFloats{
//     xpGainBonusMulti,
//     chanceForSpecialBarter,
//     chanceForFreeRefresh,
//     reduceRefreshTime,
//     chanceNotConsumeOnClaim,
//     multiPriceMultiXp,
//     giveWheatOnComplete,
//     priceMulti,

// }

// public enum ChloeUpgradeTypes{
// }
// public enum ChloeUpgradeTypesInt{
//     appearChanceWeigthMulti,
// }

// public enum ChloeUpgradeTypesBool{
//     doubleXpOnNextBarter,
// }
// public enum ChloeUpgradeTypesFloats{
//     xpGainBonusMulti,
//     chanceForSpecialBarter,
//     increaseAllXpBonusMulti,
//     priceMulti,
//     multiAllOnFavorPassed,
//     multiForAllFavorGain,
// }
// public enum FredUpgradeTypes{
//     rewardMultiFred,
//     rewardFlatFred,
// }
// public enum FredUpgradeTypesInt{
//     appearChanceWeigthMulti,
//     decreaseAllOtherFavor,
//     increaseFredFavor,
// }
// public enum FredUpgradeTypesFloats{
//     xpGainBonusMulti,
//     chanceForSpecialBarter,
//     priceMulti,
//     chanceToRecieveNothing,

// }
// public enum SamUpgradeTypes{
//     rewardMultiSam,
//     rewardFlatSam,
// }
// public enum SamUpgradeTypesInt{
//     appearChanceWeigthMulti,
// }
// public enum SamUpgradeTypesFloats{
//     xpGainBonusMulti,
//     chanceForSpecialBarter,
//     priceMulti,

// }
// public enum RogerUpgradeTypes{
//     rewardMultiRoger,
//     rewardFlatRoger,
    
// }
// public enum RogerUpgradeTypesInt{
//     appearChanceWeigthMulti,
// }
// public enum RogerUpgradeTypesFloats{
//     xpGainBonusMulti,

//     chanceForSpecialBarter,
//     priceMulti,
// }


public class MerchantUpgradeManager : MonoBehaviour
{
    public static MerchantUpgradeManager Instance {get;private set;}



    // ---------------------------- UNIFIED ----------------------------//

    //TESTING NEW SYSTEM:
    [SerializeField] public Dictionary<Merchants,MerchantUpgrades> merchantUpgrades;
    //TESTING NEW SYSTEM:



    [System.Serializable]
    public class MerchantUpgrades{
        public Dictionary<CurrencyTypes, AlphabeticNotation> unifiedFlats = new();
        public Dictionary<CurrencyTypes, float> unifiedMulties = new();
        public Dictionary<CurrencyTypes, int> unifiedRewardWeigths = new();

        public void InitializeDefaults()
        {
            foreach (CurrencyTypes type in Enum.GetValues(typeof(CurrencyTypes)))
            {
                unifiedFlats[type] = new AlphabeticNotation(0);
                unifiedMulties[type] = 1f;
                unifiedRewardWeigths[type] = 1;
            }
        }
    }





    private void Awake(){
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
// ---------------------------- UNIFIED MULTIES ----------------------------//

    public float AnyGetRewardMulti(Merchants merchant, CurrencyTypes type) => merchantUpgrades[merchant].unifiedMulties[type];
    public float AnyAddRewardMulti(Merchants merchant, CurrencyTypes type, float amount) => merchantUpgrades[merchant].unifiedMulties[type] += amount;
    public float AnySubtractRewardMulti(Merchants merchant, CurrencyTypes type, float amount) => merchantUpgrades[merchant].unifiedMulties[type] -= amount;

    public AlphabeticNotation AnyGetRewardFlat(Merchants merchant, CurrencyTypes type)=> merchantUpgrades[merchant].unifiedFlats[type];
    public AlphabeticNotation AnyAddRewardFlat(Merchants merchant, CurrencyTypes type, AlphabeticNotation amount) => merchantUpgrades[merchant].unifiedFlats[type] += amount;
    public AlphabeticNotation AnySubtractRewardFlat(Merchants merchant, CurrencyTypes type, AlphabeticNotation amount) => merchantUpgrades[merchant].unifiedFlats[type] -= amount;

    public int AnyGetRewardFlatInt(Merchants merchant, CurrencyTypes type)=> merchantUpgrades[merchant].unifiedRewardWeigths[type];
    public int AnyAddRewardFlatInt(Merchants merchant, CurrencyTypes type, int amount) => merchantUpgrades[merchant].unifiedRewardWeigths[type] += amount;
    public int AnySubtractRewardFlatInt(Merchants merchant, CurrencyTypes type, int amount) => merchantUpgrades[merchant].unifiedRewardWeigths[type] -= amount;


}



//------------------------------------------------ FLOAT DATATYPE ------------------------------------------ //
// private float GetDefaultValueForUnifiedMulties(UnifiedMulties type)
// {
//     return type switch
//     {
//         _ => 1f
//     };
// }