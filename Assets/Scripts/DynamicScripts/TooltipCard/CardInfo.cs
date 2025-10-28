using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CardInfo : MonoBehaviour
{
    [SerializeField] CardData cardData;
    [SerializeField] Image content_img;
    [SerializeField] TMP_Text header_txt;
    [SerializeField] TMP_Text description_txt;
    [SerializeField] TMP_Text level_txt;
    [SerializeField] private List<CardInfoContent>  cardInfoContents;
    [SerializeField] private HouseManager houseManager;
    public Action OnBought;
    private void HideLevel() => level_txt.text = "";
    private int level;
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
    }

        private void Awake(){
        houseManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<HouseManager>();;
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
            cardInfoContents[i].price_text.text = cardData.cardDataInfo[i].price;
            cardInfoContents[i].price_img.sprite = cardData.cardDataInfo[i].sprite;
            cardInfoContents[i].types = cardData.cardDataInfo[i].type;
        }
    }
    if(!cardData.useLevels) HideLevel();
    isInitialized = true;
}

    public bool CanAfford(){
        foreach(CardInfoContent price in cardInfoContents){
            print($"{BigNumber.Parse(price.price_text.text)} > {MoneyManager.Instance.GetCurrency(price.types)}");
            if (BigNumber.Parse(price.price_text.text) > MoneyManager.Instance.GetCurrency(price.types)){
                return false;
            }
        }
        return true;
    }

    public void OnBuyClick(){
        print("inside click");
        if(level == maxLevel && cardData.useLevels){
            return;
        }
        if(CanAfford()){
        print("inside afford");
            foreach(CardInfoContent price in cardInfoContents){
                MoneyManager.Instance.SubtractCurrency(price.types, BigNumber.Parse(price.price_text.text));
            }
            if(cardData.useLevels)level++;
            CalculateNewPrice();
            UpdateUI();
            OnBought?.Invoke();
            // upgradeApplier.ApplyUpgrade();
        }
    }


    private void CalculateNewPrice(){
        for (int i = 0; i < cardInfoContents.Count; i++)
        {
            BigNumber result = double.Parse(cardInfoContents[i].price_text.text) * cardData.cardDataInfo[i].priceMultiplier;
            cardInfoContents[i].price_text.text = result.ToString();
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
        // 🧠 First: if you clicked on *any UI element*, we check if it's part of this card
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            // Get the GameObject currently under the pointer
            var pointer = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;

            if (pointer != null)
            {
                // ✅ If it’s part of this card’s hierarchy — do nothing
                if (pointer.transform.IsChildOf(transform))
                    return;
            }
        }

        // 🧭 Second: if it’s not over any of this card’s elements, we check its rect
        if (!RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition, uiCamera))
        {
            // ✅ Clicked truly outside — hide the card
            gameObject.SetActive(false);
        }
    }
}
}
