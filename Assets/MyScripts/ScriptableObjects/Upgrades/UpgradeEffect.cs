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
    addAlpha,
    addFloat,
    addInt,
    subAlpha,
    subFloat,
    subInt,
    setAlpha,
    setFloat,
    setInt,
    setBool,
    

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
            foreach (UpgradeTypes upgradeTypes in info.upgradeTypes)
            {

                switch (upgradeTypes)
                {
                    case UpgradeTypes.unlockArea:
                    target?.SetActive(true);
                    break;
                    // -------- UPGRADES-------- //
                    case UpgradeTypes.addAlpha:
                        foreach (CurrencyTypes type in info.currencyTypes)
                        {
                            UpgradeManager.Instance.Modify(info.upgradeIDGlobal, UpgradeOperation.Add, type, info.flat_alpha);
                        }
                        break;
                    case UpgradeTypes.subAlpha:
                        foreach (CurrencyTypes type in info.currencyTypes)
                        {
                            UpgradeManager.Instance.Modify(info.upgradeIDGlobal, UpgradeOperation.Subtract, type, info.flat_alpha);
                        }
                        break;
                    case UpgradeTypes.setAlpha:
                        foreach (CurrencyTypes type in info.currencyTypes)
                        {
                            UpgradeManager.Instance.Modify(info.upgradeIDGlobal, UpgradeOperation.Set, type, info.flat_alpha);
                        }
                        break;
                    case UpgradeTypes.addInt:
                        foreach(CurrencyTypes type in info.currencyTypes){
                            UpgradeManager.Instance.Modify(info.upgradeIDGlobal, UpgradeOperation.Add,type,info.flat_intUpgrades);
                        } 
                            break;
                    case UpgradeTypes.subInt:
                        foreach(CurrencyTypes type in info.currencyTypes){
                            UpgradeManager.Instance.Modify(info.upgradeIDGlobal, UpgradeOperation.Subtract,type,info.flat_intUpgrades);
                        } 
                            break;
                    case UpgradeTypes.setInt:
                        foreach(CurrencyTypes type in info.currencyTypes){
                            UpgradeManager.Instance.Modify(info.upgradeIDGlobal, UpgradeOperation.Set,type,info.flat_intUpgrades);
                        } 
                            break;
                    case UpgradeTypes.addFloat:
                        foreach(CurrencyTypes type in info.currencyTypes){
                            UpgradeManager.Instance.Modify(info.upgradeIDGlobal, UpgradeOperation.Add,type,info.flat_floatUpgrades);
                        } 
                            break;
                    case UpgradeTypes.subFloat:
                        foreach(CurrencyTypes type in info.currencyTypes){
                            UpgradeManager.Instance.Modify(info.upgradeIDGlobal, UpgradeOperation.Subtract,type,info.flat_floatUpgrades);
                        } 
                            break;
                    case UpgradeTypes.setFloat:
                        foreach(CurrencyTypes type in info.currencyTypes){
                            UpgradeManager.Instance.Modify(info.upgradeIDGlobal, UpgradeOperation.Set,type,info.flat_floatUpgrades);
                        } 
                            break;
                    case UpgradeTypes.setBool:
                        foreach(CurrencyTypes type in info.currencyTypes){
                            UpgradeManager.Instance.Modify(info.upgradeIDGlobal, UpgradeOperation.Set,type,info.bool_state);
                        } 
                            break;
                
                }
                
            }
        }
    }

}
