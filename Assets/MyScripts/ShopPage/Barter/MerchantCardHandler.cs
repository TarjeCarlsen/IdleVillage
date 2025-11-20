
using System;
using System.Collections.Generic;
using LargeNumbers;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public enum IsWhatDatatype{
    isInt,
    isFloatDatatype,
    isAlphabeticnotationDatatype,
}

public class MerchantCardHandler : MonoBehaviour
{
    [SerializeField] private BarterManager barterManager;
    [SerializeField] UpgradeApplier upgradeApplier;
    [SerializeField] private TMP_Text pointCost_txt;
    [SerializeField] private TMP_Text header_lvl_txt;
    [SerializeField] private TMP_Text affectedUpgradeText_txt;
    [SerializeField] private GameObject cardObejct;
    public int upgradeLevel = 0;


    [Header("Define what type to display")]
    [SerializeField] private bool isPercentage = false;
    [SerializeField] private bool useMinusValue = false;
    [SerializeField] private float minusThis_forDisplayValue;
    [Header("Define max level and cost")]
    [SerializeField] private int maxLevel = 10;
    [SerializeField] private int skillPointCost;

    [SerializeField] private IsWhatDatatype isWhatDataType;
    [SerializeField] private UpgradeID upgradeID;

    [Header("Define what merchants and currencytypes to upgrade. Should correspond with scriptable object")]
    [SerializeField] private List<Merchants> merchants;
    [SerializeField] private List<CurrencyTypes> currencyTypes;


    private string templateText;
    public event Action OnBought;
    public static event System.Action<MerchantCardHandler> OnAnyCardOpened;
    private void Awake()
    {
        barterManager = GameObject.FindGameObjectWithTag("ShopPage").GetComponent<BarterManager>();
        templateText = affectedUpgradeText_txt.text;
        UpdateUI(upgradeID, isWhatDataType, merchants[0], currencyTypes[0]);
    }

    private void OnEnable()
    {
        OnAnyCardOpened += HandleOtherCardOpened;
        // barterManager.OnBarterClaimed += UpdateUIOnBarterComplete;
        barterManager.OnUpgradeBought += UpdateUI;
    }

    private void OnDisable()
    {
        OnAnyCardOpened -= HandleOtherCardOpened;
        // barterManager.OnBarterClaimed -= UpdateUIOnBarterComplete;
        barterManager.OnUpgradeBought -= UpdateUI;
    }

    private bool CanAfford()
    {
        return barterManager.merchantInfos[merchants[0]].skillPoints >= skillPointCost;
    }


    private void HandleOtherCardOpened(MerchantCardHandler openedCard)
    {
        if (openedCard != this)
        {
            // Close this card if it's not the one clicked
            cardObejct.SetActive(false);
        }
    }
    public void OnOpenCardClick()
    {
        // Notify all others first
        OnAnyCardOpened?.Invoke(this);

        // Then open this card
        cardObejct.SetActive(true);
    }


    public void OnUpgradeClick()
    {
        if (CanAfford() && upgradeLevel < maxLevel)
        {
            barterManager.merchantInfos[merchants[0]].skillPoints -= skillPointCost; // only the first merchant pays for upgrade
            upgradeApplier.ApplyUpgrade();

            foreach(Merchants merch in merchants){
                foreach(CurrencyTypes type in currencyTypes){
                    barterManager.UpgradeBought(upgradeID, isWhatDataType, merch, type);

            UpdateUI(upgradeID, isWhatDataType, merch,type);
                }
            }
            upgradeLevel++;
            OnBought?.Invoke();
        }
        else
        {
            print("Cannot afford upgrade or reached max lvl!");
        }
    }

            // if (isCurrencyUpgrade)
            // {
            //     foreach (CurrencyTypes type in currenciesToUpgrade)
            //     {
            //         barterManager.UpgradeBought(merchant, type, merchantUpgradeTypeToUpgrade);
            //     }
            // }
            // else if (isGeneralUpgrade)
            // {
            //     barterManager.UpgradeBought(merchant, CurrencyTypes.money, merchantUpgradeTypeToUpgrade); // SENDING IN CURRENCYTYPE money EVEN THO ITS NOT USED
            //                                                                                               // THIS JUST BECAUSE ITS REQUIRED AND USED FOR UPGRADES THAT USE
            //                                                                                               // CURRENCY. 
            // }

            // if (isBasedOfBarterTrades)
            // {
            // UpdateUIOnBarterComplete(merchant);
            // }
            // else
            // {
             // CHANGE THIS TO WORK FOR JUST THE ONE UPGRADE CLICKED!
                // UpdateUI_TESTING(UpgradeID.RewardFlat,merchant, currencyTypes);
            
            // }




    private void UpdateUI(UpgradeID _upgradeID, IsWhatDatatype isWhatDatatype, Merchants _merchant, CurrencyTypes _currencyTypes)
    {
        if(upgradeID != _upgradeID) return;
        pointCost_txt.text = barterManager.merchantInfos[merchants[0]].skillPoints.ToString() + "/" + skillPointCost.ToString();
        header_lvl_txt.text = string.Format("Lv.{0:F0} / Lv.{1:F0}", upgradeLevel, maxLevel);
        if (affectedUpgradeText_txt != null)
        {
            string oldText = affectedUpgradeText_txt.text;
            string updatedText = "";

            // print($"called from id - {_upgradeID} actuall id - {upgradeID} type - {_currencyTypes}");
            switch (isWhatDataType)
            {
                case IsWhatDatatype.isAlphabeticnotationDatatype:
                AlphabeticNotation alphaResult = MerchantUpgradeManager.Instance.GetAlphabetic(_upgradeID, _merchant, _currencyTypes);
                updatedText = System.Text.RegularExpressions.Regex.Replace(
                    templateText,
                    @"\{.*?\}",          //((reverseCounting ? (1f - MerchantUpgradeManager.Instance.FredGetRewardPowerFloat(fredUpgradeTypesFloat)) : (MerchantUpgradeManager.Instance.FredGetRewardPowerFloat(fredUpgradeTypesFloat) - minusThis_forDisplayValue)) * 100).ToString
                                $"<color=green>{(useMinusValue ? alphaResult- minusThis_forDisplayValue : alphaResult).ToString()}</color>");
                break;
                
                case IsWhatDatatype.isInt:
                int intResult = MerchantUpgradeManager.Instance.GetInt(_upgradeID, _merchant, _currencyTypes);
                updatedText = System.Text.RegularExpressions.Regex.Replace(
                    templateText,
                    @"\{.*?\}",          //((reverseCounting ? (1f - MerchantUpgradeManager.Instance.FredGetRewardPowerFloat(fredUpgradeTypesFloat)) : (MerchantUpgradeManager.Instance.FredGetRewardPowerFloat(fredUpgradeTypesFloat) - minusThis_forDisplayValue)) * 100).ToString
                                $"<color=green>{(useMinusValue ? intResult- minusThis_forDisplayValue : intResult).ToString()}</color>");
                break;
                case IsWhatDatatype.isFloatDatatype:
                float floatResult = MerchantUpgradeManager.Instance.GetFloat(_upgradeID, _merchant, _currencyTypes);
                float finalValue = useMinusValue ? floatResult - minusThis_forDisplayValue : floatResult;
                string formatted = isPercentage ? (finalValue * 100f).ToString("F0") + "%": finalValue.ToString();
                updatedText = System.Text.RegularExpressions.Regex.Replace(
                    templateText,
                    @"\{.*?\}",         
                                $"<color=green>{formatted}</color>");
                break;
            }
            affectedUpgradeText_txt.text = updatedText;
        }

    }
}

    // private void UpdateUIOnBarterComplete(Merchants merchant)
    // {
    //     //     if (affectedUpgradeText_txt != null)
    //     //     {
    //     //         pointCost_txt.text = barterManager.merchantInfos[merchant].skillPoints.ToString() + "/" + skillPointCost.ToString();
    //     //         header_lvl_txt.text = string.Format("Lv.{0:F0} / Lv.{1:F0}", upgradeLevel, maxLevel);
    //     //         string oldText = affectedUpgradeText_txt.text;
    //     //         string updatedText = "";

    //     //         if (isBasedOfBarterTrades && isPercentageUpgrade && isAlphabeticnotationDatatype)
    //     //         {
    //     //             switch (merchant)
    //     //             {
    //     //                 case Merchants.BobTheMerchant:
    //     //                     updatedText = System.Text.RegularExpressions.Regex.Replace(
    //     //                         templateText,
    //     //                         @"\{(.*?)\}",
    //     //                         match =>
    //     //                         {
    //     //                             string placeholder = match.Groups[1].Value;
    //     //                             switch (placeholder)
    //     //                             {
    //     //                                 case "bonus1":
    //     //                                     return $"<color=green>{((((MerchantUpgradeManager.Instance.BobGetRewardPower(bobUpgradeTypes) - minusThis_forDisplayValue) * 100) * barterManager.merchantInfos[merchant].completedInArow)).ToStringSmart(0)}</color>";
    //     //                                 case "bonus2":
    //     //                                     return $"<color=green>{barterManager.merchantInfos[merchant].completedInArow}</color>";
    //     //                                 default:
    //     //                                     return match.Value; // leave unknown placeholders as-is
    //     //                             }
    //     //                         }
    //     //                     );
    //     //                     affectedUpgradeText_txt.text = updatedText;
    //     //                     break;

    //     //                 case Merchants.CarlTheMerchant:
    //     //                     updatedText = System.Text.RegularExpressions.Regex.Replace(
    //     //                         templateText,
    //     //                         @"\{(.*?)\}",
    //     //                         match =>
    //     //                         {
    //     //                             string placeholder = match.Groups[1].Value;
    //     //                             switch (placeholder)
    //     //                             {
    //     //                                 case "bonus1":
    //     //                                     return $"<color=green>{((((MerchantUpgradeManager.Instance.CarlGetRewardPower(carlUpgradeTypes) - minusThis_forDisplayValue) * 100) * barterManager.merchantInfos[merchant].completedInArow)).ToStringSmart(0)}</color>";
    //     //                                 case "bonus2":
    //     //                                     return $"<color=green>{barterManager.merchantInfos[merchant].completedInArow}</color>";
    //     //                                 default:
    //     //                                     return match.Value; // leave unknown placeholders as-is
    //     //                             }
    //     //                         }
    //     //                     );
    //     //                     affectedUpgradeText_txt.text = updatedText;
    //     //                     break;
    //     //                 case Merchants.ChloeTheMerchant:
    //     //                     break;
    //     //                 case Merchants.FredTheMerchant:
    //     //                     break;
    //     //                 case Merchants.SamTheMerchant:
    //     //                     break;
    //     //                 case Merchants.RogerTheMerchant:
    //     //                     break;


    //     //             }
    //     //         }
    //     //     }
    // }
// // ------------------------- ADD ALL MERCHANTS UPGRADE DISPLAYS HERE ----------------------- //
// if (isBoolDatatype)
// {
//     switch (merchant)
//     {
//         case Merchants.ChloeTheMerchant:
//             // affectedUpgradeText_txt.text = oldText;
//             // print("old text = "+ oldText);
//             return;
//             //add all merchants here
//     }
// }
// if (isIntDatatype)
// {

//     switch (merchant)
//     {
//         case Merchants.BobTheMerchant:
//             updatedText = System.Text.RegularExpressions.Regex.Replace(
//                 templateText,
//                 @"\{.*?\}",
//                     $"<color=green> {MerchantUpgradeManager.Instance.BobGetRewardPowerInt(bobUpgradeTypesInt).ToString()}</color>"
//             );
//             break;
//         //add all merchants here
//         case Merchants.CarlTheMerchant:
//             updatedText = System.Text.RegularExpressions.Regex.Replace(
//                 templateText,
//                 @"\{.*?\}",
//                     $"<color=green> {MerchantUpgradeManager.Instance.CarlGetRewardPowerInt(carlUpgradeTypesInt).ToString()}</color>"
//             );
//             break;
//         case Merchants.ChloeTheMerchant:
//             updatedText = System.Text.RegularExpressions.Regex.Replace(
//                 templateText,
//                 @"\{.*?\}",
//                     $"<color=green> {MerchantUpgradeManager.Instance.ChloeGetRewardPowerInt(chloeUpgradeTypesInt).ToString()}</color>"
//             );
//             break;
//             //add all merchants here
//         case Merchants.FredTheMerchant:
//             updatedText = System.Text.RegularExpressions.Regex.Replace(
//                 templateText,
//                 @"\{.*?\}",
//                     $"<color=green> {MerchantUpgradeManager.Instance.FredGetRewardPowerInt(fredUpgradeTypesInt).ToString()}</color>"
//             );
//             break;
//             //add all merchants here
//     }
// }
// if (isAlphabeticnotationDatatype && isPercentageUpgrade)
// {
//     switch (merchant)
//     {
//         case Merchants.BobTheMerchant:
//             updatedText = System.Text.RegularExpressions.Regex.Replace(
//                 templateText,
//                 @"\{.*?\}",
//                 $"<color=green>{((MerchantUpgradeManager.Instance.BobGetRewardPower(bobUpgradeTypes) - minusThis_forDisplayValue) * 100).ToStringSmart(0)}</color>"
//             );
//             break;
//         //add all merchants here
//         case Merchants.CarlTheMerchant:
//             updatedText = System.Text.RegularExpressions.Regex.Replace(
//                 templateText,
//                 @"\{.*?\}",
//                 $"<color=green>{((MerchantUpgradeManager.Instance.CarlGetRewardPower(carlUpgradeTypes) - minusThis_forDisplayValue) * 100).ToStringSmart(0)}</color>"
//             );
//             break;
//         case Merchants.ChloeTheMerchant:
//             updatedText = System.Text.RegularExpressions.Regex.Replace(
//                 templateText,
//                 @"\{.*?\}",
//                 $"<color=green>{((MerchantUpgradeManager.Instance.ChloeGetRewardPower(chloeUpgradeTypes) - minusThis_forDisplayValue) * 100).ToStringSmart(0)}</color>"
//             );
//             break;
//             //add all merchants here
//         case Merchants.FredTheMerchant:
//             updatedText = System.Text.RegularExpressions.Regex.Replace(
//                 templateText,
//                 @"\{.*?\}",
//                 $"<color=green>{((MerchantUpgradeManager.Instance.FredGetRewardPower(fredUpgradeTypes) - minusThis_forDisplayValue) * 100).ToStringSmart(0)}</color>"
//             );
//             break;
//             //add all merchants here

//     }
// }
// if (isAlphabeticnotationDatatype && !isPercentageUpgrade)
// {
//     switch (merchant)
//     {
//         case Merchants.BobTheMerchant:
//             updatedText = System.Text.RegularExpressions.Regex.Replace(
//                 templateText,
//                 @"\{.*?\}",
//                 $"<color=green>{MerchantUpgradeManager.Instance.BobGetRewardPower(bobUpgradeTypes).ToStringSmart(0)}</color>"
//             );
//             break;
//         //add all merchants here
//         case Merchants.CarlTheMerchant:
//             updatedText = System.Text.RegularExpressions.Regex.Replace(
//                 templateText,
//                 @"\{.*?\}",
//                 $"<color=green>{MerchantUpgradeManager.Instance.CarlGetRewardPower(carlUpgradeTypes).ToStringSmart(0)}</color>"
//             );
//             break;
//         case Merchants.ChloeTheMerchant:
//             updatedText = System.Text.RegularExpressions.Regex.Replace(
//                 templateText,
//                 @"\{.*?\}",
//                 $"<color=green>{MerchantUpgradeManager.Instance.ChloeGetRewardPower(chloeUpgradeTypes).ToStringSmart(0)}</color>"
//             );
//             break;
//             //add all merchants here
//         case Merchants.FredTheMerchant:
//             updatedText = System.Text.RegularExpressions.Regex.Replace(
//                 templateText,
//                 @"\{.*?\}",
//                 $"<color=green>{MerchantUpgradeManager.Instance.FredGetRewardPower(fredUpgradeTypes).ToStringSmart(0)}</color>"
//             );
//             break;
//             //add all merchants here

//     }
// }

// if (isFloatDatatype && !isPercentageUpgrade)
// {
//     switch (merchant)
//     {
//         case Merchants.BobTheMerchant:
//             updatedText = System.Text.RegularExpressions.Regex.Replace(
//                 templateText,
//                 @"\{.*?\}",
//                 $"<color=green>{MerchantUpgradeManager.Instance.BobGetRewardPowerFloat(bobUpgradeTypesFloat).ToString()}</color>"
//             );
//             break;
//         //add all merchants here
//         case Merchants.CarlTheMerchant:
//             updatedText = System.Text.RegularExpressions.Regex.Replace(
//                 templateText,
//                 @"\{.*?\}",
//                 $"<color=green>{MerchantUpgradeManager.Instance.CarlGetRewardPowerFloat(carlUpgradeTypesFloat).ToString()}</color>"
//             );
//             break;
//         case Merchants.ChloeTheMerchant:
//             updatedText = System.Text.RegularExpressions.Regex.Replace(
//                 templateText,
//                 @"\{.*?\}",
//                 $"<color=green>{MerchantUpgradeManager.Instance.ChloeGetRewardPowerFloat(chloeUpgradeTypesFloat).ToString()}</color>"
//             );
//             break;
//             //add all merchants here
//         case Merchants.FredTheMerchant:
//             updatedText = System.Text.RegularExpressions.Regex.Replace(
//                 templateText,
//                 @"\{.*?\}",
//                 $"<color=green>{MerchantUpgradeManager.Instance.FredGetRewardPowerFloat(fredUpgradeTypesFloat).ToString()}</color>"
//             );
//             break;
//             //add all merchants here

//     }
// }
// if (isFloatDatatype && isPercentageUpgrade)
// {
//     switch (merchant)
//     {
//         case Merchants.BobTheMerchant:
//             updatedText = System.Text.RegularExpressions.Regex.Replace(
//                 templateText,
//                 @"\{.*?\}",
//                 $"<color=green>{((reverseCounting ? (1f - MerchantUpgradeManager.Instance.BobGetRewardPowerFloat(bobUpgradeTypesFloat)) : MerchantUpgradeManager.Instance.BobGetRewardPowerFloat(bobUpgradeTypesFloat) - minusThis_forDisplayValue) * 100).ToString("F0")}</color>"
//             );
//             break;
//         //add all merchants here
//         case Merchants.CarlTheMerchant:
//             updatedText = System.Text.RegularExpressions.Regex.Replace(
//                 templateText,
//                 @"\{.*?\}",
//                 $"<color=green>{((reverseCounting ? (1f - MerchantUpgradeManager.Instance.CarlGetRewardPowerFloat(carlUpgradeTypesFloat)) : (MerchantUpgradeManager.Instance.CarlGetRewardPowerFloat(carlUpgradeTypesFloat) - minusThis_forDisplayValue)) * 100).ToString("F0")}</color>"
//             );
//             break;
//         case Merchants.ChloeTheMerchant:
//             updatedText = System.Text.RegularExpressions.Regex.Replace(
//                 templateText,
//                 @"\{.*?\}",
//                 $"<color=green>{((reverseCounting ? (1f - MerchantUpgradeManager.Instance.ChloeGetRewardPowerFloat(chloeUpgradeTypesFloat)) : (MerchantUpgradeManager.Instance.ChloeGetRewardPowerFloat(chloeUpgradeTypesFloat) - minusThis_forDisplayValue)) * 100).ToString("F0")}</color>"
//             );
//             break;
//             //add all merchants here
//         case Merchants.FredTheMerchant:
//             updatedText = System.Text.RegularExpressions.Regex.Replace(
//                 templateText,
//                 @"\{.*?\}",
//                 $"<color=green>{((reverseCounting ? (1f - MerchantUpgradeManager.Instance.FredGetRewardPowerFloat(fredUpgradeTypesFloat)) : (MerchantUpgradeManager.Instance.FredGetRewardPowerFloat(fredUpgradeTypesFloat) - minusThis_forDisplayValue)) * 100).ToString("F0")}</color>"
//             );
//             break;
//             //add all merchants here

//     }
// }


