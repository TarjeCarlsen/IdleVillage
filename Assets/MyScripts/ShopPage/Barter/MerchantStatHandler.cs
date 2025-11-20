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
    public class StatInfo{
        public CurrencyTypes type;
        public  TMP_Text  flatIncrease_txt;
        public  TMP_Text  currencyMulti_txt;
    }
    private void Start(){
        UpdateUI(merchant,CurrencyTypes.money);
        
    }

    private void OnEnable(){
        barterManager.OnBarterLevelUp += UpdateLevel;
        barterManager.OnUpgradeBought += UpdateUI;
    }
    private void OnDisable(){
        barterManager.OnBarterLevelUp -= UpdateLevel;
        barterManager.OnUpgradeBought -= UpdateUI;
    }

    private void UpdateLevel(Merchants _merchants){
        if(_merchants != merchant) return;
            availableSkillpoints_txt.text = barterManager.merchantInfos[merchant].skillPoints.ToString();
            level_txt.text ="Lv."+ barterManager.merchantInfos[merchant].merchantLevel.ToString();
    }

    private void UpdateUI(Merchants _merchants, CurrencyTypes type){
        if(_merchants != merchant) return;
            foreach(StatInfo stat in statBonuses){
                stat.flatIncrease_txt.text = "+" + MerchantUpgradeManager.Instance.AnyGetRewardFlat(merchant,stat.type).ToStringSmart(1);
                stat.currencyMulti_txt.text = ((MerchantUpgradeManager.Instance.AnyGetRewardMulti(merchant,stat.type) -1)*100).ToString("F0") + "%"; // COMMENTED OUT FOR WORKING ON UNIFIED
                //  stat.currencyMulti_txt.text = ((MerchantUpgradeManager.Instance.AnyGetRewardPowerFloat() -1)*100).ToStringSmart(1) + "%";

                // if(barterManager.merchantBonuses[merchant].rewardMultiplierBonus[stat.type] < 1){ // REMOVED WHEN WORKING ON UNIFIED
                //     stat.currencyMulti_txt.color = new Color(1f,0f,0f,1f);
                // }else{
                //     stat.currencyMulti_txt.color = new Color(0f,1f,0f,1f);
                // }
            }
        // myText.text = $"testing my new text with bonus - {barterManager.merchantBonuses[merchant].rewardBaseFlatIncrease[CurrencyTypes.wheat]}";
        
    }

}
