
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MerchantCardHandler : MonoBehaviour
{
    [SerializeField] private BarterManager barterManager;
    [SerializeField] UpgradeApplier upgradeApplier;
    [SerializeField] private Merchants merchant;


    [SerializeField] private TMP_Text pointCost_txt;
    [SerializeField] private TMP_Text header_lvl_txt;
    [SerializeField] private TMP_Text affectedUpgradeText_txt;
    [SerializeField] private GameObject cardObejct;
    [SerializeField] private int skillPointCost;

    [SerializeField] private bool isIntDatatype = false;
    [SerializeField] private bool isAlphabeticnotationDatatype = true;
    [SerializeField] private bool isFloatDatatype = false;
    [SerializeField] private bool isPercentageUpgrade = false;
    [SerializeField] private bool isBasedOfBarterTrades = false;
    [SerializeField] private float minusThis_forDisplayValue = 1f; // USED FOR INITIATING DISPLAYED VALUES. FOR EXAMPLE SOME VALUES WILL HAVE A START
                                                                   // VALUE AT 1, WHILE STILL WANTING TO DISPLAY PERCENTAGE FOR THE VALUE.
                                                                   // SETTING THIS TO 0 WILL THEN LET THE PERCENTAGE START FROM 0 WITHOUT MODIFYING 
                                                                   // THE ACTUALL BONUS VALUE
    [SerializeField] private BobUpgradeTypesInt bobUpgradeTypesInt;
    [SerializeField] private BobUpgradeTypesFloats bobUpgradeTypesFloat;
    [SerializeField] private BobUpgradeTypes bobUpgradeTypes;
    [SerializeField] private bool isCurrencyUpgrade;
    [SerializeField] private bool isGeneralUpgrade;
    [SerializeField] private List<CurrencyTypes> currenciesToUpgrade;

    private string templateText;
    public event Action OnBought;
    public int upgradeLevel = 0;
    [SerializeField] private int maxLevel = 10;
    public static event System.Action<MerchantCardHandler> OnAnyCardOpened;
    private void Awake()
    {
        barterManager = GameObject.FindGameObjectWithTag("ShopPage").GetComponent<BarterManager>();
        templateText = affectedUpgradeText_txt.text;
        UpdateUI();
    }

    private void OnEnable()
    {
        OnAnyCardOpened += HandleOtherCardOpened;
        barterManager.OnBarterClaimed += UpdateUIOnBarterComplete;
    }

    private void OnDisable()
    {
        OnAnyCardOpened -= HandleOtherCardOpened;
        barterManager.OnBarterClaimed -= UpdateUIOnBarterComplete;
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
            if (isCurrencyUpgrade)
            {
                foreach (CurrencyTypes type in currenciesToUpgrade)
                {
                    barterManager.UpgradeBoughtCurrency(merchant, type);
                }
            }
            else if (isGeneralUpgrade)
            {
                barterManager.UpgradeBoughtGeneral(merchant);
            }
            upgradeLevel++;

            if (isBasedOfBarterTrades)
            {
                UpdateUIOnBarterComplete(merchant);
            }
            else
            {
                UpdateUI();
            }

            OnBought?.Invoke();
            print("bought upgrade!");
        }
        else
        {
            print("Cannot afford upgrade or reached max lvl!");
        }
    }



    private void UpdateUIOnBarterComplete(Merchants merchant)
    {
        if (affectedUpgradeText_txt != null)
        {
            pointCost_txt.text = barterManager.merchantInfos[merchant].skillPoints.ToString() + "/" + skillPointCost.ToString();
            header_lvl_txt.text = string.Format("Lv.{0:F0} / Lv.{1:F0}", upgradeLevel, maxLevel);
            string oldText = affectedUpgradeText_txt.text;
            string updatedText = "";

            if (isBasedOfBarterTrades && isPercentageUpgrade && isAlphabeticnotationDatatype)
            {
                switch (merchant)
                {
                    case Merchants.BobTheMerchant:
                        print("barters completeCOMOOEd = " + barterManager.merchantInfos[merchant].completedInArow);
                        updatedText = System.Text.RegularExpressions.Regex.Replace(
                            templateText,
                            @"\{(.*?)\}",
                            match =>
                            {
                                string placeholder = match.Groups[1].Value;
                                switch (placeholder)
                                {
                                    case "bonus1":
                                        return $"<color=green>{((((MerchantUpgradeManager.Instance.BobGetRewardPower(bobUpgradeTypes) - minusThis_forDisplayValue) * 100) * barterManager.merchantInfos[merchant].completedInArow)).ToStringSmart(0)}</color>";
                                    case "bonus2":
                                        return $"<color=green>{barterManager.merchantInfos[merchant].completedInArow}</color>";
                                    default:
                                        return match.Value; // leave unknown placeholders as-is
                                }
                            }
                        );
                        affectedUpgradeText_txt.text = updatedText;
                        break;
                    //add all merchants here
                    case Merchants.CarlTheMerchant:
                        break;
                    case Merchants.ChloeTheMerchant:
                        break;
                    case Merchants.FredTheMerchant:
                        break;
                    case Merchants.SamTheMerchant:
                        break;
                    case Merchants.RogerTheMerchant:
                        break;


                }
            }
        }
    }

    private void UpdateUI()
    {
        pointCost_txt.text = barterManager.merchantInfos[merchant].skillPoints.ToString() + "/" + skillPointCost.ToString();
        header_lvl_txt.text = string.Format("Lv.{0:F0} / Lv.{1:F0}", upgradeLevel, maxLevel);
        if (affectedUpgradeText_txt != null)
        {
            string oldText = affectedUpgradeText_txt.text;
            string updatedText = "";

            // ------------------------- ADD ALL MERCHANTS UPGRADE DISPLAYS HERE ----------------------- //
            if (isIntDatatype)
            {

                switch (merchant)
                {
                    case Merchants.BobTheMerchant:
                        updatedText = System.Text.RegularExpressions.Regex.Replace(
                            templateText,
                            @"\{.*?\}",
                                $"<color=green> {MerchantUpgradeManager.Instance.BobGetRewardPowerInt(bobUpgradeTypesInt).ToString()}</color>"
                        );
                        break;
                        //add all merchants here
                }
            }
            if (isAlphabeticnotationDatatype && isPercentageUpgrade)
            {
                switch (merchant)
                {
                    case Merchants.BobTheMerchant:
                        updatedText = System.Text.RegularExpressions.Regex.Replace(
                            templateText,
                            @"\{.*?\}",
                            $"<color=green>{((MerchantUpgradeManager.Instance.BobGetRewardPower(bobUpgradeTypes) - minusThis_forDisplayValue) * 100).ToStringSmart(0)}</color>"
                        );
                        break;
                        //add all merchants here

                }
            }
            if (isAlphabeticnotationDatatype && !isPercentageUpgrade)
            {
                switch (merchant)
                {
                    case Merchants.BobTheMerchant:
                        updatedText = System.Text.RegularExpressions.Regex.Replace(
                            templateText,
                            @"\{.*?\}",
                            $"<color=green>{MerchantUpgradeManager.Instance.BobGetRewardPower(bobUpgradeTypes).ToStringSmart(0)}</color>"
                        );
                        break;
                        //add all merchants here

                }
            }

            if (isFloatDatatype && !isPercentageUpgrade)
            {
                switch (merchant)
                {
                    case Merchants.BobTheMerchant:
                        updatedText = System.Text.RegularExpressions.Regex.Replace(
                            templateText,
                            @"\{.*?\}",
                            $"<color=green>{MerchantUpgradeManager.Instance.BobGetRewardPowerFloat(bobUpgradeTypesFloat).ToString()}</color>"
                        );
                        break;
                        //add all merchants here

                }
            }
            if (isFloatDatatype && isPercentageUpgrade)
            {
                switch (merchant)
                {
                    case Merchants.BobTheMerchant:
                        updatedText = System.Text.RegularExpressions.Regex.Replace(
                            templateText,
                            @"\{.*?\}",
                            $"<color=green>{((MerchantUpgradeManager.Instance.BobGetRewardPowerFloat(bobUpgradeTypesFloat) - minusThis_forDisplayValue) * 100).ToString("F0")}</color>"
                        );
                        break;
                        //add all merchants here

                }
            }


            affectedUpgradeText_txt.text = updatedText;
        }
    }

}
