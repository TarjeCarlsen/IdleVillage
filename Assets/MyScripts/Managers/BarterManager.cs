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

    [SerializeField] private float growthRate;

    [System.Serializable]
    public class MerchantInfo{
        public Sprite merchantIcon_sprite;
        public int merchantLevel;
        public float merchantXp;
        public float requiredXp;
    }

    private void Awake(){
        merchantInfos = new Dictionary<Merchants, MerchantInfo>();
        foreach(Merchants merchant in Enum.GetValues(typeof(Merchants))){
            MerchantInfo info = startValues[(int)merchant];
            merchantInfos.Add(merchant, info);
        }
    }



    public void BarterOfferBought(Merchants merchant,float xpAmount){
        // print($"merchant - {merchant} xpIncrease - {xpAmount}");
            merchantInfos[merchant].merchantXp += xpAmount;
                                //1500                          //1000
            if(merchantInfos[merchant].merchantXp >= merchantInfos[merchant].requiredXp){
                float overflowXp = merchantInfos[merchant].merchantXp - merchantInfos[merchant].requiredXp;
                MerchantLevelUp(merchant, overflowXp);
            }
    }

    public void MerchantLevelUp(Merchants merchant, float xpAmount){
        float nextRequiredXp = startValues[(int)merchant].requiredXp * Mathf.Pow(growthRate, merchantInfos[merchant].merchantLevel);
        merchantInfos[merchant].requiredXp = nextRequiredXp;
        merchantInfos[merchant].merchantXp = xpAmount;
        merchantInfos[merchant].merchantLevel += 1;

    }
    
}
