using UnityEngine;
using LargeNumbers;
using System;
using UnityEngine.Assertions.Must;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Rendering;
public enum UpgradeTypes
{
    unlockArea,
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
public interface IUpgradeEffect
{
    void Apply(GameObject target = null);
}

[CreateAssetMenu(fileName = "Upgrades", menuName = "Upgrade/UpgradeEffect Data")]
public class UpgradeEffect : ScriptableObject
{
    public List<UpgradeTypeInfoGlobal> upgradeTypeInfoGlobals;
    [System.Serializable]
    public class UpgradeTypeInfoGlobal
    {
        public List<UpgradeTypes> upgradeTypes;
        public List<CurrencyTypes> currencyTypes;
        public UpgradeIDGlobal upgradeIDGlobal;
        public AlphabeticNotation flat_alpha;
        public int flat_intUpgrades;
        public float flat_floatUpgrades;
        public bool bool_state;
    }

    public void Apply(GameObject target = null)
    {
        
        foreach (var info in upgradeTypeInfoGlobals)
        {
        Debug.Log($"ID = {info.upgradeIDGlobal}");
            foreach (UpgradeTypes upgradeTypes in info.upgradeTypes)
            {

                switch (upgradeTypes)
                {
                    case UpgradeTypes.unlockArea:
                    target?.SetActive(true);
                    break;
                    // -------- UPGRADES-------- //
                    case UpgradeTypes.addRewardAlpha:
                        foreach (CurrencyTypes type in info.currencyTypes)
                        {
                            UpgradeManager.Instance.Modify(info.upgradeIDGlobal, UpgradeOperation.Add, type, info.flat_alpha);
                        }
                        break;
                    case UpgradeTypes.subRewardAlpha:
                        foreach (CurrencyTypes type in info.currencyTypes)
                        {
                            UpgradeManager.Instance.Modify(info.upgradeIDGlobal, UpgradeOperation.Subtract, type, info.flat_alpha);
                        }
                        break;
                    case UpgradeTypes.setRewardAlpha:
                        foreach (CurrencyTypes type in info.currencyTypes)
                        {
                            UpgradeManager.Instance.Modify(info.upgradeIDGlobal, UpgradeOperation.Set, type, info.flat_alpha);
                        }
                        break;
                    case UpgradeTypes.addRewardInt:
                        foreach(CurrencyTypes type in info.currencyTypes){
                            UpgradeManager.Instance.Modify(info.upgradeIDGlobal, UpgradeOperation.Add,type,info.flat_intUpgrades);
                        } 
                            break;
                    case UpgradeTypes.subRewardInt:
                        foreach(CurrencyTypes type in info.currencyTypes){
                            UpgradeManager.Instance.Modify(info.upgradeIDGlobal, UpgradeOperation.Subtract,type,info.flat_intUpgrades);
                        } 
                            break;
                    case UpgradeTypes.setRewardInt:
                        foreach(CurrencyTypes type in info.currencyTypes){
                            UpgradeManager.Instance.Modify(info.upgradeIDGlobal, UpgradeOperation.Set,type,info.flat_intUpgrades);
                        } 
                            break;
                    case UpgradeTypes.addRewardFloat:
                        foreach(CurrencyTypes type in info.currencyTypes){
                            UpgradeManager.Instance.Modify(info.upgradeIDGlobal, UpgradeOperation.Add,type,info.flat_floatUpgrades);
                        } 
                            break;
                    case UpgradeTypes.subRewardFloat:
                        foreach(CurrencyTypes type in info.currencyTypes){
                            UpgradeManager.Instance.Modify(info.upgradeIDGlobal, UpgradeOperation.Subtract,type,info.flat_floatUpgrades);
                        } 
                            break;
                    case UpgradeTypes.setRewardFloat:
                        foreach(CurrencyTypes type in info.currencyTypes){
                            UpgradeManager.Instance.Modify(info.upgradeIDGlobal, UpgradeOperation.Set,type,info.flat_floatUpgrades);
                        } 
                            break;
                    case UpgradeTypes.setRewardBool:
                        foreach(CurrencyTypes type in info.currencyTypes){
                            UpgradeManager.Instance.Modify(info.upgradeIDGlobal, UpgradeOperation.Set,type,info.bool_state);
                        } 
                            break;
                
                }
                
            }
        }
    }

}
