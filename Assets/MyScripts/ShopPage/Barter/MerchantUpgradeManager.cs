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
}
public enum BobUpgradeTypesFloats{
    increaseAllXpBonusMulti,
    xpGainBonusMulti,
    increaseAllSpecialBarterChance,
    chanceForSpecialBarter,

}
public enum CarlUpgradeTypes{
    rewardMultiCarl,
    rewardFlatCarl,
}
public enum CarlUpgradeTypesFloats{
    xpGainBonusMulti,
    chanceForSpecialBarter,

}

public enum ChloeUpgradeTypes{
    rewardMultiChloe,
    rewardFlatChloe,
}
public enum ChloeUpgradeTypesFloats{
    xpGainBonusMulti,
    chanceForSpecialBarter,

}
public enum FredUpgradeTypes{
    rewardMultiFred,
    rewardFlatFred,
}
public enum FredUpgradeTypesFloats{
    xpGainBonusMulti,
    chanceForSpecialBarter,

}
public enum SamUpgradeTypes{
    rewardMultiSam,
    rewardFlatSam,
}
public enum SamUpgradeTypesFloats{
    xpGainBonusMulti,
    chanceForSpecialBarter,

}
public enum RogerUpgradeTypes{
    rewardMultiRoger,
    rewardFlatRoger,
    
}
public enum RogerUpgradeTypesFloats{
    xpGainBonusMulti,

    chanceForSpecialBarter,
}


public class MerchantUpgradeManager : MonoBehaviour
{
    public static MerchantUpgradeManager Instance {get;private set;}

    // ---------------------------- BOB ----------------------------//
    [SerializeField] public Dictionary<BobUpgradeTypes, AlphabeticNotation> bobUpgrades;
    [SerializeField] public AlphabeticNotation bobStartValues = new AlphabeticNotation(0); // MIGHT NEED TO REDO, SOME VALUES SHOULDNT BE 1 AT START
    [SerializeField] public AlphabeticNotation bobMultiStartValues = new AlphabeticNotation(1); // MIGHT NEED TO REDO, SOME VALUES SHOULDNT BE 1 AT START
    [SerializeField] public float bobFloatStartValues = 1f; 
    [SerializeField] public Dictionary<BobUpgradeTypesInt, int> bobUpgradesInt;
    [SerializeField] public Dictionary<BobUpgradeTypesFloats, float> bobUpgradesFloat;
    [SerializeField] public int bobStartValuesInts = 0;
    // ---------------------------- CARL ----------------------------//
    [SerializeField] public Dictionary <CarlUpgradeTypes, AlphabeticNotation> carlUpgrades;
    [SerializeField] public AlphabeticNotation carlStartValues = new AlphabeticNotation(1);
    [SerializeField] public AlphabeticNotation carlMultiStartValues = new AlphabeticNotation(1); // MIGHT NEED TO REDO, SOME VALUES SHOULDNT BE 1 AT START
    [SerializeField] public float carlFloatStartValues = 1f; 
    [SerializeField] public Dictionary<CarlUpgradeTypesFloats, float> carlUpgradesFloat;

    // ---------------------------- CHLOE ----------------------------//
    [SerializeField] public Dictionary <ChloeUpgradeTypes, AlphabeticNotation> chloeUpgrades;
    [SerializeField] public AlphabeticNotation chloeStartValues = new AlphabeticNotation(1);
    [SerializeField] public AlphabeticNotation chloeMultiStartValues = new AlphabeticNotation(1); // MIGHT NEED TO REDO, SOME VALUES SHOULDNT BE 1 AT START
    [SerializeField] public float chloeFloatStartValues = 1f; 
    [SerializeField] public Dictionary<ChloeUpgradeTypesFloats, float> chloeUpgradesFloat;
    // ---------------------------- FRED ----------------------------//
    [SerializeField] public Dictionary <FredUpgradeTypes, AlphabeticNotation> fredUpgrades;
    [SerializeField] public AlphabeticNotation fredStartValues = new AlphabeticNotation(1);
    [SerializeField] public AlphabeticNotation fredMultiStartValues = new AlphabeticNotation(1); // MIGHT NEED TO REDO, SOME VALUES SHOULDNT BE 1 AT START
    [SerializeField] public float fredFloatStartValues = 1f; 
    [SerializeField] public Dictionary<FredUpgradeTypesFloats, float> fredUpgradesFloat;
    // ---------------------------- SAM ----------------------------//
    [SerializeField] public Dictionary <SamUpgradeTypes, AlphabeticNotation> samUpgrades;
    [SerializeField] public AlphabeticNotation samStartValues = new AlphabeticNotation(1);
    [SerializeField] public AlphabeticNotation samMultiStartValues = new AlphabeticNotation(1); // MIGHT NEED TO REDO, SOME VALUES SHOULDNT BE 1 AT START
    [SerializeField] public float samFloatStartValues = 1f; 
    [SerializeField] public Dictionary<SamUpgradeTypesFloats, float> samUpgradesFloat;
    // ---------------------------- ROGER ----------------------------//
    [SerializeField] public Dictionary <RogerUpgradeTypes, AlphabeticNotation>rogerUpgrades;
    [SerializeField] public AlphabeticNotation rogerStartvalues = new AlphabeticNotation(1);
    [SerializeField] public AlphabeticNotation rogerMultiStartValues = new AlphabeticNotation(1); // MIGHT NEED TO REDO, SOME VALUES SHOULDNT BE 1 AT START
    [SerializeField] public float rogerFloatStartValues = 1f; 
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
        ChloeUpgradeTypes.rewardMultiChloe => new AlphabeticNotation(1),
        ChloeUpgradeTypes.rewardFlatChloe => new AlphabeticNotation(0),
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
        BobUpgradeTypesInt.moneyWeightChanceBob => 0,
        _ => 0
    };
}

//------------------------------------------------ FLOAT DATATYPE ------------------------------------------ //
private float GetDefaultValueForBob(BobUpgradeTypesFloats type)
{
    return type switch
    {
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
        _ => 1f
    };
}

private float GetDefaultValueForChloe(ChloeUpgradeTypesFloats type)
{
    return type switch
    {
        ChloeUpgradeTypesFloats.xpGainBonusMulti => 1f,
        ChloeUpgradeTypesFloats.chanceForSpecialBarter => 0f,
        _ => 1f
    };
}

private float GetDefaultValueForFred(FredUpgradeTypesFloats type)
{
    return type switch
    {
        FredUpgradeTypesFloats.xpGainBonusMulti => 1f,
        FredUpgradeTypesFloats.chanceForSpecialBarter => 0f,
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
private void InitializeMerchantUpgrades()
{
    // --- Init dictionaries ---
    bobUpgrades = new Dictionary<BobUpgradeTypes, AlphabeticNotation>();
    bobUpgradesInt = new Dictionary<BobUpgradeTypesInt, int>();
    bobUpgradesFloat = new Dictionary<BobUpgradeTypesFloats, float>();

    carlUpgrades = new Dictionary<CarlUpgradeTypes, AlphabeticNotation>();
    carlUpgradesFloat = new Dictionary<CarlUpgradeTypesFloats, float>();

    chloeUpgrades = new Dictionary<ChloeUpgradeTypes, AlphabeticNotation>();
    chloeUpgradesFloat = new Dictionary<ChloeUpgradeTypesFloats, float>();

    fredUpgrades = new Dictionary<FredUpgradeTypes, AlphabeticNotation>();
    fredUpgradesFloat = new Dictionary<FredUpgradeTypesFloats, float>();

    samUpgrades = new Dictionary<SamUpgradeTypes, AlphabeticNotation>();
    samUpgradesFloat = new Dictionary<SamUpgradeTypesFloats, float>();

    rogerUpgrades = new Dictionary<RogerUpgradeTypes, AlphabeticNotation>();
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
    foreach (CarlUpgradeTypesFloats type in Enum.GetValues(typeof(CarlUpgradeTypesFloats)))
        carlUpgradesFloat[type] = GetDefaultValueForCarl(type);

    // --- CHLOE ---
    foreach (ChloeUpgradeTypes type in Enum.GetValues(typeof(ChloeUpgradeTypes)))
        chloeUpgrades[type] = GetDefaultValueForChloe(type);
    foreach (ChloeUpgradeTypesFloats type in Enum.GetValues(typeof(ChloeUpgradeTypesFloats)))
        chloeUpgradesFloat[type] = GetDefaultValueForChloe(type);

    // --- FRED ---
    foreach (FredUpgradeTypes type in Enum.GetValues(typeof(FredUpgradeTypes)))
        fredUpgrades[type] = GetDefaultValueForFred(type);
    foreach (FredUpgradeTypesFloats type in Enum.GetValues(typeof(FredUpgradeTypesFloats)))
        fredUpgradesFloat[type] = GetDefaultValueForFred(type);

    // --- SAM ---
    foreach (SamUpgradeTypes type in Enum.GetValues(typeof(SamUpgradeTypes)))
        samUpgrades[type] = GetDefaultValueForSam(type);
    foreach (SamUpgradeTypesFloats type in Enum.GetValues(typeof(SamUpgradeTypesFloats)))
        samUpgradesFloat[type] = GetDefaultValueForSam(type);

    // --- ROGER ---
    foreach (RogerUpgradeTypes type in Enum.GetValues(typeof(RogerUpgradeTypes)))
        rogerUpgrades[type] = GetDefaultValueForRoger(type);
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
        // ----------float upgrades ---------//
    public float CarlGetRewardPowerFloat(CarlUpgradeTypesFloats type) => carlUpgradesFloat[type];
    public void CarlAddFlatRewardFloat(CarlUpgradeTypesFloats type, float amount) => carlUpgradesFloat[type] += amount; 
    public void CarlMultiplyRewardFloat(CarlUpgradeTypesFloats type, float amount) => carlUpgradesFloat[type] *= amount;
// ---------------------------- Chloe ----------------------------//
    public AlphabeticNotation ChloeGetRewardPower(ChloeUpgradeTypes type) => chloeUpgrades[type];
    public void ChloeAddFlatReward(ChloeUpgradeTypes type, AlphabeticNotation amount) => chloeUpgrades[type] += amount; 
    public void ChloeMultiplyReward(ChloeUpgradeTypes type, AlphabeticNotation amount) => chloeUpgrades[type] *= amount;
        // ----------float upgrades ---------//
    public float ChloeGetRewardPowerFloat(ChloeUpgradeTypesFloats type) => chloeUpgradesFloat[type];
    public void ChloeAddFlatRewardFloat(ChloeUpgradeTypesFloats type, float amount) => chloeUpgradesFloat[type] += amount; 
    public void ChloeMultiplyRewardFloat(ChloeUpgradeTypesFloats type, float amount) => chloeUpgradesFloat[type] *= amount;
// ---------------------------- FRED ----------------------------//
    public AlphabeticNotation FredGetRewardPower(FredUpgradeTypes type) => fredUpgrades[type];
    public void FredAddFlatReward(FredUpgradeTypes type, AlphabeticNotation amount) => fredUpgrades[type] += amount; 
    public void FredMultiplyReward(FredUpgradeTypes type, AlphabeticNotation amount) => fredUpgrades[type] *= amount;
        // ----------float upgrades ---------//
    public float FredGetRewardPowerFloat(FredUpgradeTypesFloats type) => fredUpgradesFloat[type];
    public void FredAddFlatRewardFloat(FredUpgradeTypesFloats type, float amount) => fredUpgradesFloat[type] += amount; 
    public void FredMultiplyRewardFloat(FredUpgradeTypesFloats type, float amount) => fredUpgradesFloat[type] *= amount;
// ---------------------------- SAM ----------------------------//
    public AlphabeticNotation SamGetRewardPower(SamUpgradeTypes type) => samUpgrades[type];
    public void SamAddFlatReward(SamUpgradeTypes type, AlphabeticNotation amount) => samUpgrades[type] += amount; 
    public void SamMultiplyReward(SamUpgradeTypes type, AlphabeticNotation amount) => samUpgrades[type] *= amount;
        // ----------float upgrades ---------//
    public float SamGetRewardPowerFloat(SamUpgradeTypesFloats type) => samUpgradesFloat[type];
    public void SamAddFlatRewardFloat(SamUpgradeTypesFloats type, float amount) => samUpgradesFloat[type] += amount; 
    public void SamMultiplyRewardFloat(SamUpgradeTypesFloats type, float amount) => samUpgradesFloat[type] *= amount;
// ---------------------------- ROGER ----------------------------//
    public AlphabeticNotation RogerGetRewardPower(RogerUpgradeTypes type) => rogerUpgrades[type];
    public void RogerAddFlatReward(RogerUpgradeTypes type, AlphabeticNotation amount) => rogerUpgrades[type] += amount; 
    public void RogerMultiplyReward(RogerUpgradeTypes type, AlphabeticNotation amount) => rogerUpgrades[type] *= amount;
        // ----------float upgrades ---------//
    public float RogerGetRewardPowerFloat(RogerUpgradeTypesFloats type) => rogerUpgradesFloat[type];
    public void RogerAddFlatRewardFloat(RogerUpgradeTypesFloats type, float amount) => rogerUpgradesFloat[type] += amount; 
    public void RogerMultiplyRewardFloat(RogerUpgradeTypesFloats type, float amount) => rogerUpgradesFloat[type] *= amount;
}
