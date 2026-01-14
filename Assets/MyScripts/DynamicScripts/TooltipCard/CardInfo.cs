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
    [SerializeField] Image content_img;
    [SerializeField] TMP_Text header_txt;
    [SerializeField] TMP_Text description_txt;
    [SerializeField] TMP_Text level_txt;
    [SerializeField] private List<CardInfoContent>  cardInfoContents;
    [SerializeField] private HouseManager houseManager;
    [SerializeField] private FarmManager farmManager;
    public event Action OnBought;
    private void HideLevel() => level_txt.text = "";
    public int level;
    private int maxLevel;
    private bool isInitialized = false;
    private Camera uiCamera;
    private RectTransform rectTransform;
[System.Serializable]
    public class CardInfoContent{
        public GameObject price_parent_obj;
        public TMP_Text price_text;
        public Image price_img;
        public CurrencyTypes types;
        public AlphabeticNotation rawPrice; 
    }

        private void Awake(){
        houseManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<HouseManager>();
        farmManager = GameObject.FindGameObjectWithTag("ShopPage").GetComponent<FarmManager>();
        rectTransform = GetComponent<RectTransform>();
        uiCamera = Camera.main; 
        Init();  
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
    if(!cardData.useLevels) HideLevel();
    isInitialized = true;
}

    public bool CanAfford(){
        foreach(CardInfoContent price in cardInfoContents){
            if (price.rawPrice > MoneyManager.Instance.GetCurrency(price.types)){
                return false;
            }
        }
        return true;
    }

    public void OnBuyClick(){
        if(level == maxLevel && cardData.useLevels){
            return;
        }
        if(CanAfford()){
            foreach(CardInfoContent price in cardInfoContents){
                MoneyManager.Instance.SubtractCurrency(price.types, price.rawPrice);
            }
            if(cardData.useLevels)level++;
            CalculateNewPrice();
            UpdateUI();
            OnBought?.Invoke();
            farmManager.OnUpgradeBought(upgradeIDGlobal,isWhatDatatype,currencyType);
            // upgradeApplier.ApplyUpgrade();
        }
    }


    private void CalculateNewPrice(){
        for (int i = 0; i < cardInfoContents.Count; i++)
        {
            AlphabeticNotation result = cardInfoContents[i].rawPrice * cardData.cardDataInfo[i].priceMultiplier;
            cardInfoContents[i].price_text.text = result.ToStringSmart(1);
            cardInfoContents[i].rawPrice = result;
        }
    }

        private void UpdateUI(){
        if(level == maxLevel && cardData.useLevels){
        level_txt.text ="Lv. "+ level.ToString() + " MAX";
        foreach(CardInfoContent info in cardInfoContents){
            info.price_text.text = "";
            info.price_img = null;
            info.price_parent_obj.SetActive(false);
        }
        }else if(cardData.useLevels){
        level_txt.text ="Lv. "+ level.ToString();
        }
        if(!cardData.useLevels) HideLevel();

    }
void Update()
{
    if (Input.GetMouseButtonDown(0))
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

}
