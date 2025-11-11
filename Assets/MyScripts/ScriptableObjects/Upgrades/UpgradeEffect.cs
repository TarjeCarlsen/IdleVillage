using UnityEngine;
using LargeNumbers;
public enum UpgradeTypes{
    UnlockArea,
    SetActivationState,
    AddPowerFlat,
    AddPowerMulti,
    AddMerchantPower,
    AddMerchantMulti,
}

public interface IUpgradeEffect
{
    void Apply(GameObject target = null);
}

[CreateAssetMenu(fileName = "Upgrades", menuName = "Upgrade/UpgradeEffect Data")]
public class UpgradeEffect : ScriptableObject
{
    public UpgradeTypes upgradeTypes;
    public CurrencyTypes currencyTypes;
    public ActivationUnlocks activationUnlocks;
    public AlphabeticNotation flat;
    public AlphabeticNotation multi;
    public bool activationState;
    [Header("Spesific upgrades for the merchants at barter trades. Ignore this if not for merchant")]
    public Merchants merchants;
    public MerchantUpgradeTypes merchantUpgradeTypes;
    public BobUpgradeTypes bobUpgradeTypes;


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
            case UpgradeTypes.AddMerchantPower:
                MerchantUpgradeManager.Instance.BobAddFlatReward(bobUpgradeTypes, flat);
                break;
            case UpgradeTypes.AddMerchantMulti: // adding more to the multiplier for bobs rewards
                MerchantUpgradeManager.Instance.BobAddFlatReward(bobUpgradeTypes, flat);
                break;
        }
    }
    
}
