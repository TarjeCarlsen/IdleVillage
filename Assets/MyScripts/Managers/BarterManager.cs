using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
    public int currencyTypeRewardWeigth;
    public int currencyTypePriceWeigth;
}
public class BarterManager : MonoBehaviour
{
    public List<BarterCurrencyValues> barterCurrencyValues;
    public Dictionary<Merchants, MerchantInfo> merchantInfos;
    public Dictionary<Merchants, MerchantBonuses> merchantBonuses;
    [SerializeField] private List<MerchantInfo> startValues;

    [SerializeField] private GameObject barterOfferPrefab;
    [SerializeField] private GameObject specialBarterOfferPrefab;

    [SerializeField] private GameObject TESTING_barterOfferPrefab;
    [SerializeField] private Transform barterParentContainer;
    private List<BarterCardHandler> barterCardHandlers = new();

    [SerializeField] private float growthRate; // tweak this to adjust scaling for how fast lvl requirement xp increases
    [SerializeField] private int maxAmountOfBarters;

    public Merchants previousMerchantCompleted; // STOPPED HERE. NEED TO IMPLEMENT A PREVIOUS MERCHANT POINTER TO CHECK IF THE
                                                // CLAIMED BARTER IS GOING TO ADD TO THE COMPLETED IN A ROW COUNTER OR RESET IT 
                                                // AND SET PREVIOUS MERCHANT COMPLETED TO THIS ONE

    public event Action<Merchants> OnBarterLevelUp;
    public event Action<Merchants> OnBarterXpGain;
    public event Action<Merchants> OnUpgradeBought;
    public event Action<Merchants> OnBarterClaimed;
    public event Action<Merchants> OnXpUpgradeBought;

    [System.Serializable]
    public class MerchantInfo
    {
        public string merchantName; // name is just for testing, not used anywhere important
        public Sprite merchantIcon_sprite;
        public float bonus;
        public int skillPoints;
        public int merchantLevel;
        public float merchantXp;
        public float requiredXp;
        public int appearChanceWeigth; // To increase chance of a merchant appearing increase weight by +1, +2, +3...
                                       // depending on how big slice of the wheel he should take
        public int completedBartersForMerchant = 0;
        public int completedInArow = 0;



        public Dictionary<CurrencyTypes, int> rewardCurrencyWeigth;

        public void InitializeRewardWeights()
        {
            rewardCurrencyWeigth = new Dictionary<CurrencyTypes, int>();
            foreach (CurrencyTypes type in Enum.GetValues(typeof(CurrencyTypes)))
            {
                if (type == CurrencyTypes.wheat)
                {
                    rewardCurrencyWeigth[type] = 1; // TESTING, REMOVE WHEN DONE!
                }
                else
                {
                    rewardCurrencyWeigth[type] = 1; // or whatever default weight you want
                }
            }
        }
    }
    [System.Serializable]
    public class MerchantBonuses
    {
        public Dictionary<CurrencyTypes, AlphabeticNotation> rewardMultiplierBonus = new();
        public Dictionary<CurrencyTypes, AlphabeticNotation> rewardBaseFlatIncreaseBonus = new();
        public Dictionary<CurrencyTypes, AlphabeticNotation> totalBonus = new();
        public Dictionary<CurrencyTypes, int> rewardCurrencyWeigthBonus = new();
        public AlphabeticNotation stackingMulit = new AlphabeticNotation(1);
        public float xpRewardBonus = 1f;
        public float specialBarterOfferMulti = 100;
        public float specialBarterChance = 0f;
        public void InitializeDefaults()
        {
            foreach (CurrencyTypes type in Enum.GetValues(typeof(CurrencyTypes)))
            {
                rewardMultiplierBonus[type] = new AlphabeticNotation(1); // 1 means no multiplier (neutral)
                rewardBaseFlatIncreaseBonus[type] = new AlphabeticNotation(0); // 0 means no flat bonus
                totalBonus[type] = rewardMultiplierBonus[type] * rewardBaseFlatIncreaseBonus[type];
                rewardCurrencyWeigthBonus[type] = 0;
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
            info.InitializeRewardWeights();
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

    public void UpgradeBought(Merchants merchant, CurrencyTypes types, UpgradeTypes upgradeType)
    { // SETS THE UPGRADE FOR EACH OF THE MERCHANTS. Set specific upgrades here
        switch (merchant)
        {

            case Merchants.BobTheMerchant:
                switch (upgradeType)
                {
                    case UpgradeTypes.BobAddFlat:
                        print("UPGRADED FLAT FOR BOB!");
                        merchantBonuses[merchant].rewardBaseFlatIncreaseBonus[types] = MerchantUpgradeManager.Instance.BobGetRewardPower(BobUpgradeTypes.rewardFlatBob) +
                                                                                       MerchantUpgradeManager.Instance.BobGetRewardPower(BobUpgradeTypes.rewardFlatBob_2);
                        break;

                    case UpgradeTypes.BobAddFlatToMulti:
                        print("UPGRADED FLAT TO MULT FOR BOB!");
                        merchantBonuses[merchant].rewardMultiplierBonus[types] = MerchantUpgradeManager.Instance.BobGetRewardPower(BobUpgradeTypes.rewardMultiBob);
                        break;

                    case UpgradeTypes.BobIncreaseChanceForMoneyReward:
                        print("UPGRADED CHANCE FOR MONEY REWARD BOB!");
                        merchantBonuses[merchant].rewardCurrencyWeigthBonus[types] = MerchantUpgradeManager.Instance.BobGetRewardPowerInt(BobUpgradeTypesInt.moneyWeightChanceBob);
                        break;

                    case UpgradeTypes.BobUpgradeXpForAllMerchants:
                        print("UPGRADED XP FOR ALL FROM BOB!");
                        foreach (Merchants merch in Enum.GetValues(typeof(Merchants)))
                        {
                            UpdateAllMerchantsXpGain(merch, MerchantUpgradeManager.Instance.BobGetRewardPowerFloat(BobUpgradeTypesFloats.increaseAllXpBonusMulti));
                            OnUpgradeBought?.Invoke(merch);
                        }
                        // merchantBonuses[merchant].xpRewardBonus = MerchantUpgradeManager.Instance.BobGetRewardPowerFloat(BobUpgradeTypesFloats.xpGainBonusMulti);
                        break;

                    case UpgradeTypes.BobStackMultiFlat_ResetOnOtherBarter:
                        print("UPGRADED STACKING MULT ON BOB!");
                        merchantBonuses[merchant].stackingMulit = MerchantUpgradeManager.Instance.BobGetRewardPower(BobUpgradeTypes.multiAll_resetOnOther);
                        break;

                    case UpgradeTypes.BobIncreaseAllChanceForSpecial:
                        print("UPGRADED CHANCE FOR SPECIAL FOR ALL FROM BOB!");
                        foreach (Merchants merch in Enum.GetValues(typeof(Merchants)))
                        {
                            UpdateAllSpecialBarterChances(merch, MerchantUpgradeManager.Instance.BobGetRewardPowerFloat(BobUpgradeTypesFloats.increaseAllSpecialBarterChance));
                            OnUpgradeBought?.Invoke(merch);
                        }
                        break;
                }
                break;
            case Merchants.CarlTheMerchant:
                switch (upgradeType)
                {
                    case UpgradeTypes.CarlAddFlat:
                        print("UPGRADED FLAT FOR Carl!");
                        merchantBonuses[merchant].rewardBaseFlatIncreaseBonus[types] = MerchantUpgradeManager.Instance.CarlGetRewardPower(CarlUpgradeTypes.rewardFlatCarl);
                        break;

                    case UpgradeTypes.CarlAddFlatToMulti:
                        print("UPGRADED FLAT TO MULT FOR Carl!");
                        merchantBonuses[merchant].rewardMultiplierBonus[types] = MerchantUpgradeManager.Instance.CarlGetRewardPower(CarlUpgradeTypes.rewardMultiCarl);
                        break;

                }
                break;
            case Merchants.ChloeTheMerchant:
                switch (upgradeType)
                {
                    case UpgradeTypes.ChloeAddFlat:
                        print("UPGRADED FLAT FOR Chloe!");
                        merchantBonuses[merchant].rewardBaseFlatIncreaseBonus[types] = MerchantUpgradeManager.Instance.ChloeGetRewardPower(ChloeUpgradeTypes.rewardFlatChloe);
                        break;

                    case UpgradeTypes.ChloeAddFlatToMulti:
                        print("UPGRADED FLAT TO MULT FOR Chloe!");
                        merchantBonuses[merchant].rewardMultiplierBonus[types] = MerchantUpgradeManager.Instance.ChloeGetRewardPower(ChloeUpgradeTypes.rewardMultiChloe);
                        break;
                }
                break;
            case Merchants.FredTheMerchant:
                switch (upgradeType)
                {
                    case UpgradeTypes.FredAddFlat:
                        print("UPGRADED FLAT FOR FRED!");
                        merchantBonuses[merchant].rewardBaseFlatIncreaseBonus[types] = MerchantUpgradeManager.Instance.FredGetRewardPower(FredUpgradeTypes.rewardFlatFred);

                        break;
                    case UpgradeTypes.FredAddFlatToMulti:
                        print("UPGRADED FLAT TO MULT FOR FRED!");
                        merchantBonuses[merchant].rewardMultiplierBonus[types] = MerchantUpgradeManager.Instance.FredGetRewardPower(FredUpgradeTypes.rewardMultiFred);
                        break;
                }
                break;
            case Merchants.SamTheMerchant:
                switch (upgradeType)
                {
                    case UpgradeTypes.SamAddFlat:
                        print("UPGRADED FLAT FOR SAM!");
                        merchantBonuses[merchant].rewardBaseFlatIncreaseBonus[types] = MerchantUpgradeManager.Instance.SamGetRewardPower(SamUpgradeTypes.rewardFlatSam);

                        break;

                    case UpgradeTypes.SamAddFlatToMulti:
                        print("UPGRADED FLAT TO MULT FOR SAM!");
                        merchantBonuses[merchant].rewardMultiplierBonus[types] = MerchantUpgradeManager.Instance.SamGetRewardPower(SamUpgradeTypes.rewardMultiSam);
                        break;
                }
                break;
            case Merchants.RogerTheMerchant:
                switch (upgradeType)
                {
                    case UpgradeTypes.RogerAddFlat:
                        print("UPGRADED FLAT FOR BOB!");
                        merchantBonuses[merchant].rewardBaseFlatIncreaseBonus[types] = MerchantUpgradeManager.Instance.RogerGetRewardPower(RogerUpgradeTypes.rewardFlatRoger);
                        break;
                    case UpgradeTypes.RogerAddFlatToMulti:
                        print("UPGRADED FLAT TO MULT FOR SAM!");
                        merchantBonuses[merchant].rewardMultiplierBonus[types] = MerchantUpgradeManager.Instance.RogerGetRewardPower(RogerUpgradeTypes.rewardMultiRoger);
                        break;

                }
                break;
        }
        OnUpgradeBought?.Invoke(merchant);
    }


    private void UpdateAllMerchantsXpGain(Merchants merchant, float amount)
    {
        merchantBonuses[merchant].xpRewardBonus = amount; // RIGHT NOW SETS IT STATICALLY. HAVE TO ADD TO THE ORIGINAL AMOUNT WHEN MORE 
                                                          // MERCHANTS THEN BOB AFFECT XP RATES
    }

    private void UpdateAllSpecialBarterChances(Merchants merchant, float amount)
    {
        merchantBonuses[merchant].specialBarterChance = amount; // RIGHT NOW SETS IT STATICALLY. HAVE TO ADD TO THE ORIGINAL AMOUNT WHEN MORE 
                                                                // MERCHANTS THEN BOB AFFECT SPECIAL BARTER TRADE CHANCES

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

    public void TESTING_CREATE_TESTINGBARTER()
    {
        GameObject newBarterOffer = Instantiate(TESTING_barterOfferPrefab, barterParentContainer);
        BarterCardHandler barterHandler = newBarterOffer.GetComponent<BarterCardHandler>();
        barterCardHandlers.Add(barterHandler);
        barterHandler.OnBarterClaimed += ForwardEventRaised;
    }

    private int GetRandomMerchant()
    {

        int totalWeigth = 0;
        foreach (var segment in merchantInfos.Values)
        {
            totalWeigth += segment.appearChanceWeigth;
        }
        int randomValue = UnityEngine.Random.Range(0, totalWeigth);
        int currentSum = 0;
        int currentIndex = 0;
        foreach (var segment in merchantInfos.Values)
        {
            currentSum += segment.appearChanceWeigth;
            if (randomValue < currentSum)
            {
                return currentIndex;
            }
            currentIndex++;
        }

        // Safety fallback
        return 0;
    }

    private bool isBarterSpecial(int chosenMerchant)
    {
        float chance = merchantBonuses[(Merchants)chosenMerchant].specialBarterChance;
        float roll = UnityEngine.Random.Range(0f, 1f);
        if (roll < chance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private void CreateBarterOffers()
    {
        int randomChosenMerchant = GetRandomMerchant();
        GameObject newBarterOffer;
        BarterCardHandler barterHandler;

        if (isBarterSpecial(randomChosenMerchant))
        {
            newBarterOffer = Instantiate(specialBarterOfferPrefab, barterParentContainer);

        }
        else
        {
            newBarterOffer = Instantiate(barterOfferPrefab, barterParentContainer);
        }

        barterHandler = newBarterOffer.GetComponent<BarterCardHandler>();
        barterHandler.chosenMerchantIndex = randomChosenMerchant;
        barterCardHandlers.Add(barterHandler);
        barterHandler.OnBarterClaimed += ForwardEventRaised;
    }

    public void UnsubscribeFromCard(BarterCardHandler handler)
    {
        handler.OnBarterClaimed -= ForwardEventRaised;
    }

    private void ForwardEventRaised(Merchants _merchant)
    {
        OnBarterClaimed?.Invoke(_merchant);
    }

    public bool isMerchantSameAsLast(Merchants _merchant)
    {
        if (previousMerchantCompleted != _merchant)
        {
            previousMerchantCompleted = _merchant;
            foreach (Merchants merchant in Enum.GetValues(typeof(Merchants)))
            {
                merchantInfos[merchant].completedInArow = 0;
                OnBarterClaimed?.Invoke(merchant);
            }
            return false;
        }
        return true;


    }

}
