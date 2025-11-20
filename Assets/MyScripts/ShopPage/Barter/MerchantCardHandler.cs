
using System;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum IsWhatType{
    isFlatUpgrade,
    isMultiUpgrade,
    isMultiPercentage,
    isFlatIntUpgrade,
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

    [Header("Define merchant and currency type, used for start function")]
    [SerializeField] private Merchants merchant;
    [SerializeField] private CurrencyTypes currencyTypes;
    [Header("Define what type to display")]
    [SerializeField] private IsWhatType isWhatType;
    [SerializeField] private bool useMinusValue = false;
    [SerializeField] private float minusThis_forDisplayValue;
    [Header("Define max level and cost")]
    [SerializeField] private int maxLevel = 10;
    [SerializeField] private int skillPointCost;



    private string templateText;
    public event Action OnBought;
    public static event System.Action<MerchantCardHandler> OnAnyCardOpened;
    private void Awake()
    {
        barterManager = GameObject.FindGameObjectWithTag("ShopPage").GetComponent<BarterManager>();
        templateText = affectedUpgradeText_txt.text;
        UpdateUI(merchant, currencyTypes);
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
        return barterManager.merchantInfos[merchant].skillPoints >= skillPointCost;
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
            barterManager.merchantInfos[merchant].skillPoints -= skillPointCost;
            upgradeApplier.ApplyUpgrade();
            barterManager.UpgradeBought(merchant, currencyTypes);
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
            upgradeLevel++;

            // if (isBasedOfBarterTrades)
            // {
            // UpdateUIOnBarterComplete(merchant);
            // }
            // else
            // {
            UpdateUI(merchant, currencyTypes);
            // }

            OnBought?.Invoke();
        }
        else
        {
            print("Cannot afford upgrade or reached max lvl!");
        }
    }






    private void UpdateUI(Merchants _merchant, CurrencyTypes _currencyTypes)
    {
        print($"called from {_merchant} type {_currencyTypes} value = {MerchantUpgradeManager.Instance.AnyGetRewardFlat(_merchant, _currencyTypes)}");
        pointCost_txt.text = barterManager.merchantInfos[merchant].skillPoints.ToString() + "/" + skillPointCost.ToString();
        header_lvl_txt.text = string.Format("Lv.{0:F0} / Lv.{1:F0}", upgradeLevel, maxLevel);
        if (affectedUpgradeText_txt != null)
        {
            string oldText = affectedUpgradeText_txt.text;
            string updatedText = "";


            switch (isWhatType)
            {
                case IsWhatType.isFlatUpgrade:
                updatedText = System.Text.RegularExpressions.Regex.Replace(
                    templateText,
                    @"\{.*?\}",          //((reverseCounting ? (1f - MerchantUpgradeManager.Instance.FredGetRewardPowerFloat(fredUpgradeTypesFloat)) : (MerchantUpgradeManager.Instance.FredGetRewardPowerFloat(fredUpgradeTypesFloat) - minusThis_forDisplayValue)) * 100).ToString
                                $"<color=green>{(useMinusValue ? MerchantUpgradeManager.Instance.AnyGetRewardFlat(_merchant, _currencyTypes)- minusThis_forDisplayValue : MerchantUpgradeManager.Instance.AnyGetRewardFlat(_merchant, _currencyTypes)).ToString()}</color>");
                break;

                case IsWhatType.isFlatIntUpgrade:
                updatedText = System.Text.RegularExpressions.Regex.Replace(
                    templateText,
                    @"\{.*?\}",
                        $"<color=green> {(useMinusValue ? MerchantUpgradeManager.Instance.AnyGetRewardFlatInt(_merchant, _currencyTypes) - minusThis_forDisplayValue : MerchantUpgradeManager.Instance.AnyGetRewardFlatInt(_merchant, _currencyTypes)).ToString()}</color>");
                break;
                case IsWhatType.isMultiUpgrade:
                updatedText = System.Text.RegularExpressions.Regex.Replace(
                    templateText,
                    @"\{.*?\}",
                        $"<color=green> {(useMinusValue ? MerchantUpgradeManager.Instance.AnyGetRewardMulti(_merchant, _currencyTypes) - minusThis_forDisplayValue : MerchantUpgradeManager.Instance.AnyGetRewardMulti(_merchant, _currencyTypes)).ToString()}</color>");
                break;
                case IsWhatType.isMultiPercentage:
                updatedText = System.Text.RegularExpressions.Regex.Replace(
                    templateText,
                    @"\{.*?\}",
                        $"<color=green> {(useMinusValue ? ((MerchantUpgradeManager.Instance.AnyGetRewardMulti(_merchant, _currencyTypes)- minusThis_forDisplayValue) * 100) : MerchantUpgradeManager.Instance.AnyGetRewardMulti(_merchant, _currencyTypes)).ToString("F0") + "%"}</color>");
                break;
            }
            //  switch (merchant)
            //  {
            //      case Merchants.BobTheMerchant:
            //          updatedText = System.Text.RegularExpressions.Regex.Replace(
            //              templateText,
            //              @"\{.*?\}",
            //                  $"<color=green> {MerchantUpgradeManager.Instance.AnyGetRewardFlat(_merchant,_currencyTypes).ToString()}</color>"
            //          );
            //          break;

            // }
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


