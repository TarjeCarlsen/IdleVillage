using System;
using System.Collections.Generic;
using LargeNumbers;
using UnityEngine;



public enum BobUpgradeTypes{
    rewardMultiBob,
    rewardFlatBob,
}
public enum CarlUpgradeTypes{

}
public enum ChloeUpgradeTypes{
    
}
public enum FredUpgradeTypes{

}
public enum SamUpgradeTypes{

}
public enum RogerUpgradeTypes{

}


public class MerchantUpgradeManager : MonoBehaviour
{
    public static MerchantUpgradeManager Instance {get;private set;}
    [SerializeField] public Dictionary<BobUpgradeTypes, AlphabeticNotation> bobUpgrades;
    [SerializeField] public AlphabeticNotation startValues = new AlphabeticNotation(1); // MIGHT NEED TO REDO, SOME VALUES SHOULDNT BE 1 AT START
    private void Awake(){
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        InitializeBobUpgrades();
    }

    private void InitializeBobUpgrades(){
         bobUpgrades = new Dictionary<BobUpgradeTypes, AlphabeticNotation>();
        foreach(BobUpgradeTypes upgradetype in Enum.GetValues(typeof(BobUpgradeTypes))){
            bobUpgrades[upgradetype] = startValues;
        }
    }



    public AlphabeticNotation BobGetRewardPower(BobUpgradeTypes type) => bobUpgrades[type];
    public void BobAddFlatReward(BobUpgradeTypes type, AlphabeticNotation amount) => bobUpgrades[type] += amount; 
    public void BobMultiplyReward(BobUpgradeTypes type, AlphabeticNotation amount) => bobUpgrades[type] *= amount;
}
