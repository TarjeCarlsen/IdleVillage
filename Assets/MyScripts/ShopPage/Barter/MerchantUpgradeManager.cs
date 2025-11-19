using System;
using System.Collections.Generic;
using LargeNumbers;
using UnityEngine;



public enum BobUpgradeTypes{
    rewardMultiBob,
    rewardFlatBob,
    rewardFlatBob_2,
    multiAll_resetOnOther,
}
public enum BobUpgradeTypesInt{
    moneyWeightChanceBob,
    appearChanceWeigthMulti,
}
public enum BobUpgradeTypesFloats{
    increaseAllXpBonusMulti,
    xpGainBonusMulti,
    increaseAllSpecialBarterChance,
    chanceForSpecialBarter,
    priceMulti,

}
public enum CarlUpgradeTypes{
    rewardMultiCarl,
    rewardFlatCarl,
}
public enum CarlUpgradeTypesInt{
    refreshCountCarl,
    appearChanceWeigthMulti,
}
public enum CarlUpgradeTypesFloats{
    xpGainBonusMulti,
    chanceForSpecialBarter,
    chanceForFreeRefresh,
    reduceRefreshTime,
    chanceNotConsumeOnClaim,
    multiPriceMultiXp,
    giveWheatOnComplete,
    priceMulti,

}

public enum ChloeUpgradeTypes{
}
public enum ChloeUpgradeTypesInt{
    appearChanceWeigthMulti,
}

public enum ChloeUpgradeTypesBool{
    doubleXpOnNextBarter,
}
public enum ChloeUpgradeTypesFloats{
    xpGainBonusMulti,
    chanceForSpecialBarter,
    increaseAllXpBonusMulti,
    priceMulti,
    multiAllOnFavorPassed,
    multiForAllFavorGain,
}
public enum FredUpgradeTypes{
    rewardMultiFred,
    rewardFlatFred,
}
public enum FredUpgradeTypesInt{
    appearChanceWeigthMulti,
}
public enum FredUpgradeTypesFloats{
    xpGainBonusMulti,
    chanceForSpecialBarter,
    priceMulti,
    chanceToRecieveNothing,

}
public enum SamUpgradeTypes{
    rewardMultiSam,
    rewardFlatSam,
}
public enum SamUpgradeTypesInt{
    appearChanceWeigthMulti,
}
public enum SamUpgradeTypesFloats{
    xpGainBonusMulti,
    chanceForSpecialBarter,
    priceMulti,

}
public enum RogerUpgradeTypes{
    rewardMultiRoger,
    rewardFlatRoger,
    
}
public enum RogerUpgradeTypesInt{
    appearChanceWeigthMulti,
}
public enum RogerUpgradeTypesFloats{
    xpGainBonusMulti,

    chanceForSpecialBarter,
    priceMulti,
}


public class MerchantUpgradeManager : MonoBehaviour
{
    public static MerchantUpgradeManager Instance {get;private set;}

    [SerializeField] private Merchants merchants;

    // ---------------------------- BOB ----------------------------//
    [SerializeField] public Dictionary<BobUpgradeTypes, AlphabeticNotation> bobUpgrades;
    [SerializeField] public Dictionary<BobUpgradeTypesInt, int> bobUpgradesInt;
    [SerializeField] public Dictionary<BobUpgradeTypesFloats, float> bobUpgradesFloat;
    // ---------------------------- CARL ----------------------------//
    [SerializeField] public Dictionary <CarlUpgradeTypes, AlphabeticNotation> carlUpgrades;
    [SerializeField] public Dictionary<CarlUpgradeTypesInt, int> carlUpgradesInt;
    [SerializeField] public Dictionary<CarlUpgradeTypesFloats, float> carlUpgradesFloat;

    // ---------------------------- CHLOE ----------------------------//
    [SerializeField] public Dictionary <ChloeUpgradeTypes, AlphabeticNotation> chloeUpgrades;
    [SerializeField] public Dictionary <ChloeUpgradeTypesInt, int> chloeUpgradesInt;
    [SerializeField] public Dictionary<ChloeUpgradeTypesFloats, float> chloeUpgradesFloat;
    [SerializeField] public Dictionary<ChloeUpgradeTypesBool,bool> chloeUpgradesBool;
    // ---------------------------- FRED ----------------------------//
    [SerializeField] public Dictionary <FredUpgradeTypes, AlphabeticNotation> fredUpgrades;
    [SerializeField] public Dictionary <FredUpgradeTypesInt, int> fredUpgradesInt;
    [SerializeField] public Dictionary<FredUpgradeTypesFloats, float> fredUpgradesFloat;
    // ---------------------------- SAM ----------------------------//
    [SerializeField] public Dictionary <SamUpgradeTypes, AlphabeticNotation> samUpgrades;
    [SerializeField] public Dictionary <SamUpgradeTypesInt, int> samUpgradesInt;
    [SerializeField] public Dictionary<SamUpgradeTypesFloats, float> samUpgradesFloat;
    // ---------------------------- ROGER ----------------------------//
    [SerializeField] public Dictionary <RogerUpgradeTypes, AlphabeticNotation>rogerUpgrades;
    [SerializeField] public Dictionary <RogerUpgradeTypesInt, int> rogerUpgradesInt;
    [SerializeField] public Dictionary<RogerUpgradeTypesFloats, float> rogerUpgradesFloat;
    private void Awake(){
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        InitializeMerchantUpgrades();
    }
//------------------------------------------------ ALPHABETIC NOTATION DATATYPE ------------------------------------------ //
private AlphabeticNotation GetDefaultValueForBob(BobUpgradeTypes type)
{
    return type switch
    {
        BobUpgradeTypes.rewardMultiBob => new AlphabeticNotation(1),
        BobUpgradeTypes.rewardFlatBob => new AlphabeticNotation(0),
        BobUpgradeTypes.rewardFlatBob_2 => new AlphabeticNotation(0),
        BobUpgradeTypes.multiAll_resetOnOther => new AlphabeticNotation(1f),
        _ => new AlphabeticNotation(0)
    };
}

private AlphabeticNotation GetDefaultValueForCarl(CarlUpgradeTypes type)
{
    return type switch
    {
        CarlUpgradeTypes.rewardMultiCarl => new AlphabeticNotation(1),
        CarlUpgradeTypes.rewardFlatCarl => new AlphabeticNotation(0),
        _ => new AlphabeticNotation(0)
    };
}

private AlphabeticNotation GetDefaultValueForChloe(ChloeUpgradeTypes type)
{
    return type switch
    {
        
        _ => new AlphabeticNotation(0)
    };
}

private AlphabeticNotation GetDefaultValueForFred(FredUpgradeTypes type)
{
    return type switch
    {
        FredUpgradeTypes.rewardMultiFred => new AlphabeticNotation(1),
        FredUpgradeTypes.rewardFlatFred => new AlphabeticNotation(0),
        _ => new AlphabeticNotation(0)
    };
}

private AlphabeticNotation GetDefaultValueForSam(SamUpgradeTypes type)
{
    return type switch
    {
        SamUpgradeTypes.rewardMultiSam => new AlphabeticNotation(1),
        SamUpgradeTypes.rewardFlatSam => new AlphabeticNotation(0),
        _ => new AlphabeticNotation(0)
    };
}

private AlphabeticNotation GetDefaultValueForRoger(RogerUpgradeTypes type)
{
    return type switch
    {
        RogerUpgradeTypes.rewardMultiRoger => new AlphabeticNotation(1),
        RogerUpgradeTypes.rewardFlatRoger => new AlphabeticNotation(0),
        _ => new AlphabeticNotation(0)
    };
}
//------------------------------------------------ INT DATATYPE ------------------------------------------ //
private int GetDefaultValueForBob(BobUpgradeTypesInt type)
{
    return type switch
    {
        BobUpgradeTypesInt.appearChanceWeigthMulti => 1,
        BobUpgradeTypesInt.moneyWeightChanceBob => 0,
        _ => 0
    };
}
private int GetDefaultValueForCarl(CarlUpgradeTypesInt type)
{
    return type switch
    {
        CarlUpgradeTypesInt.appearChanceWeigthMulti => 1,
        CarlUpgradeTypesInt.refreshCountCarl => 0,
        _ => 0
    };
}
private int GetDefaultValueForChloe(ChloeUpgradeTypesInt type)
{
    return type switch
    {
        ChloeUpgradeTypesInt.appearChanceWeigthMulti => 1,
        _ => 0
    };
}
private int GetDefaultValueForFred(FredUpgradeTypesInt type)
{
    return type switch
    {
        FredUpgradeTypesInt.appearChanceWeigthMulti => 1,
        _ => 0
    };
}
private int GetDefaultValueForSam(SamUpgradeTypesInt type)
{
    return type switch
    {
        SamUpgradeTypesInt.appearChanceWeigthMulti => 1,
        _ => 0
    };
}
private int GetDefaultValueForRoger(RogerUpgradeTypesInt type)
{
    return type switch
    {
        RogerUpgradeTypesInt.appearChanceWeigthMulti => 1,
        _ => 0
    };
}


//------------------------------------------------ FLOAT DATATYPE ------------------------------------------ //
private float GetDefaultValueForBob(BobUpgradeTypesFloats type)
{
    return type switch
    {
        BobUpgradeTypesFloats.increaseAllXpBonusMulti => 1f,
        BobUpgradeTypesFloats.xpGainBonusMulti => 1f,
        BobUpgradeTypesFloats.increaseAllSpecialBarterChance => 0f,
        BobUpgradeTypesFloats.chanceForSpecialBarter => 0f,
        _ => 1f
    };
}

private float GetDefaultValueForCarl(CarlUpgradeTypesFloats type)
{
    return type switch
    {
        CarlUpgradeTypesFloats.xpGainBonusMulti => 1f,
        CarlUpgradeTypesFloats.chanceForSpecialBarter => 0f,
        CarlUpgradeTypesFloats.chanceForFreeRefresh => 0f,
        CarlUpgradeTypesFloats.reduceRefreshTime => 1f,
        CarlUpgradeTypesFloats.chanceNotConsumeOnClaim => 0f,
        CarlUpgradeTypesFloats.giveWheatOnComplete => 0f,
        _ => 1f
    };
}

private float GetDefaultValueForChloe(ChloeUpgradeTypesFloats type)
{
    return type switch
    {
        
        ChloeUpgradeTypesFloats.increaseAllXpBonusMulti => 1f,
        ChloeUpgradeTypesFloats.multiAllOnFavorPassed => 1f,
        _ => 1f
    };
}

private float GetDefaultValueForFred(FredUpgradeTypesFloats type)
{
    return type switch
    {
        FredUpgradeTypesFloats.xpGainBonusMulti => 1f,
        FredUpgradeTypesFloats.chanceForSpecialBarter => 0f,
        FredUpgradeTypesFloats.chanceToRecieveNothing => 1f,
        _ => 1f
    };
}

private float GetDefaultValueForSam(SamUpgradeTypesFloats type)
{
    return type switch
    {
        SamUpgradeTypesFloats.xpGainBonusMulti => 1f,
        SamUpgradeTypesFloats.chanceForSpecialBarter => 0f,
        _ => 1f
    };
}

private float GetDefaultValueForRoger(RogerUpgradeTypesFloats type)
{
    return type switch
    {
        RogerUpgradeTypesFloats.xpGainBonusMulti => 1f,
        RogerUpgradeTypesFloats.chanceForSpecialBarter => 0f,
        _ => 1f
    };
}
//------------------------------------------------ Bool DATATYPE ------------------------------------------ //

private bool GetDefaultValueForChloe(ChloeUpgradeTypesBool type)
{
    return type switch
    {
        
        ChloeUpgradeTypesBool.doubleXpOnNextBarter => false,
        _ => false
    };
}
private void InitializeMerchantUpgrades()
{
    // --- Init dictionaries ---
    bobUpgrades = new Dictionary<BobUpgradeTypes, AlphabeticNotation>();
    bobUpgradesInt = new Dictionary<BobUpgradeTypesInt, int>();
    bobUpgradesFloat = new Dictionary<BobUpgradeTypesFloats, float>();

    carlUpgrades = new Dictionary<CarlUpgradeTypes, AlphabeticNotation>();
    carlUpgradesInt = new Dictionary<CarlUpgradeTypesInt, int>();
    carlUpgradesFloat = new Dictionary<CarlUpgradeTypesFloats, float>();

    chloeUpgrades = new Dictionary<ChloeUpgradeTypes, AlphabeticNotation>();
    chloeUpgradesInt = new Dictionary<ChloeUpgradeTypesInt, int>();
    chloeUpgradesFloat = new Dictionary<ChloeUpgradeTypesFloats, float>();
    chloeUpgradesBool = new Dictionary<ChloeUpgradeTypesBool, bool>();

    fredUpgrades = new Dictionary<FredUpgradeTypes, AlphabeticNotation>();
    fredUpgradesInt = new Dictionary<FredUpgradeTypesInt, int>();
    fredUpgradesFloat = new Dictionary<FredUpgradeTypesFloats, float>();

    samUpgrades = new Dictionary<SamUpgradeTypes, AlphabeticNotation>();
    samUpgradesInt = new Dictionary<SamUpgradeTypesInt, int>();
    samUpgradesFloat = new Dictionary<SamUpgradeTypesFloats, float>();

    rogerUpgrades = new Dictionary<RogerUpgradeTypes, AlphabeticNotation>();
    rogerUpgradesInt = new Dictionary<RogerUpgradeTypesInt, int>();
    rogerUpgradesFloat = new Dictionary<RogerUpgradeTypesFloats, float>();

    // --- BOB ---
    foreach (BobUpgradeTypes type in Enum.GetValues(typeof(BobUpgradeTypes)))
        bobUpgrades[type] = GetDefaultValueForBob(type);
    foreach (BobUpgradeTypesInt type in Enum.GetValues(typeof(BobUpgradeTypesInt)))
        bobUpgradesInt[type] = GetDefaultValueForBob(type);
    foreach (BobUpgradeTypesFloats type in Enum.GetValues(typeof(BobUpgradeTypesFloats)))
        bobUpgradesFloat[type] = GetDefaultValueForBob(type);

    // --- CARL ---
    foreach (CarlUpgradeTypes type in Enum.GetValues(typeof(CarlUpgradeTypes)))
        carlUpgrades[type] = GetDefaultValueForCarl(type);
    foreach (CarlUpgradeTypesInt type in Enum.GetValues(typeof(CarlUpgradeTypesInt)))
        carlUpgradesInt[type] = GetDefaultValueForCarl(type);
    foreach (CarlUpgradeTypesFloats type in Enum.GetValues(typeof(CarlUpgradeTypesFloats)))
        carlUpgradesFloat[type] = GetDefaultValueForCarl(type);

    // --- CHLOE ---
    foreach (ChloeUpgradeTypes type in Enum.GetValues(typeof(ChloeUpgradeTypes)))
        chloeUpgrades[type] = GetDefaultValueForChloe(type);
    foreach (ChloeUpgradeTypesInt type in Enum.GetValues(typeof(ChloeUpgradeTypesInt)))
        chloeUpgradesInt[type] = GetDefaultValueForChloe(type);
    foreach (ChloeUpgradeTypesFloats type in Enum.GetValues(typeof(ChloeUpgradeTypesFloats)))
        chloeUpgradesFloat[type] = GetDefaultValueForChloe(type);
    foreach (ChloeUpgradeTypesBool type in Enum.GetValues(typeof(ChloeUpgradeTypesBool)))
        chloeUpgradesBool[type] = GetDefaultValueForChloe(type);

    // --- FRED ---
    foreach (FredUpgradeTypes type in Enum.GetValues(typeof(FredUpgradeTypes)))
        fredUpgrades[type] = GetDefaultValueForFred(type);
    foreach (FredUpgradeTypesInt type in Enum.GetValues(typeof(FredUpgradeTypesInt)))
        fredUpgradesInt[type] = GetDefaultValueForFred(type);
    foreach (FredUpgradeTypesFloats type in Enum.GetValues(typeof(FredUpgradeTypesFloats)))
        fredUpgradesFloat[type] = GetDefaultValueForFred(type);

    // --- SAM ---
    foreach (SamUpgradeTypes type in Enum.GetValues(typeof(SamUpgradeTypes)))
        samUpgrades[type] = GetDefaultValueForSam(type);
    foreach (SamUpgradeTypesInt type in Enum.GetValues(typeof(SamUpgradeTypesInt)))
        samUpgradesInt[type] = GetDefaultValueForSam(type);
    foreach (SamUpgradeTypesFloats type in Enum.GetValues(typeof(SamUpgradeTypesFloats)))
        samUpgradesFloat[type] = GetDefaultValueForSam(type);

    // --- ROGER ---
    foreach (RogerUpgradeTypes type in Enum.GetValues(typeof(RogerUpgradeTypes)))
        rogerUpgrades[type] = GetDefaultValueForRoger(type);
    foreach (RogerUpgradeTypesInt type in Enum.GetValues(typeof(RogerUpgradeTypesInt)))
        rogerUpgradesInt[type] = GetDefaultValueForRoger(type);
    foreach (RogerUpgradeTypesFloats type in Enum.GetValues(typeof(RogerUpgradeTypesFloats)))
        rogerUpgradesFloat[type] = GetDefaultValueForRoger(type);
}





// ---------------------------- BOB ----------------------------//
    public AlphabeticNotation BobGetRewardPower(BobUpgradeTypes type) => bobUpgrades[type];
    public void BobAddFlatReward(BobUpgradeTypes type, AlphabeticNotation amount) => bobUpgrades[type] += amount; 
    public void BobAddToMultiReward(BobUpgradeTypes type, AlphabeticNotation amount) => bobUpgrades[type] += amount;
        // ----------int upgrades ---------//
    public int BobGetRewardPowerInt(BobUpgradeTypesInt type) => bobUpgradesInt[type];
    public void BobAddFlatRewardInt(BobUpgradeTypesInt type, int amount) => bobUpgradesInt[type] += amount; 
    public void BobMultiplyRewardInt(BobUpgradeTypesInt type, int amount) => bobUpgradesInt[type] *= amount;
        // ----------float upgrades ---------//
    public float BobGetRewardPowerFloat(BobUpgradeTypesFloats type) => bobUpgradesFloat[type];
    public void BobAddFlatRewardFloat(BobUpgradeTypesFloats type, float amount) => bobUpgradesFloat[type] += amount; 
    public void BobMultiplyRewardFloat(BobUpgradeTypesFloats type, float amount) => bobUpgradesFloat[type] *= amount;

// ---------------------------- CARL ----------------------------//
    public AlphabeticNotation CarlGetRewardPower(CarlUpgradeTypes type) => carlUpgrades[type];
    public void CarlAddFlatReward(CarlUpgradeTypes type, AlphabeticNotation amount) => carlUpgrades[type] += amount; 
    public void CarlMultiplyReward(CarlUpgradeTypes type, AlphabeticNotation amount) => carlUpgrades[type] *= amount;
        // ----------int upgrades ---------//
    public int CarlGetRewardPowerInt(CarlUpgradeTypesInt type) => carlUpgradesInt[type];
    public void CarlAddFlatRewardInt(CarlUpgradeTypesInt type, int amount) => carlUpgradesInt[type] += amount; 
    public void CarlMultiplyRewardInt(CarlUpgradeTypesInt type, int amount) => carlUpgradesInt[type] *= amount;
        // ----------float upgrades ---------//
    public float CarlGetRewardPowerFloat(CarlUpgradeTypesFloats type) => carlUpgradesFloat[type];
    public void CarlAddFlatRewardFloat(CarlUpgradeTypesFloats type, float amount) => carlUpgradesFloat[type] += amount; 
    public void CarlSubtractFlatRewardFloat(CarlUpgradeTypesFloats type, float amount) => carlUpgradesFloat[type] -= amount; 
    public void CarlMultiplyRewardFloat(CarlUpgradeTypesFloats type, float amount) => carlUpgradesFloat[type] *= amount;
// ---------------------------- Chloe ----------------------------//
    public AlphabeticNotation ChloeGetRewardPower(ChloeUpgradeTypes type) => chloeUpgrades[type];
    public void ChloeAddFlatReward(ChloeUpgradeTypes type, AlphabeticNotation amount) => chloeUpgrades[type] += amount; 
    public void ChloeMultiplyReward(ChloeUpgradeTypes type, AlphabeticNotation amount) => chloeUpgrades[type] *= amount;
        // ----------int upgrades ---------//
    public int ChloeGetRewardPowerInt(ChloeUpgradeTypesInt type) => chloeUpgradesInt[type];
    public void ChloeAddFlatRewardInt(ChloeUpgradeTypesInt type, int amount) => chloeUpgradesInt[type] += amount; 
    public void ChloeMultiplyRewardInt(ChloeUpgradeTypesInt type, int amount) => chloeUpgradesInt[type] *= amount;
        // ----------float upgrades ---------//
    public float ChloeGetRewardPowerFloat(ChloeUpgradeTypesFloats type) => chloeUpgradesFloat[type];
    public void ChloeAddFlatRewardFloat(ChloeUpgradeTypesFloats type, float amount) => chloeUpgradesFloat[type] += amount; 
    public void ChloeMultiplyRewardFloat(ChloeUpgradeTypesFloats type, float amount) => chloeUpgradesFloat[type] *= amount;
        // ----------bool upgrades ---------//
    public bool ChloeGetRewardPowerBool(ChloeUpgradeTypesBool type) => chloeUpgradesBool[type];
    public void ChloeRewardSetBool(ChloeUpgradeTypesBool type, bool state) => chloeUpgradesBool[type] = state; 
// ---------------------------- FRED ----------------------------//
    public AlphabeticNotation FredGetRewardPower(FredUpgradeTypes type) => fredUpgrades[type];
    public void FredAddFlatReward(FredUpgradeTypes type, AlphabeticNotation amount) => fredUpgrades[type] += amount; 
    public void FredSubtractFlatReward(FredUpgradeTypes type, AlphabeticNotation amount) => fredUpgrades[type] -= amount; 
    public void FredMultiplyReward(FredUpgradeTypes type, AlphabeticNotation amount) => fredUpgrades[type] *= amount;
        // ----------int upgrades ---------//
    public int FredGetRewardPowerInt(FredUpgradeTypesInt type) => fredUpgradesInt[type];
    public void FredAddFlatRewardInt(FredUpgradeTypesInt type, int amount) => fredUpgradesInt[type] += amount; 
    public void FredMultiplyRewardInt(FredUpgradeTypesInt type, int amount) => fredUpgradesInt[type] *= amount;
        // ----------float upgrades ---------//
    public float FredGetRewardPowerFloat(FredUpgradeTypesFloats type) => fredUpgradesFloat[type];
    public void FredAddFlatRewardFloat(FredUpgradeTypesFloats type, float amount) => fredUpgradesFloat[type] += amount; 
    public void FredSubtractFlatRewardFloat(FredUpgradeTypesFloats type, float amount) => fredUpgradesFloat[type] -= amount; 
    public void FredMultiplyRewardFloat(FredUpgradeTypesFloats type, float amount) => fredUpgradesFloat[type] *= amount;
// ---------------------------- SAM ----------------------------//
    public AlphabeticNotation SamGetRewardPower(SamUpgradeTypes type) => samUpgrades[type];
    public void SamAddFlatReward(SamUpgradeTypes type, AlphabeticNotation amount) => samUpgrades[type] += amount; 
    public void SamMultiplyReward(SamUpgradeTypes type, AlphabeticNotation amount) => samUpgrades[type] *= amount;
        // ----------int upgrades ---------//
    public int SamGetRewardPowerInt(SamUpgradeTypesInt type) => samUpgradesInt[type];
    public void SamAddFlatRewardInt(SamUpgradeTypesInt type, int amount) => samUpgradesInt[type] += amount; 
    public void SamMultiplyRewardInt(SamUpgradeTypesInt type, int amount) => samUpgradesInt[type] *= amount;
        // ----------float upgrades ---------//
    public float SamGetRewardPowerFloat(SamUpgradeTypesFloats type) => samUpgradesFloat[type];
    public void SamAddFlatRewardFloat(SamUpgradeTypesFloats type, float amount) => samUpgradesFloat[type] += amount; 
    public void SamMultiplyRewardFloat(SamUpgradeTypesFloats type, float amount) => samUpgradesFloat[type] *= amount;
// ---------------------------- ROGER ----------------------------//
    public AlphabeticNotation RogerGetRewardPower(RogerUpgradeTypes type) => rogerUpgrades[type];
    public void RogerAddFlatReward(RogerUpgradeTypes type, AlphabeticNotation amount) => rogerUpgrades[type] += amount; 
    public void RogerMultiplyReward(RogerUpgradeTypes type, AlphabeticNotation amount) => rogerUpgrades[type] *= amount;
        // ----------int upgrades ---------//
    public int RogerGetRewardPowerInt(RogerUpgradeTypesInt type) => rogerUpgradesInt[type];
    public void RogerAddFlatRewardInt(RogerUpgradeTypesInt type, int amount) => rogerUpgradesInt[type] += amount; 
    public void RogerMultiplyRewardInt(RogerUpgradeTypesInt type, int amount) => rogerUpgradesInt[type] *= amount;
        // ----------float upgrades ---------//
    public float RogerGetRewardPowerFloat(RogerUpgradeTypesFloats type) => rogerUpgradesFloat[type];
    public void RogerAddFlatRewardFloat(RogerUpgradeTypesFloats type, float amount) => rogerUpgradesFloat[type] += amount; 
    public void RogerMultiplyRewardFloat(RogerUpgradeTypesFloats type, float amount) => rogerUpgradesFloat[type] *= amount;
}
