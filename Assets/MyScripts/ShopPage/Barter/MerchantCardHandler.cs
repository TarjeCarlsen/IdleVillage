
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
    }

    private void OnDisable()
    {
        OnAnyCardOpened -= HandleOtherCardOpened;
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
            }else if(isGeneralUpgrade){
                barterManager.UpgradeBoughtGeneral(merchant);
            }
            upgradeLevel++;
            UpdateUI();
            OnBought?.Invoke();
            print("bought upgrade!");
        }
        else
        {
            print("Cannot afford upgrade or reached max lvl!");
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
                            $"<color=green>{((MerchantUpgradeManager.Instance.BobGetRewardPower(bobUpgradeTypes) - 1) * 100).ToStringSmart(0)}</color>"
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
                            $"<color=green>{((MerchantUpgradeManager.Instance.BobGetRewardPowerFloat(bobUpgradeTypesFloat)- 1) * 100).ToString("F0")}</color>"
                        );
                        break;
                        //add all merchants here

                }
            }
            affectedUpgradeText_txt.text = updatedText;
        }
    }

}
