using UnityEngine;
using LargeNumbers;
using System;
using UnityEngine.Assertions.Must;
public enum MerchantUpgradeTypes{
    BobAddFlat,
    BobAddFlatToMulti,
    BobIncreaseChanceForMoneyReward,
    BobUpgradeXpForAllMerchants,
    BobStackMultiFlat_ResetOnOtherBarter,
    BobIncreaseAllChanceForSpecial,
    CarlAddFlat,
    CarlAddFlatToMulti,
    CarlAddRefreshCount,
    CarlFreeRefreshChance,
    CarlReduceRefreshTime,
    CarlChanceToNotConsumeClaim,
    CarlMultiPriceMultiXp,
    CarlGiveWheatOnComplete,
    ChloeAddFlat,
    ChloeAddFlatToMulti,
    ChloeUpgradeXpForAllMerchants,
    ChloeMultiAllOnFavorPassed,
    ChloeDoubleXpOnNextBarter,
    ChloeAddFlatMultiFavor,
    ChloeAddChanceOfAppearing,
    FredAddFlat,
    FredAddFlatToMulti,
    SamAddFlat,
    SamAddFlatToMulti,
    RogerAddFlat,
    RogerAddFlatToMulti,
    RogerAddAllCurrencyFlat,

}

public interface IUpgradeEffectMerchants
{
    void Apply(GameObject target = null);
}

[CreateAssetMenu(fileName = "UpgradeEffectMerchants", menuName = "Upgrade/Upgrade effect merchants")]
public class UpgradeEffectMerchants : ScriptableObject
{
    [Header("Chose upgrade type. This has to be chosen for any upgrade")]
    public MerchantUpgradeTypes merchantUpgradeTypes;

    [Header("Chose currency to upgrade")]
    public CurrencyTypes currencyTypes;

    [Header("Spesific upgrades for the merchants at barter trades. Ignore this if not for merchant")]
    [Header("Chose merchant")]
    public Merchants merchants;
    [Header("Chose Type")]

    [Header("BOB UPGRADES")]
    public BobUpgradeTypes bobUpgradeTypes;
    public BobUpgradeTypesInt bobUpgradeTypesInt;
    public BobUpgradeTypesFloats bobUpgradeTypesFloat;
    [Header("CARL UPGRADES")]
    public CarlUpgradeTypes carlUpgradeTypes;
    public CarlUpgradeTypesInt carlUpgradeTypesInt;
    public CarlUpgradeTypesFloats carlUpgradeTypesFloat;
    [Header("CHLOE UPGRADES")]
    public ChloeUpgradeTypes chloeUpgradeTypes;
    public ChloeUpgradeTypesInt chloeUpgradeTypesInt;
    public ChloeUpgradeTypesFloats chloeUpgradeTypesFloat;
    public ChloeUpgradeTypesBool chloeUpgradeTypesBool;
    [Header("FRED UPGRADES")]
    public FredUpgradeTypes fredUpgradeTypes;
    public FredUpgradeTypesInt fredUpgradeTypesInt;
    public FredUpgradeTypesFloats fredUpgradeTypesFloat;
    [Header("SAM UPGRADES")]
    public SamUpgradeTypes samUpgradeTypes;
    public SamUpgradeTypesInt samUpgradeTypesInt;
    public SamUpgradeTypesFloats samUpgradeTypesFloat;
    [Header("ROGER UPGRADES")]
    public RogerUpgradeTypes rogerUpgradeTypes;
    public RogerUpgradeTypesInt rogerUpgradeTypesInt;
    public RogerUpgradeTypesFloats rogerUpgradeTypesFloat;


    [Header("Define the amount of the upgrade")]
    public AlphabeticNotation flat;
    public AlphabeticNotation multi;
    public int flat_forIntUpgrades;
    public float flat_forFloatUpgrades;
    public bool stateForUpgrade = false;
    public void Apply(GameObject target = null){


        switch(merchantUpgradeTypes){
            // -------- MERCHANT UPGRADES-------- //
                    //..BOB..//
            case MerchantUpgradeTypes.BobAddFlat:
                MerchantUpgradeManager.Instance.BobAddFlatReward(bobUpgradeTypes, flat);
                break;
            case MerchantUpgradeTypes.BobAddFlatToMulti: // adding more to the multiplier for bobs rewards
                MerchantUpgradeManager.Instance.BobAddToMultiReward(bobUpgradeTypes, flat);
                break;
            case MerchantUpgradeTypes.BobIncreaseChanceForMoneyReward: // adding more to the multiplier for bobs rewards
                MerchantUpgradeManager.Instance.BobAddFlatRewardInt(bobUpgradeTypesInt, flat_forIntUpgrades);
                break;
            case MerchantUpgradeTypes.BobUpgradeXpForAllMerchants:
                MerchantUpgradeManager.Instance.BobAddFlatRewardFloat(BobUpgradeTypesFloats.xpGainBonusMulti,flat_forFloatUpgrades);
                MerchantUpgradeManager.Instance.CarlAddFlatRewardFloat(CarlUpgradeTypesFloats.xpGainBonusMulti,flat_forFloatUpgrades);
                MerchantUpgradeManager.Instance.ChloeAddFlatRewardFloat(ChloeUpgradeTypesFloats.xpGainBonusMulti,flat_forFloatUpgrades);
                MerchantUpgradeManager.Instance.FredAddFlatRewardFloat(FredUpgradeTypesFloats.xpGainBonusMulti,flat_forFloatUpgrades);
                MerchantUpgradeManager.Instance.SamAddFlatRewardFloat(SamUpgradeTypesFloats.xpGainBonusMulti,flat_forFloatUpgrades);
                MerchantUpgradeManager.Instance.RogerAddFlatRewardFloat(RogerUpgradeTypesFloats.xpGainBonusMulti,flat_forFloatUpgrades);
                
                MerchantUpgradeManager.Instance.BobAddFlatRewardFloat(BobUpgradeTypesFloats.increaseAllXpBonusMulti,flat_forFloatUpgrades); //only used for displaying 
                                                                                                                                            //total bonus in description
                break;
            case MerchantUpgradeTypes.BobStackMultiFlat_ResetOnOtherBarter:
                MerchantUpgradeManager.Instance.BobAddFlatReward(bobUpgradeTypes, flat);
                break;
            case MerchantUpgradeTypes.BobIncreaseAllChanceForSpecial:
                MerchantUpgradeManager.Instance.BobAddFlatRewardFloat(bobUpgradeTypesFloat, flat_forFloatUpgrades);
                break;
                    //..CARL..//
            case MerchantUpgradeTypes.CarlAddFlat:
                MerchantUpgradeManager.Instance.CarlAddFlatReward(carlUpgradeTypes,flat);
                break;
            case MerchantUpgradeTypes.CarlAddFlatToMulti: 
                MerchantUpgradeManager.Instance.CarlAddFlatReward(carlUpgradeTypes, flat);
                break;
            case MerchantUpgradeTypes.CarlAddRefreshCount:
                MerchantUpgradeManager.Instance.CarlAddFlatRewardInt(carlUpgradeTypesInt,flat_forIntUpgrades);
                break;
            case MerchantUpgradeTypes.CarlFreeRefreshChance:
                MerchantUpgradeManager.Instance.CarlAddFlatRewardFloat(carlUpgradeTypesFloat,flat_forFloatUpgrades);
                break;
            case MerchantUpgradeTypes.CarlReduceRefreshTime:
                MerchantUpgradeManager.Instance.CarlSubtractFlatRewardFloat(carlUpgradeTypesFloat,flat_forFloatUpgrades);
                break;
            case MerchantUpgradeTypes.CarlChanceToNotConsumeClaim:
                MerchantUpgradeManager.Instance.CarlAddFlatRewardFloat(carlUpgradeTypesFloat,flat_forFloatUpgrades);
                break;
            case MerchantUpgradeTypes.CarlMultiPriceMultiXp:
                MerchantUpgradeManager.Instance.BobAddFlatRewardFloat(BobUpgradeTypesFloats.xpGainBonusMulti,flat_forFloatUpgrades);
                MerchantUpgradeManager.Instance.CarlAddFlatRewardFloat(CarlUpgradeTypesFloats.xpGainBonusMulti,flat_forFloatUpgrades);
                MerchantUpgradeManager.Instance.ChloeAddFlatRewardFloat(ChloeUpgradeTypesFloats.xpGainBonusMulti,flat_forFloatUpgrades);
                MerchantUpgradeManager.Instance.FredAddFlatRewardFloat(FredUpgradeTypesFloats.xpGainBonusMulti,flat_forFloatUpgrades);
                MerchantUpgradeManager.Instance.SamAddFlatRewardFloat(SamUpgradeTypesFloats.xpGainBonusMulti,flat_forFloatUpgrades);
                MerchantUpgradeManager.Instance.RogerAddFlatRewardFloat(RogerUpgradeTypesFloats.xpGainBonusMulti,flat_forFloatUpgrades);

                MerchantUpgradeManager.Instance.BobAddFlatRewardFloat(BobUpgradeTypesFloats.priceMulti,flat_forFloatUpgrades);
                MerchantUpgradeManager.Instance.CarlAddFlatRewardFloat(CarlUpgradeTypesFloats.priceMulti,flat_forFloatUpgrades);
                MerchantUpgradeManager.Instance.ChloeAddFlatRewardFloat(ChloeUpgradeTypesFloats.priceMulti,flat_forFloatUpgrades);
                MerchantUpgradeManager.Instance.FredAddFlatRewardFloat(FredUpgradeTypesFloats.priceMulti,flat_forFloatUpgrades);
                MerchantUpgradeManager.Instance.SamAddFlatRewardFloat(SamUpgradeTypesFloats.priceMulti,flat_forFloatUpgrades);
                MerchantUpgradeManager.Instance.RogerAddFlatRewardFloat(RogerUpgradeTypesFloats.priceMulti,flat_forFloatUpgrades);

                MerchantUpgradeManager.Instance.CarlAddFlatRewardFloat(CarlUpgradeTypesFloats.multiPriceMultiXp,flat_forFloatUpgrades); //only used for displaying 
                                                                                                                                        //total bonus in description
                break;
            case MerchantUpgradeTypes.CarlGiveWheatOnComplete:
                MerchantUpgradeManager.Instance.CarlAddFlatRewardFloat(carlUpgradeTypesFloat,flat_forFloatUpgrades);
                break;
                    //..CHLOE..//
            case MerchantUpgradeTypes.ChloeUpgradeXpForAllMerchants:
                MerchantUpgradeManager.Instance.BobAddFlatRewardFloat(BobUpgradeTypesFloats.xpGainBonusMulti,flat_forFloatUpgrades);
                MerchantUpgradeManager.Instance.CarlAddFlatRewardFloat(CarlUpgradeTypesFloats.xpGainBonusMulti,flat_forFloatUpgrades);
                MerchantUpgradeManager.Instance.ChloeAddFlatRewardFloat(ChloeUpgradeTypesFloats.xpGainBonusMulti,flat_forFloatUpgrades);
                MerchantUpgradeManager.Instance.FredAddFlatRewardFloat(FredUpgradeTypesFloats.xpGainBonusMulti,flat_forFloatUpgrades);
                MerchantUpgradeManager.Instance.SamAddFlatRewardFloat(SamUpgradeTypesFloats.xpGainBonusMulti,flat_forFloatUpgrades);
                MerchantUpgradeManager.Instance.RogerAddFlatRewardFloat(RogerUpgradeTypesFloats.xpGainBonusMulti,flat_forFloatUpgrades);

                MerchantUpgradeManager.Instance.ChloeAddFlatRewardFloat(ChloeUpgradeTypesFloats.increaseAllXpBonusMulti,flat_forFloatUpgrades); //only used for displaying 
                                                                                                                                                //total bonus in description
                break;
            case MerchantUpgradeTypes.ChloeMultiAllOnFavorPassed:
                MerchantUpgradeManager.Instance.ChloeAddFlatRewardFloat(ChloeUpgradeTypesFloats.multiAllOnFavorPassed,flat_forFloatUpgrades); //only used for displaying 
                                                                                                                                                //total bonus in description
                break;
            case MerchantUpgradeTypes.ChloeDoubleXpOnNextBarter:
                MerchantUpgradeManager.Instance.ChloeRewardSetBool(ChloeUpgradeTypesBool.doubleXpOnNextBarter,stateForUpgrade);
                break;
            case MerchantUpgradeTypes.ChloeAddFlatMultiFavor:
                MerchantUpgradeManager.Instance.ChloeAddFlatRewardFloat(chloeUpgradeTypesFloat,flat_forFloatUpgrades);
                break;
            case MerchantUpgradeTypes.ChloeAddChanceOfAppearing:
                MerchantUpgradeManager.Instance.ChloeAddFlatRewardInt(chloeUpgradeTypesInt,flat_forIntUpgrades);
                break;

                    //..FRED..//
            case MerchantUpgradeTypes.FredAddFlat:
                MerchantUpgradeManager.Instance.FredAddFlatReward(fredUpgradeTypes,flat);
                break;
            case MerchantUpgradeTypes.FredAddFlatToMulti: // adding more to the multiplier for bobs rewards
                MerchantUpgradeManager.Instance.FredAddFlatReward(fredUpgradeTypes, flat);
                break;
                    //..SAM..//
            case MerchantUpgradeTypes.SamAddFlat:
                MerchantUpgradeManager.Instance.SamAddFlatReward(samUpgradeTypes,flat);
                break;
            case MerchantUpgradeTypes.SamAddFlatToMulti: // adding more to the multiplier for bobs rewards
                MerchantUpgradeManager.Instance.SamAddFlatReward(samUpgradeTypes, flat);
                break;
                    //..ROGER..//
            case MerchantUpgradeTypes.RogerAddFlat:
                MerchantUpgradeManager.Instance.RogerAddFlatReward(rogerUpgradeTypes,flat);
                break;
            case MerchantUpgradeTypes.RogerAddFlatToMulti: // adding more to the multiplier for bobs rewards
                MerchantUpgradeManager.Instance.RogerAddFlatReward(rogerUpgradeTypes, flat);
                break;
            case MerchantUpgradeTypes.RogerAddAllCurrencyFlat: // adding more to the multiplier for bobs rewards
                MerchantUpgradeManager.Instance.RogerAddFlatReward(rogerUpgradeTypes, flat);
                break;
        }
    }
    
}
