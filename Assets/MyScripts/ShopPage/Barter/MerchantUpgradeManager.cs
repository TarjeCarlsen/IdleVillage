using System;
using System.Collections.Generic;
using LargeNumbers;
using UnityEngine;



public enum BobUpgradeTypes{
    rewardMultiBob,
    rewardFlatBob,
    rewardFlatBob_2,
}
public enum BobUpgradeTypesInt{
    moneyWeightChanceBob,
}
public enum BobUpgradeTypesFloats{
    xpGainBonusMulti,

}
public enum CarlUpgradeTypes{
    rewardMultiCarl,
    rewardFlatCarl,
}
public enum CarlUpgradeTypesFloats{
    xpGainBonusMulti,

}

public enum ChloeUpgradeTypes{
    rewardMultiChloe,
    rewardFlatChloe,
}
public enum ChloeUpgradeTypesFloats{
    xpGainBonusMulti,

}
public enum FredUpgradeTypes{
    rewardMultiFred,
    rewardFlatFred,
}
public enum FredUpgradeTypesFloats{
    xpGainBonusMulti,

}
public enum SamUpgradeTypes{
    rewardMultiSam,
    rewardFlatSam,
}
public enum SamUpgradeTypesFloats{
    xpGainBonusMulti,

}
public enum RogerUpgradeTypes{
    rewardMultiRoger,
    rewardFlatRoger,
    
}
public enum RogerUpgradeTypesFloats{
    xpGainBonusMulti,

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

    private void InitializeMerchantUpgrades(){
         bobUpgrades = new Dictionary<BobUpgradeTypes, AlphabeticNotation>();
         bobUpgradesInt = new Dictionary<BobUpgradeTypesInt, int>();
         bobUpgradesFloat = new Dictionary<BobUpgradeTypesFloats, float>();
        chloeUpgrades = new Dictionary<ChloeUpgradeTypes, AlphabeticNotation>();
         chloeUpgradesFloat = new Dictionary<ChloeUpgradeTypesFloats, float>();
        carlUpgrades = new Dictionary<CarlUpgradeTypes, AlphabeticNotation>();
         carlUpgradesFloat = new Dictionary<CarlUpgradeTypesFloats, float>();
        fredUpgrades = new Dictionary<FredUpgradeTypes, AlphabeticNotation>();
         fredUpgradesFloat = new Dictionary<FredUpgradeTypesFloats, float>();
        samUpgrades = new Dictionary<SamUpgradeTypes, AlphabeticNotation>();
         samUpgradesFloat = new Dictionary<SamUpgradeTypesFloats, float>();
        rogerUpgrades = new Dictionary<RogerUpgradeTypes, AlphabeticNotation>();
         rogerUpgradesFloat = new Dictionary<RogerUpgradeTypesFloats, float>();

// -------------------------------------- BOB ----------------------------------------- //
        foreach(BobUpgradeTypes upgradeTypes in Enum.GetValues(typeof(BobUpgradeTypes))){
            string name = upgradeTypes.ToString().ToLower();
            bobUpgrades[upgradeTypes] = name.Contains("multi") ? bobMultiStartValues : bobStartValues;
        }
        foreach(BobUpgradeTypesInt upgradeTypes in Enum.GetValues(typeof(BobUpgradeTypesInt))){
            bobUpgradesInt[upgradeTypes] = bobStartValuesInts;
        }
        foreach(BobUpgradeTypesFloats upgradeTypes in Enum.GetValues(typeof(BobUpgradeTypesFloats))){
            bobUpgradesFloat[upgradeTypes] = bobFloatStartValues; 
        }

// -------------------------------------- CARL ----------------------------------------- //
        foreach(CarlUpgradeTypes upgradeTypes in Enum.GetValues(typeof(CarlUpgradeTypes))){
            string name = upgradeTypes.ToString().ToLower();
            carlUpgrades[upgradeTypes] = name.Contains("multi") ? carlMultiStartValues : carlStartValues;
        }
        foreach(CarlUpgradeTypesFloats upgradeTypes in Enum.GetValues(typeof(CarlUpgradeTypesFloats))){
            carlUpgradesFloat[upgradeTypes] = carlFloatStartValues; 
        }
// -------------------------------------- CHLOE ----------------------------------------- //
        foreach(ChloeUpgradeTypes upgradeTypes in Enum.GetValues(typeof(ChloeUpgradeTypes))){
            string name = upgradeTypes.ToString().ToLower();
            chloeUpgrades[upgradeTypes] = name.Contains("multi") ? chloeMultiStartValues : chloeStartValues;
        }
        foreach(ChloeUpgradeTypesFloats upgradeTypes in Enum.GetValues(typeof(ChloeUpgradeTypesFloats))){
            chloeUpgradesFloat[upgradeTypes] = chloeFloatStartValues; 
        }
// -------------------------------------- FRED ----------------------------------------- //
        foreach(FredUpgradeTypes upgradeTypes in Enum.GetValues(typeof(FredUpgradeTypes))){
            string name = upgradeTypes.ToString().ToLower();
            fredUpgrades[upgradeTypes] = name.Contains("multi") ? fredMultiStartValues : fredStartValues;
        }
        foreach(FredUpgradeTypesFloats upgradeTypes in Enum.GetValues(typeof(FredUpgradeTypesFloats))){
            fredUpgradesFloat[upgradeTypes] = fredFloatStartValues; 
        }
// -------------------------------------- SAM ----------------------------------------- //
        foreach(SamUpgradeTypes upgradeTypes in Enum.GetValues(typeof(SamUpgradeTypes))){
            string name = upgradeTypes.ToString().ToLower();
            samUpgrades[upgradeTypes] = name.Contains("multi") ? samMultiStartValues : samStartValues;
        }
        foreach(SamUpgradeTypesFloats upgradeTypes in Enum.GetValues(typeof(SamUpgradeTypesFloats))){
            samUpgradesFloat[upgradeTypes] = samFloatStartValues; 
        }
// -------------------------------------- ROGER ----------------------------------------- //
        foreach(RogerUpgradeTypes upgradeTypes in Enum.GetValues(typeof(RogerUpgradeTypes))){
            string name = upgradeTypes.ToString().ToLower();
            rogerUpgrades[upgradeTypes] = name.Contains("multi") ? rogerMultiStartValues : rogerStartvalues;
        }
        foreach(RogerUpgradeTypesFloats upgradeTypes in Enum.GetValues(typeof(RogerUpgradeTypesFloats))){
            rogerUpgradesFloat[upgradeTypes] = rogerFloatStartValues; 
        }
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
