using UnityEngine;
using LargeNumbers;
using System;
public enum UpgradeTypes{
    UnlockArea,
    SetActivationState,
    AddPowerFlat,
    AddPowerMulti,
    BobAddFlat,
    BobAddFlatToMulti,
    BobIncreaseChanceForMoneyReward,
    BobUpgradeXpForAllMerchants,
    CarlAddFlat,
    CarlAddFlatToMulti,
    ChloeAddFlat,
    ChloeAddFlatToMulti,
    FredAddFlat,
    FredAddFlatToMulti,
    SamAddFlat,
    SamAddFlatToMulti,
    RogerAddFlat,
    RogerAddFlatToMulti,
    RogerAddAllCurrencyFlat,

}

public interface IUpgradeEffect
{
    void Apply(GameObject target = null);
}

[CreateAssetMenu(fileName = "Upgrades", menuName = "Upgrade/UpgradeEffect Data")]
public class UpgradeEffect : ScriptableObject
{
    [Header("Chose upgrade type. This has to be chosen for any upgrade")]
    public UpgradeTypes upgradeTypes;

    [Header("Chose currency to upgrade")]
    public CurrencyTypes currencyTypes;
    [Header("Enable to make the upgrade a activation unlock")]
    public ActivationUnlocks activationUnlocks;
    public bool activationState;
    [Header("Spesific upgrades for the merchants at barter trades. Ignore this if not for merchant")]
    [Header("Chose merchant")]
    public Merchants merchants;
    [Header("Chose Type")]
    public BobUpgradeTypes bobUpgradeTypes;
    public BobUpgradeTypesInt bobUpgradeTypesInt;
    public BobUpgradeTypesFloats bobUpgradeTypesFloat;
    public CarlUpgradeTypes carlUpgradeTypes;
    public CarlUpgradeTypesFloats carlUpgradeTypesFloat;
    public ChloeUpgradeTypes chloeUpgradeTypes;
    public ChloeUpgradeTypesFloats chloeUpgradeTypesFloat;
    public FredUpgradeTypes fredUpgradeTypes;
    public FredUpgradeTypesFloats fredUpgradeTypesFloat;
    public SamUpgradeTypes samUpgradeTypes;
    public SamUpgradeTypesFloats samUpgradeTypesFloat;
    public RogerUpgradeTypes rogerUpgradeTypes;
    public RogerUpgradeTypesFloats rogerUpgradeTypesFloat;


    [Header("Define the amount of the upgrade")]
    public AlphabeticNotation flat;
    public AlphabeticNotation multi;
    public int flat_forIntUpgrades;
    public float flat_forFloatUpgrades;
    public void Apply(GameObject target = null){


        switch(upgradeTypes){
            // -------- AREA UNLOCKS -------- //
            case UpgradeTypes.UnlockArea:
                target?.SetActive(true);
                break;
            // -------- AREA UNLOCKS -------- //
            case UpgradeTypes.SetActivationState:
                UpgradeManager.Instance.SetActivationUnlock(activationUnlocks, activationState);
                break;
            // -------- POWER UPGRADES -------- //
            case UpgradeTypes.AddPowerFlat:
                UpgradeManager.Instance.AddPowerFlat(currencyTypes,flat);
                break;
            case UpgradeTypes.AddPowerMulti:
                UpgradeManager.Instance.AddPowerMulti(currencyTypes, multi);
                break;
            // -------- MERCHANT UPGRADES-------- //
                    //..BOB..//
            case UpgradeTypes.BobAddFlat:
                MerchantUpgradeManager.Instance.BobAddFlatReward(bobUpgradeTypes, flat);
                break;
            case UpgradeTypes.BobAddFlatToMulti: // adding more to the multiplier for bobs rewards
                MerchantUpgradeManager.Instance.BobAddToMultiReward(bobUpgradeTypes, flat);
                break;
            case UpgradeTypes.BobIncreaseChanceForMoneyReward: // adding more to the multiplier for bobs rewards
                MerchantUpgradeManager.Instance.BobAddFlatRewardInt(bobUpgradeTypesInt, flat_forIntUpgrades);
                break;
            case UpgradeTypes.BobUpgradeXpForAllMerchants:
                MerchantUpgradeManager.Instance.BobAddFlatRewardFloat(BobUpgradeTypesFloats.xpGainBonusMulti,flat_forFloatUpgrades);
                MerchantUpgradeManager.Instance.CarlAddFlatRewardFloat(CarlUpgradeTypesFloats.xpGainBonusMulti,flat_forFloatUpgrades);
                MerchantUpgradeManager.Instance.ChloeAddFlatRewardFloat(ChloeUpgradeTypesFloats.xpGainBonusMulti,flat_forFloatUpgrades);
                MerchantUpgradeManager.Instance.FredAddFlatRewardFloat(FredUpgradeTypesFloats.xpGainBonusMulti,flat_forFloatUpgrades);
                MerchantUpgradeManager.Instance.SamAddFlatRewardFloat(SamUpgradeTypesFloats.xpGainBonusMulti,flat_forFloatUpgrades);
                MerchantUpgradeManager.Instance.RogerAddFlatRewardFloat(RogerUpgradeTypesFloats.xpGainBonusMulti,flat_forFloatUpgrades);
                break;
                    //..CARL..//
            case UpgradeTypes.CarlAddFlat:
                MerchantUpgradeManager.Instance.CarlAddFlatReward(carlUpgradeTypes,flat);
                break;
            case UpgradeTypes.CarlAddFlatToMulti: // adding more to the multiplier for bobs rewards
                MerchantUpgradeManager.Instance.CarlAddFlatReward(carlUpgradeTypes, flat);
                break;
                    //..CHLOE..//
            case UpgradeTypes.ChloeAddFlat:
                MerchantUpgradeManager.Instance.ChloeAddFlatReward(chloeUpgradeTypes,flat);
                break;
            case UpgradeTypes.ChloeAddFlatToMulti: // adding more to the multiplier for bobs rewards
                MerchantUpgradeManager.Instance.ChloeAddFlatReward(chloeUpgradeTypes, flat);
                break;
                    //..FRED..//
            case UpgradeTypes.FredAddFlat:
                MerchantUpgradeManager.Instance.FredAddFlatReward(fredUpgradeTypes,flat);
                break;
            case UpgradeTypes.FredAddFlatToMulti: // adding more to the multiplier for bobs rewards
                MerchantUpgradeManager.Instance.FredAddFlatReward(fredUpgradeTypes, flat);
                break;
                    //..SAM..//
            case UpgradeTypes.SamAddFlat:
                MerchantUpgradeManager.Instance.SamAddFlatReward(samUpgradeTypes,flat);
                break;
            case UpgradeTypes.SamAddFlatToMulti: // adding more to the multiplier for bobs rewards
                MerchantUpgradeManager.Instance.SamAddFlatReward(samUpgradeTypes, flat);
                break;
                    //..ROGER..//
            case UpgradeTypes.RogerAddFlat:
                MerchantUpgradeManager.Instance.RogerAddFlatReward(rogerUpgradeTypes,flat);
                break;
            case UpgradeTypes.RogerAddFlatToMulti: // adding more to the multiplier for bobs rewards
                MerchantUpgradeManager.Instance.RogerAddFlatReward(rogerUpgradeTypes, flat);
                break;
            case UpgradeTypes.RogerAddAllCurrencyFlat: // adding more to the multiplier for bobs rewards
                MerchantUpgradeManager.Instance.RogerAddFlatReward(rogerUpgradeTypes, flat);
                break;
        }
    }
    
}
