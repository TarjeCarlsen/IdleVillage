using System.Collections.Generic;
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
        public AlphabeticNotation currentValue;
        public CurrencyTypes type;
        public  GameObject statInfoObjects;
        public  TMP_Text  statText_txt;
        public  Image statIcon_img;
        public  Sprite statIcon_sprite;
    }
    private void Start(){
        UpdateUI(merchant);
        
    }

    private void OnEnable(){
        barterManager.OnBarterLevelUp += UpdateUI;
        barterManager.OnUpgradeBought += UpdateUI;
    }
    private void OnDisable(){
        barterManager.OnBarterLevelUp -= UpdateUI;
        barterManager.OnUpgradeBought -= UpdateUI;
    }

    private void UpdateUI(Merchants _merchants){
        if(_merchants != merchant) return;
            availableSkillpoints_txt.text = barterManager.merchantInfos[merchant].skillPoints.ToString();
            level_txt.text ="Lv."+ barterManager.merchantInfos[merchant].merchantLevel.ToString();

            foreach(StatInfo stat in statBonuses){
                stat.statText_txt.text = barterManager.merchantBonuses[merchant].totalBonus[stat.type].ToStringSmart(1);
            }
        // myText.text = $"testing my new text with bonus - {barterManager.merchantBonuses[merchant].rewardBaseFlatIncrease[CurrencyTypes.wheat]}";
        
    }

}
