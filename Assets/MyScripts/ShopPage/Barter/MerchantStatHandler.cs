using System.Collections.Generic;
using System.Runtime.CompilerServices;
using LargeNumbers;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MerchantStatHandler : MonoBehaviour
{
    [SerializeField] private BarterManager barterManager;

    [SerializeField] private TMP_Text availableSkillpoints_txt;
    [SerializeField] private TMP_Text level_txt;
    [SerializeField] private Merchants merchant;
    [SerializeField] private List<StatInfo> statBonuses;


    [System.Serializable]
    public class StatInfo
    {
        public CurrencyTypes type;
        public TMP_Text flatIncrease_txt;
        public TMP_Text currencyMulti_txt;
    }
    private void Start()
    {
        //UpdateUI(UpgradeID.RewardFlat,isWhatDatatype,merchant,CurrencyTypes.money); // initialize from somewhere else. bartermanager perhaps
        UpdateUIInit(); 
    }

    private void OnEnable()
    {
        barterManager.OnBarterLevelUp += UpdateLevel;
        barterManager.OnUpgradeBought += UpdateUI;
    }
    private void OnDisable()
    {
        barterManager.OnBarterLevelUp -= UpdateLevel;
        barterManager.OnUpgradeBought -= UpdateUI;
    }

    private void UpdateLevel(Merchants _merchants)
    {
        if (_merchants != merchant) return;
        availableSkillpoints_txt.text = barterManager.merchantInfos[merchant].skillPoints.ToString();
        level_txt.text = "Lv." + barterManager.merchantInfos[merchant].merchantLevel.ToString();
    }

private void UpdateUIInit()
{
    foreach (var stat in statBonuses)
    {
        var flat = MerchantUpgradeManager.Instance
                    .GetAlphabetic(UpgradeID.RewardFlat, merchant, stat.type);

        var multi = MerchantUpgradeManager.Instance
                    .GetFloat(UpgradeID.RewardMulti, merchant, stat.type);

        stat.flatIncrease_txt.text = "+" + flat.ToStringSmart(1);
        stat.currencyMulti_txt.text = ((multi - 1) * 100f).ToString("F0") + "%";
    }

    level_txt.text = "Lv." + barterManager.merchantInfos[merchant].merchantLevel;
    availableSkillpoints_txt.text = barterManager.merchantInfos[merchant].skillPoints.ToString();
}
    private void UpdateUI(UpgradeID upgradeID, IsWhatDatatype isWhatDatatype, Merchants _merchants, CurrencyTypes type)
    {
        if (_merchants != merchant) return;

                foreach (StatInfo stat in statBonuses)
                {
                    // print($"stat type {stat.type} id = {UpgradeID.RewardMulti} merchant = {merchant} amount = {(MerchantUpgradeManager.Instance.GetFloat(UpgradeID.RewardMulti, merchant, stat.type))}");
                    stat.flatIncrease_txt.text = "+" + MerchantUpgradeManager.Instance.GetAlphabetic(UpgradeID.RewardFlat, merchant, stat.type).ToStringSmart(1);
                    stat.currencyMulti_txt.text = ((MerchantUpgradeManager.Instance.GetFloat(UpgradeID.RewardMulti, merchant, stat.type) - 1) * 100).ToString("F0") + "%"; // COMMENTED OUT FOR WORKING ON UNIFIED
                }
        }

    }



        // switch (isWhatDatatype)
        // {
            // case IsWhatDatatype.isInt:
            //     foreach (StatInfo stat in statBonuses)
            //     {
            //         stat.flatIncrease_txt.text = "+" + MerchantUpgradeManager.Instance.GetInt(upgradeID, merchant, stat.type).ToString("F0");
            //         // stat.currencyMulti_txt.text = ((MerchantUpgradeManager.Instance.GetInt(upgradeID, merchant, stat.type) - 1) * 100).ToString("F0") + "%"; // COMMENTED OUT FOR WORKING ON UNIFIED
            //         break;
            //     }
            //     break;
            // case IsWhatDatatype.isFloatDatatype:
            //     foreach (StatInfo stat in statBonuses)
            //     {
            //         stat.flatIncrease_txt.text = "+" + MerchantUpgradeManager.Instance.GetFloat(upgradeID, merchant, stat.type).ToString("F0");
            //         // stat.currencyMulti_txt.text = ((MerchantUpgradeManager.Instance.GetFloat(upgradeID, merchant, stat.type) - 1) * 100).ToString("F0") + "%"; // COMMENTED OUT FOR WORKING ON UNIFIED
            //         break;
            //     }
            //     break;
            // case IsWhatDatatype.isAlphabeticnotationDatatype:

//  stat.currencyMulti_txt.text = ((MerchantUpgradeManager.Instance.AnyGetRewardPowerFloat() -1)*100).ToStringSmart(1) + "%";

// if(barterManager.merchantBonuses[merchant].rewardMultiplierBonus[stat.type] < 1){ // REMOVED WHEN WORKING ON UNIFIED
//     stat.currencyMulti_txt.color = new Color(1f,0f,0f,1f);
// }else{
//     stat.currencyMulti_txt.color = new Color(0f,1f,0f,1f);
// }
// myText.text = $"testing my new text with bonus - {barterManager.merchantBonuses[merchant].rewardBaseFlatIncrease[CurrencyTypes.wheat]}";
