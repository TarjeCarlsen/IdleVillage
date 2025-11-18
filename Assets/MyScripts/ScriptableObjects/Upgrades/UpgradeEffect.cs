using UnityEngine;
using LargeNumbers;
using System;
using UnityEngine.Assertions.Must;
public enum UpgradeTypes{
    UnlockArea,
    SetActivationState,
    AddPowerFlat,
    AddPowerMulti,

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
        }
    }
    
}
