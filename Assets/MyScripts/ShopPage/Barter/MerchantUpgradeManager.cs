using System;
using System.Collections.Generic;
using LargeNumbers;
using UnityEngine;



public enum BobUpgradeTypes{
    rewardMultiBob,
    rewardFlatBob,
}
public enum CarlUpgradeTypes{
    rewardMultiCarl,
    rewardFlatCarl,
}
public enum ChloeUpgradeTypes{
    rewardMultiChloe,
    rewardFlatChloe,
}
public enum FredUpgradeTypes{
    rewardMultiFred,
    rewardFlatFred,
}
public enum SamUpgradeTypes{
    rewardMultiSam,
    rewardFlatSam,
}
public enum RogerUpgradeTypes{
    rewardMultiRoger,
    rewardFlatRoger,
}


public class MerchantUpgradeManager : MonoBehaviour
{
    public static MerchantUpgradeManager Instance {get;private set;}

    // ---------------------------- BOB ----------------------------//
    [SerializeField] public Dictionary<BobUpgradeTypes, AlphabeticNotation> bobUpgrades;
    [SerializeField] public AlphabeticNotation bobStartValues = new AlphabeticNotation(1); // MIGHT NEED TO REDO, SOME VALUES SHOULDNT BE 1 AT START
    // ---------------------------- CARL ----------------------------//
    [SerializeField] public Dictionary <CarlUpgradeTypes, AlphabeticNotation> carlUpgrades;
    [SerializeField] public AlphabeticNotation carlStartValues = new AlphabeticNotation(1);
    // ---------------------------- CHLOE ----------------------------//
    [SerializeField] public Dictionary <ChloeUpgradeTypes, AlphabeticNotation> chloeUpgrades;
    [SerializeField] public AlphabeticNotation chloeStartValues = new AlphabeticNotation(1);
    // ---------------------------- FRED ----------------------------//
    [SerializeField] public Dictionary <FredUpgradeTypes, AlphabeticNotation> fredUpgrades;
    [SerializeField] public AlphabeticNotation fredStartValues = new AlphabeticNotation(1);
    // ---------------------------- SAM ----------------------------//
    [SerializeField] public Dictionary <SamUpgradeTypes, AlphabeticNotation> samUpgrades;
    [SerializeField] public AlphabeticNotation samStartValues = new AlphabeticNotation(1);
    // ---------------------------- ROGER ----------------------------//
    [SerializeField] public Dictionary <RogerUpgradeTypes, AlphabeticNotation>rogerUpgrades;
    [SerializeField] public AlphabeticNotation rogerStartvalues = new AlphabeticNotation(1);
    private void Awake(){
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        InitializeMerchantUpgrades();
    }

    private void InitializeMerchantUpgrades(){
         bobUpgrades = new Dictionary<BobUpgradeTypes, AlphabeticNotation>();
        chloeUpgrades = new Dictionary<ChloeUpgradeTypes, AlphabeticNotation>();
        carlUpgrades = new Dictionary<CarlUpgradeTypes, AlphabeticNotation>();
        fredUpgrades = new Dictionary<FredUpgradeTypes, AlphabeticNotation>();
        samUpgrades = new Dictionary<SamUpgradeTypes, AlphabeticNotation>();
        rogerUpgrades = new Dictionary<RogerUpgradeTypes, AlphabeticNotation>();


        foreach(BobUpgradeTypes upgradetype in Enum.GetValues(typeof(BobUpgradeTypes))){
            bobUpgrades[upgradetype] = bobStartValues;
        }
        foreach(CarlUpgradeTypes upgradeTypes in Enum.GetValues(typeof(CarlUpgradeTypes))){
            carlUpgrades[upgradeTypes] = carlStartValues;
        }
        foreach(ChloeUpgradeTypes upgradeTypes in Enum.GetValues(typeof(ChloeUpgradeTypes))){
            chloeUpgrades[upgradeTypes] = chloeStartValues;
        }
        foreach(FredUpgradeTypes upgradeTypes in Enum.GetValues(typeof(FredUpgradeTypes))){
            fredUpgrades[upgradeTypes] =fredStartValues;
        }
        foreach(SamUpgradeTypes upgradeTypes in Enum.GetValues(typeof(SamUpgradeTypes))){
            samUpgrades[upgradeTypes] =samStartValues;
        }
        foreach(RogerUpgradeTypes upgradeTypes in Enum.GetValues(typeof(RogerUpgradeTypes))){
            rogerUpgrades[upgradeTypes] = rogerStartvalues;
        }
    }


// ---------------------------- BOB ----------------------------//
    public AlphabeticNotation BobGetRewardPower(BobUpgradeTypes type) => bobUpgrades[type];
    public void BobAddFlatReward(BobUpgradeTypes type, AlphabeticNotation amount) => bobUpgrades[type] += amount; 
    public void BobMultiplyReward(BobUpgradeTypes type, AlphabeticNotation amount) => bobUpgrades[type] *= amount;

// ---------------------------- CARL ----------------------------//
    public AlphabeticNotation CarlGetRewardPower(CarlUpgradeTypes type) => carlUpgrades[type];
    public void CarlAddFlatReward(CarlUpgradeTypes type, AlphabeticNotation amount) => carlUpgrades[type] += amount; 
    public void CarlMultiplyReward(CarlUpgradeTypes type, AlphabeticNotation amount) => carlUpgrades[type] *= amount;
// ---------------------------- Chloe ----------------------------//
    public AlphabeticNotation ChloeGetRewardPower(ChloeUpgradeTypes type) => chloeUpgrades[type];
    public void ChloeAddFlatReward(ChloeUpgradeTypes type, AlphabeticNotation amount) => chloeUpgrades[type] += amount; 
    public void ChloeMultiplyReward(ChloeUpgradeTypes type, AlphabeticNotation amount) => chloeUpgrades[type] *= amount;
// ---------------------------- FRED ----------------------------//
    public AlphabeticNotation FredGetRewardPower(FredUpgradeTypes type) => fredUpgrades[type];
    public void FredAddFlatReward(FredUpgradeTypes type, AlphabeticNotation amount) => fredUpgrades[type] += amount; 
    public void FredMultiplyReward(FredUpgradeTypes type, AlphabeticNotation amount) => fredUpgrades[type] *= amount;
// ---------------------------- SAM ----------------------------//
    public AlphabeticNotation SamGetRewardPower(SamUpgradeTypes type) => samUpgrades[type];
    public void SamAddFlatReward(SamUpgradeTypes type, AlphabeticNotation amount) => samUpgrades[type] += amount; 
    public void SamMultiplyReward(SamUpgradeTypes type, AlphabeticNotation amount) => samUpgrades[type] *= amount;
// ---------------------------- ROGER ----------------------------//
    public AlphabeticNotation RogerGetRewardPower(RogerUpgradeTypes type) => rogerUpgrades[type];
    public void RogerAddFlatReward(RogerUpgradeTypes type, AlphabeticNotation amount) => rogerUpgrades[type] += amount; 
    public void RogerMultiplyReward(RogerUpgradeTypes type, AlphabeticNotation amount) => rogerUpgrades[type] *= amount;
}
