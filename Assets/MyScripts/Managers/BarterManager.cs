using System;
using System.Collections.Generic;
using LargeNumbers;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public enum Merchants{
    BobTheMerchant,
    CarlTheMerchant,
    ChloeTheMerchant,
    FredTheMerchant,
    SamTheMerchant,
    RogerTheMerchant,
}

[System.Serializable    ]
public struct BarterCurrencyValues{
    public CurrencyTypes currencyType;
    public Sprite currencySprite;
    public float defaultCurrencyValue;
}

public class BarterManager : MonoBehaviour
{   
    public List<BarterCurrencyValues> barterCurrencyValues;
    public Dictionary<Merchants, MerchantInfo> merchantInfos;
    [SerializeField] private List<MerchantInfo> startValues;

    [SerializeField] private GameObject barterOfferPrefab;
    [SerializeField] private Transform barterParentContainer;
    [SerializeField] private List<BarterCardHandler> barterCardHandlers;

    [SerializeField] private float growthRate; // tweak this to adjust scaling for how fast lvl requirement xp increases
    [SerializeField] private int maxAmountOfBarters;

    public event Action<Merchants> OnBarterLevelUp;
    public event Action<Merchants> OnBarterXpGain;

    [System.Serializable]
    public class MerchantInfo{
        public Sprite merchantIcon_sprite;
        public int merchantLevel;
        public float bonus;
        public float merchantXp;
        public float requiredXp;
    }

    private void Awake(){
        merchantInfos = new Dictionary<Merchants, MerchantInfo>();
        foreach(Merchants merchant in Enum.GetValues(typeof(Merchants))){
            MerchantInfo info = startValues[(int)merchant];
            merchantInfos.Add(merchant, info);
        }
        for (int i = 0; i < maxAmountOfBarters; i++)
        {
            CreateBarterOffers();
        }
    }

    public void BarterOfferBought(Merchants merchant,float xpAmount){
            merchantInfos[merchant].merchantXp += xpAmount;
            if(merchantInfos[merchant].merchantXp >= merchantInfos[merchant].requiredXp){
                float overflowXp = merchantInfos[merchant].merchantXp - merchantInfos[merchant].requiredXp;
                MerchantLevelUp(merchant, overflowXp);
            }
                OnBarterXpGain?.Invoke(merchant);
    }

    public void MerchantLevelUp(Merchants merchant, float xpAmount){
        float nextRequiredXp = startValues[(int)merchant].requiredXp * Mathf.Pow(growthRate, merchantInfos[merchant].merchantLevel);
        merchantInfos[merchant].requiredXp = nextRequiredXp;
        merchantInfos[merchant].merchantXp = xpAmount;
        merchantInfos[merchant].merchantLevel += 1;
        merchantInfos[merchant].bonus += .05f;
        OnBarterLevelUp?.Invoke(merchant);
    }

    public void OnRefreshClick(){
        foreach(var card in barterCardHandlers){
            if(card != null){
                card.DestroyCard();
            }
        }
        barterCardHandlers.Clear();

        for (int i = 0; i < maxAmountOfBarters; i++)
        {
        CreateBarterOffers();
        }
    }
    private void CreateBarterOffers(){
        GameObject newBarterOffer = Instantiate(barterOfferPrefab,barterParentContainer);
        BarterCardHandler barterHandler = newBarterOffer.GetComponent<BarterCardHandler>();
        barterCardHandlers.Add(barterHandler);
    }
    
}
