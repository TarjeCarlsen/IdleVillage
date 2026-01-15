using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using LargeNumbers;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CardInfo : MonoBehaviour
{
    [Header("Specify what id, datatype and currencytype for the upgrade. Default values for unlock upgrades")]
    [SerializeField] private UpgradeIDGlobal upgradeIDGlobal;
    [SerializeField] private IsWhatDatatype isWhatDatatype;
    [SerializeField] private CurrencyTypes currencyType;
    [SerializeField] CardData cardData;
    [SerializeField] private List<CardInfoContent> cardInfoContents;


    [SerializeField] private bool isPercentage = false;
    [SerializeField] private bool useMinusValue = false;
    [SerializeField] private float minusThis_forDisplayValue;
    [SerializeField] private int amountOfDecimals; // currently only implemented for floats
    [SerializeField] Image content_img;
    [SerializeField] TMP_Text header_txt;
    [SerializeField] TMP_Text description_txt;
    [SerializeField] TMP_Text level_txt;
    const string POSITIVE_COLOR = "#1E7F1E"; // dark green color
    const string NEGATIVE_COLOR = "#7A1E1E"; // dark red color
    private string templateText;
    private HouseManager houseManager;
    private UpgradeHandler upgradeHandler;
    public event Action OnBought;
    private void HideLevel() => level_txt.text = "";
    public int level;
    private int maxLevel;
    private bool isInitialized = false;
    private Camera uiCamera;
    private RectTransform rectTransform;
    [System.Serializable]
    public class CardInfoContent
    {
        public GameObject price_parent_obj;
        public TMP_Text price_text;
        public Image price_img;
        public CurrencyTypes types;
        public AlphabeticNotation rawPrice;
    }

    private void Awake()
    {
        houseManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<HouseManager>();
        upgradeHandler = GameObject.FindGameObjectWithTag("ShopPage").GetComponent<UpgradeHandler>();
        rectTransform = GetComponent<RectTransform>();
        uiCamera = Camera.main;
        Init();
        templateText = description_txt.text;
        
        UpdateDescription(upgradeIDGlobal,isWhatDatatype,currencyType);

    }

    private void OnEnable()
    {
        upgradeHandler.OnAnyUpgrade += UpdateDescription;
    }
    private void OnDisable()
    {
        upgradeHandler.OnAnyUpgrade -= UpdateDescription;

    }

    public void Init()
    {
        if (isInitialized) return;

        header_txt.text = cardData.header_txt;
        content_img.sprite = cardData.content_images[level];
        description_txt.text = cardData.descriptionText_txt;
        level_txt.text = "Lv. " + level.ToString();
        maxLevel = cardData.maxLevel;

        for (int i = 0; i < cardData.cardDataInfo.Count; i++)
        {
            if (cardInfoContents[i] == null)
            {
                cardInfoContents[i].price_parent_obj.SetActive(false);
            }
            else
            {
                cardInfoContents[i].price_parent_obj.SetActive(true);
                cardInfoContents[i].price_text.text = cardData.cardDataInfo[i].price.ToString();
                cardInfoContents[i].price_img.sprite = cardData.cardDataInfo[i].sprite;
                cardInfoContents[i].types = cardData.cardDataInfo[i].type;
                cardInfoContents[i].rawPrice = cardData.cardDataInfo[i].price;
            }
        }
        if (!cardData.useLevels) HideLevel();
        isInitialized = true;
    }

    public bool CanAfford()
    {
        foreach (CardInfoContent price in cardInfoContents)
        {
            if (price.rawPrice > MoneyManager.Instance.GetCurrency(price.types))
            {
                return false;
            }
        }
        return true;
    }

    public void OnBuyClick()
    {
        if (level == maxLevel && cardData.useLevels)
        {
            return;
        }
        if (CanAfford())
        {
            foreach (CardInfoContent price in cardInfoContents)
            {
                MoneyManager.Instance.SubtractCurrency(price.types, price.rawPrice);
            }
            if (cardData.useLevels) level++;
            CalculateNewPrice();
            UpdateUI();
            OnBought?.Invoke();
            upgradeHandler.OnUpgradeBought(upgradeIDGlobal, isWhatDatatype, currencyType);
            // upgradeApplier.ApplyUpgrade();
        }
    }


    private void CalculateNewPrice()
    {
        for (int i = 0; i < cardInfoContents.Count; i++)
        {
            AlphabeticNotation result = cardInfoContents[i].rawPrice * cardData.cardDataInfo[i].priceMultiplier;
            cardInfoContents[i].price_text.text = result.ToStringSmart(1);
            cardInfoContents[i].rawPrice = result;
        }
    }

    private void UpdateUI()
    {
        if (level == maxLevel && cardData.useLevels)
        {
            level_txt.text = "Lv. " + level.ToString() + " MAX";
            foreach (CardInfoContent info in cardInfoContents)
            {
                info.price_text.text = "";
                info.price_img = null;
                info.price_parent_obj.SetActive(false);
            }
        }
        else if (cardData.useLevels)
        {
            level_txt.text = "Lv. " + level.ToString();
        }
        if (!cardData.useLevels) HideLevel();

    }
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                var pointer = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;

                if (pointer != null)
                {
                    if (pointer.transform.IsChildOf(transform))
                        return;
                }
            }

            if (!RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition, uiCamera))
            {
                gameObject.SetActive(false);
            }
        }
    }





    private void UpdateDescription(UpgradeIDGlobal id, IsWhatDatatype datatype, CurrencyTypes currencyTypes)
    {
        print("inside updateDescription");

        if (description_txt == null) return;

        string oldText = description_txt.text;
        string updatedText = "";

        switch (datatype)
        {
            case IsWhatDatatype.isAlphabeticnotationDatatype:
                AlphabeticNotation alphaResult = UpgradeManager.Instance.GetAlphabetic(id, currencyType);
                updatedText = System.Text.RegularExpressions.Regex.Replace(
                    templateText,
                    @"\{.*?\}",          //((reverseCounting ? (1f - MerchantUpgradeManager.Instance.FredGetRewardPowerFloat(fredUpgradeTypesFloat)) : (MerchantUpgradeManager.Instance.FredGetRewardPowerFloat(fredUpgradeTypesFloat) - minusThis_forDisplayValue)) * 100).ToString
                                $"<color={POSITIVE_COLOR}>{(useMinusValue ? alphaResult - minusThis_forDisplayValue : alphaResult).ToString()}</color>");
                break;

            case IsWhatDatatype.isInt:
                int intResult = UpgradeManager.Instance.GetInt(id, currencyType);
                updatedText = System.Text.RegularExpressions.Regex.Replace(
                    templateText,
                    @"\{.*?\}",          //((reverseCounting ? (1f - MerchantUpgradeManager.Instance.FredGetRewardPowerFloat(fredUpgradeTypesFloat)) : (MerchantUpgradeManager.Instance.FredGetRewardPowerFloat(fredUpgradeTypesFloat) - minusThis_forDisplayValue)) * 100).ToString
                                $"<color={POSITIVE_COLOR}>{(useMinusValue ? intResult - minusThis_forDisplayValue : intResult).ToString()}</color>");
                break;
            case IsWhatDatatype.isFloatDatatype:
            print("inside float datatype");
                float floatResult = UpgradeManager.Instance.GetFloat(id, currencyType);
                float finalValue = useMinusValue ? floatResult - minusThis_forDisplayValue : floatResult;

                string decimalFormat = "F" + amountOfDecimals;
                string formatted = isPercentage ? (finalValue * 100f).ToString(decimalFormat) + "%" : finalValue.ToString(decimalFormat);
                updatedText = System.Text.RegularExpressions.Regex.Replace(
                    templateText,
                    @"\{.*?\}",
                                $"<color={POSITIVE_COLOR}>{formatted}</color>");
                break;
            case IsWhatDatatype.dontDisplay:
                updatedText = oldText;
                break;
        }
        print("updated text = " + oldText);
        description_txt.text = updatedText;


    }



}
