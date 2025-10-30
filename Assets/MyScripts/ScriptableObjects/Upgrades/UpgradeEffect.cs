using UnityEngine;
using LargeNumbers;
public enum UpgradeTypes{
    UnlockArea,
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
    public UpgradeTypes upgradeTypes;
    public CurrencyTypes currencyTypes;
    public AlphabeticNotation flat;
    public AlphabeticNotation multi;

    public void Apply(GameObject target = null){


        switch(upgradeTypes){
            // -------- AREA UNLOCKS -------- //
            case UpgradeTypes.UnlockArea:
                target?.SetActive(true);
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
