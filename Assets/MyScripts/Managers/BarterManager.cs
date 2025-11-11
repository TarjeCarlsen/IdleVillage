using System;
using System.Collections.Generic;
using LargeNumbers;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.Analytics;

public enum Merchants
{
    BobTheMerchant,
    CarlTheMerchant,
    ChloeTheMerchant,
    FredTheMerchant,
    SamTheMerchant,
    RogerTheMerchant,
}

[System.Serializable]
public struct BarterCurrencyValues
{
    public CurrencyTypes currencyType;
    public Sprite currencySprite;
    public float defaultCurrencyValue;
}
public class BarterManager : MonoBehaviour
{
    public List<BarterCurrencyValues> barterCurrencyValues;
    public Dictionary<Merchants, MerchantInfo> merchantInfos;
    public Dictionary<Merchants, MerchantBonuses> merchantBonuses;
    [SerializeField] private List<MerchantInfo> startValues;
    [SerializeField] private List<MerchantBonuses> bonusesStartValues;

    [SerializeField] private GameObject barterOfferPrefab;
    [SerializeField] private Transform barterParentContainer;
    [SerializeField] private List<BarterCardHandler> barterCardHandlers;

    [SerializeField] private float growthRate; // tweak this to adjust scaling for how fast lvl requirement xp increases
    [SerializeField] private int maxAmountOfBarters;

    public event Action<Merchants> OnBarterLevelUp;
    public event Action<Merchants> OnBarterXpGain;

    [System.Serializable]
    public class MerchantInfo
    {
        public Sprite merchantIcon_sprite;
        public float bonus;
        public int skillPoints;
        public int merchantLevel;
        public float merchantXp;
        public float requiredXp;
    }
    [System.Serializable]
    public class MerchantBonuses
    {
        public Dictionary<CurrencyTypes, AlphabeticNotation> rewardMultiplier = new();
        public Dictionary<CurrencyTypes, AlphabeticNotation> rewardBaseFlatIncrease = new();
        public void InitializeDefaults()
        {
            foreach (CurrencyTypes type in Enum.GetValues(typeof(CurrencyTypes)))
            {
                rewardMultiplier[type] = UpgradeManager.Instance.GetMerchantPower(MerchantUpgradeTypes.rewardBonusMulti); // 1 means no multiplier (neutral)
                rewardBaseFlatIncrease[type] = UpgradeManager.Instance.GetMerchantPower(MerchantUpgradeTypes.rewardBonusFlat); // 0 means no flat bonus
            }
        }
    }

    private void Awake()
    {
        InitializeMerchantInfos();
        InitializeMerchantBonuses();
        for (int i = 0; i < maxAmountOfBarters; i++)
        {
            CreateBarterOffers();
        }
    }

    private void InitializeMerchantInfos()
    {
        merchantInfos = new Dictionary<Merchants, MerchantInfo>();
        foreach (Merchants merchant in Enum.GetValues(typeof(Merchants)))
        {
            MerchantInfo info = startValues[(int)merchant];
            merchantInfos.Add(merchant, info);
        }
    }

    private void InitializeMerchantBonuses()
    {
        merchantBonuses = new Dictionary<Merchants, MerchantBonuses>();

        foreach (Merchants merchant in Enum.GetValues(typeof(Merchants)))
        {
            MerchantBonuses info = new MerchantBonuses();
            info.InitializeDefaults();
            merchantBonuses.Add(merchant, info);
        }
    }

    public void BarterOfferBought(Merchants merchant, float xpAmount)
    {
        merchantInfos[merchant].merchantXp += xpAmount;
        if (merchantInfos[merchant].merchantXp >= merchantInfos[merchant].requiredXp)
        {
            float overflowXp = merchantInfos[merchant].merchantXp - merchantInfos[merchant].requiredXp;
            MerchantLevelUp(merchant, overflowXp);
        }
        OnBarterXpGain?.Invoke(merchant);
    }

    public void UpgradeBought(Merchants merchant, CurrencyTypes types){ // SETS THE UPGRADE FOR EACH OF THE MERCHANTS
        switch(merchant){
            case Merchants.BobTheMerchant:
                merchantBonuses[merchant].rewardBaseFlatIncrease[types] = MerchantUpgradeManager.Instance.BobGetRewardPower(BobUpgradeTypes.rewardFlatBob);
                merchantBonuses[merchant].rewardMultiplier[types] = MerchantUpgradeManager.Instance.BobGetRewardPower(BobUpgradeTypes.rewardMultiBob);
                print("FLAT BOB = "+merchantBonuses[merchant].rewardBaseFlatIncrease[types]);
                print("MULTI BOB = "+merchantBonuses[merchant].rewardMultiplier[types]);
            break;
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


        foreach(Merchants merchants in Enum.GetValues(typeof(Merchants))){
            print("upgrade amount "+ UpgradeManager.Instance.GetMerchantPower(MerchantUpgradeTypes.rewardBonusFlat));
            print($"merchant: {merchants} flat bonus = {merchantBonuses[merchants].rewardBaseFlatIncrease[CurrencyTypes.wheat]}");
        }
    }
    public void MerchantLevelUp(Merchants merchant, float xpAmount)
    {
        float nextRequiredXp = startValues[(int)merchant].requiredXp * Mathf.Pow(growthRate, merchantInfos[merchant].merchantLevel);
        merchantInfos[merchant].requiredXp = nextRequiredXp;
        merchantInfos[merchant].merchantXp = xpAmount;
        merchantInfos[merchant].merchantLevel += 1;
        merchantInfos[merchant].bonus += .05f;
        OnBarterLevelUp?.Invoke(merchant);
    }

    public void OnRefreshClick()
    {
        foreach (var card in barterCardHandlers)
        {
            if (card != null)
            {
                card.DestroyCard();
            }
        }
        barterCardHandlers.Clear();

        for (int i = 0; i < maxAmountOfBarters; i++)
        {
            CreateBarterOffers();
        }
    }
    private void CreateBarterOffers()
    {
        GameObject newBarterOffer = Instantiate(barterOfferPrefab, barterParentContainer);
        BarterCardHandler barterHandler = newBarterOffer.GetComponent<BarterCardHandler>();
        barterCardHandlers.Add(barterHandler);
    }

}
