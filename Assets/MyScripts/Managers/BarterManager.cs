using System;
using System.Collections.Generic;
using LargeNumbers;
using TMPro;
using UnityEngine;
using System.Collections;

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
    [SerializeField] private GameObject timedBarterOfferPrefab;

    [SerializeField] private GameObject TESTING_barterOfferPrefab;
    [SerializeField] private Transform barterParentContainer;
    private List<BarterCardHandler> barterCardHandlers = new();

    [SerializeField] private TMP_Text refreshAmount_txt;
    [SerializeField] private TMP_Text refreshTimer_txt;
    private Coroutine refreshCoroutine;
    [SerializeField] private int refreshAmount = 1;
    [SerializeField] private int maxAmountRefresh = 10;
    [SerializeField] private float refreshTimerStart = 5f;
    [SerializeField] private float chanceForTimedOffer = 0.05f;
    private float refreshTimer;
    public float freeRefreshChance = 0;

    [SerializeField] private float growthRate; // tweak this to adjust scaling for how fast lvl requirement xp increases
    [SerializeField] private int maxAmountOfBarters;

    [SerializeField] private bool isTesting;
    public Merchants previousMerchantCompleted; 


    public event Action<Merchants> OnBarterLevelUp;
    public event Action<Merchants> OnBarterXpGain;
    public event Action<Merchants> OnUpgradeBought;
    public event Action<Merchants> OnBarterClaimed;
    public event Action<Merchants,int> OnFavorGained;


    [System.Serializable]
    public class MerchantInfo
    {
        public string merchantName; // name is just for testing, not used anywhere important
        public Sprite merchantIcon_sprite;
        public int favor;
        public int skillPoints;
        public int merchantLevel;
        public float merchantXp;
        public float requiredXp;
        public float timedBarterChance;
        public int appearChanceWeigth = 10; // To increase chance of a merchant appearing increase weight by +1, +2, +3...
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
        public Dictionary<CurrencyTypes, float> giveCurrencyOnBarterCompletion = new();
        public List<CurrencyTypes> giveCurrencies = new();
        public AlphabeticNotation stackingMulit = new AlphabeticNotation(1);
        public float xpRewardBonus = 1f;
        public float specialBarterOfferMulti = 100;
        public float specialBarterChanceBonus = 0f;
        public int refreshAditionBonus = 0;
        public float freeRefreshChanceBonus = 0;
        public float reducedRefreshTimeBonus = 0;
        public float chanceToNotConsumeClaimBonus = 0f;
        public float priceMultiplier = 1f;
        public float favorMultiBonus= 1f;

        public void InitializeDefaults()
        {
            foreach (CurrencyTypes type in Enum.GetValues(typeof(CurrencyTypes)))
            {
                rewardMultiplierBonus[type] = new AlphabeticNotation(1); // 1 means no multiplier (neutral)
                rewardBaseFlatIncreaseBonus[type] = new AlphabeticNotation(0); // 0 means no flat bonus
                giveCurrencyOnBarterCompletion[type] = 0f;
                totalBonus[type] = rewardMultiplierBonus[type] * rewardBaseFlatIncreaseBonus[type];
                rewardCurrencyWeigthBonus[type] = 0;
            }
        }
    }

    private void Awake()
    {
        InitializeMerchantInfos();
        InitializeMerchantBonuses();
        foreach(Merchants merchant in Enum.GetValues(typeof(Merchants))){
            merchantInfos[merchant].timedBarterChance = chanceForTimedOffer;
        }

        for (int i = 0; i < maxAmountOfBarters; i++)
        {
            CreateBarterOffers();
        }
    }

    private void Start()
    {

        StartRefreshTimer();
        if(isTesting){
            foreach(Merchants _merchant in Enum.GetValues(typeof(Merchants))){
                merchantInfos[_merchant].favor = 300;
                ForwardEventRaisedGainFavor(_merchant, 0);
            }
        }
        UpdateUI();
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
        while (merchantInfos[merchant].merchantXp >= merchantInfos[merchant].requiredXp)
        {
            float overflowXp = merchantInfos[merchant].merchantXp - merchantInfos[merchant].requiredXp;
            MerchantLevelUp(merchant, overflowXp);
        }
        OnBarterXpGain?.Invoke(merchant);
    }

    public void UpgradeBought(Merchants merchant, CurrencyTypes types, MerchantUpgradeTypes merchantUpgradeType)
    { // SETS THE UPGRADE FOR EACH OF THE MERCHANTS. Set specific upgrades here
            print($"merchant {merchant} - currencytype {types} - upgradetype {merchantUpgradeType}");
        switch (merchant)
        {
            case Merchants.BobTheMerchant:
                switch (merchantUpgradeType)
                {
                    case MerchantUpgradeTypes.BobAddFlat:
                        print("UPGRADED FLAT FOR BOB!");
                        merchantBonuses[merchant].rewardBaseFlatIncreaseBonus[types] = MerchantUpgradeManager.Instance.BobGetRewardPower(BobUpgradeTypes.rewardFlatBob) +
                                                                                       MerchantUpgradeManager.Instance.BobGetRewardPower(BobUpgradeTypes.rewardFlatBob_2);
                        break;

                    case MerchantUpgradeTypes.BobAddFlatToMulti:
                        print("UPGRADED FLAT TO MULT FOR BOB!");
                        merchantBonuses[merchant].rewardMultiplierBonus[types] = MerchantUpgradeManager.Instance.BobGetRewardPower(BobUpgradeTypes.rewardMultiBob);
                        break;

                    case MerchantUpgradeTypes.BobIncreaseChanceForMoneyReward:
                        print("UPGRADED CHANCE FOR MONEY REWARD BOB!");
                        merchantBonuses[merchant].rewardCurrencyWeigthBonus[types] = MerchantUpgradeManager.Instance.BobGetRewardPowerInt(BobUpgradeTypesInt.moneyWeightChanceBob);
                        break;

                    case MerchantUpgradeTypes.BobUpgradeXpForAllMerchants:
                        print("UPGRADED XP FOR ALL FROM BOB!");
                        merchantBonuses[Merchants.BobTheMerchant].xpRewardBonus = MerchantUpgradeManager.Instance.BobGetRewardPowerFloat(BobUpgradeTypesFloats.xpGainBonusMulti);
                        merchantBonuses[Merchants.CarlTheMerchant].xpRewardBonus = MerchantUpgradeManager.Instance.CarlGetRewardPowerFloat(CarlUpgradeTypesFloats.xpGainBonusMulti);
                        merchantBonuses[Merchants.ChloeTheMerchant].xpRewardBonus = MerchantUpgradeManager.Instance.ChloeGetRewardPowerFloat(ChloeUpgradeTypesFloats.xpGainBonusMulti);
                        merchantBonuses[Merchants.FredTheMerchant].xpRewardBonus = MerchantUpgradeManager.Instance.FredGetRewardPowerFloat(FredUpgradeTypesFloats.xpGainBonusMulti);
                        merchantBonuses[Merchants.SamTheMerchant].xpRewardBonus = MerchantUpgradeManager.Instance.SamGetRewardPowerFloat(SamUpgradeTypesFloats.xpGainBonusMulti);
                        merchantBonuses[Merchants.RogerTheMerchant].xpRewardBonus = MerchantUpgradeManager.Instance.RogerGetRewardPowerFloat(RogerUpgradeTypesFloats.xpGainBonusMulti);
                        foreach (Merchants merch in Enum.GetValues(typeof(Merchants)))
                        {
                            print($"testing xp gain for {merch} = {merchantBonuses[merch].xpRewardBonus}");
                            OnUpgradeBought?.Invoke(merch);
                        }
                        // merchantBonuses[merchant].xpRewardBonus = MerchantUpgradeManager.Instance.BobGetRewardPowerFloat(BobUpgradeTypesFloats.xpGainBonusMulti);
                        break;

                    case MerchantUpgradeTypes.BobStackMultiFlat_ResetOnOtherBarter:
                        print("UPGRADED STACKING MULT ON BOB!");
                        merchantBonuses[merchant].stackingMulit = MerchantUpgradeManager.Instance.BobGetRewardPower(BobUpgradeTypes.multiAll_resetOnOther);
                        break;

                    case MerchantUpgradeTypes.BobIncreaseAllChanceForSpecial:
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
                switch (merchantUpgradeType)
                {
                    case MerchantUpgradeTypes.CarlAddFlat:
                        print("UPGRADED FLAT FOR Carl!");
                        merchantBonuses[merchant].rewardBaseFlatIncreaseBonus[types] = MerchantUpgradeManager.Instance.CarlGetRewardPower(CarlUpgradeTypes.rewardFlatCarl);
                        break;

                    case MerchantUpgradeTypes.CarlAddFlatToMulti:
                        print("UPGRADED FLAT TO MULT FOR Carl!");
                        merchantBonuses[merchant].rewardMultiplierBonus[types] = MerchantUpgradeManager.Instance.CarlGetRewardPower(CarlUpgradeTypes.rewardMultiCarl);
                        break;
                    case MerchantUpgradeTypes.CarlAddRefreshCount:
                        print("UPGRADED REFRESH COUNT Carl!");
                        int new_carlAddRefreshBonus = MerchantUpgradeManager.Instance.CarlGetRewardPowerInt(CarlUpgradeTypesInt.refreshCountCarl);
                        maxAmountRefresh -= merchantBonuses[merchant].refreshAditionBonus; // Remove the old bonus to apply the new one

                        merchantBonuses[merchant].refreshAditionBonus = new_carlAddRefreshBonus;
                        maxAmountRefresh += merchantBonuses[merchant].refreshAditionBonus;

                        print("new max refresh = " + maxAmountRefresh);
                        StartRefreshTimer();
                        UpdateUI();
                        break;
                    case MerchantUpgradeTypes.CarlFreeRefreshChance:
                        print("UPGRADED FREE REFRESH CHANCE Carl!");
                        float new_carlAddFreeRefreshChance = MerchantUpgradeManager.Instance.CarlGetRewardPowerFloat(CarlUpgradeTypesFloats.chanceForFreeRefresh);
                        freeRefreshChance -= merchantBonuses[merchant].freeRefreshChanceBonus;

                        merchantBonuses[merchant].freeRefreshChanceBonus = new_carlAddFreeRefreshChance;
                        freeRefreshChance += merchantBonuses[merchant].freeRefreshChanceBonus;

                        print("new chance for free refresh = " + freeRefreshChance);

                        break;
                    case MerchantUpgradeTypes.CarlReduceRefreshTime:
                        print("UPGRADED FREE REFRESH CHANCE Carl!");
                        float new_carlReduceRefreshTime = MerchantUpgradeManager.Instance.CarlGetRewardPowerFloat(CarlUpgradeTypesFloats.reduceRefreshTime);
                        if(merchantBonuses[merchant].reducedRefreshTimeBonus > 0){
                            refreshTimerStart = refreshTimerStart / merchantBonuses[merchant].reducedRefreshTimeBonus; // add back the reduced time
                        }
                        print($"bonus: {MerchantUpgradeManager.Instance.CarlGetRewardPowerFloat(CarlUpgradeTypesFloats.reduceRefreshTime)}");
                        merchantBonuses[merchant].reducedRefreshTimeBonus = new_carlReduceRefreshTime;
                        refreshTimerStart = refreshTimerStart * merchantBonuses[merchant].reducedRefreshTimeBonus; //add back the improved bonus

                        StopRefreshTimer();
                        StartRefreshTimer();
                        print("new reduced time = " + refreshTimerStart);

                        break;
                    case MerchantUpgradeTypes.CarlChanceToNotConsumeClaim:
                        print("CLAIM FREE CHANCE Carl!");
                        foreach (Merchants merch in Enum.GetValues(typeof(Merchants)))
                        {
                            merchantBonuses[(Merchants)merch].chanceToNotConsumeClaimBonus = MerchantUpgradeManager.Instance.CarlGetRewardPowerFloat(CarlUpgradeTypesFloats.chanceNotConsumeOnClaim);
                        }
                        break;
                    case MerchantUpgradeTypes.CarlMultiPriceMultiXp:
                        print("MULTI PRICE MULTI REWARD Carl!");
                        merchantBonuses[Merchants.BobTheMerchant].xpRewardBonus = MerchantUpgradeManager.Instance.BobGetRewardPowerFloat(BobUpgradeTypesFloats.xpGainBonusMulti);
                        merchantBonuses[Merchants.CarlTheMerchant].xpRewardBonus = MerchantUpgradeManager.Instance.CarlGetRewardPowerFloat(CarlUpgradeTypesFloats.xpGainBonusMulti);
                        merchantBonuses[Merchants.ChloeTheMerchant].xpRewardBonus = MerchantUpgradeManager.Instance.ChloeGetRewardPowerFloat(ChloeUpgradeTypesFloats.xpGainBonusMulti);
                        merchantBonuses[Merchants.FredTheMerchant].xpRewardBonus = MerchantUpgradeManager.Instance.FredGetRewardPowerFloat(FredUpgradeTypesFloats.xpGainBonusMulti);
                        merchantBonuses[Merchants.SamTheMerchant].xpRewardBonus = MerchantUpgradeManager.Instance.SamGetRewardPowerFloat(SamUpgradeTypesFloats.xpGainBonusMulti);
                        merchantBonuses[Merchants.RogerTheMerchant].xpRewardBonus = MerchantUpgradeManager.Instance.RogerGetRewardPowerFloat(RogerUpgradeTypesFloats.xpGainBonusMulti);

                        merchantBonuses[Merchants.BobTheMerchant].priceMultiplier = MerchantUpgradeManager.Instance.BobGetRewardPowerFloat(BobUpgradeTypesFloats.priceMulti);
                        merchantBonuses[Merchants.CarlTheMerchant].priceMultiplier = MerchantUpgradeManager.Instance.CarlGetRewardPowerFloat(CarlUpgradeTypesFloats.priceMulti);
                        merchantBonuses[Merchants.ChloeTheMerchant].priceMultiplier = MerchantUpgradeManager.Instance.ChloeGetRewardPowerFloat(ChloeUpgradeTypesFloats.priceMulti);
                        merchantBonuses[Merchants.FredTheMerchant].priceMultiplier = MerchantUpgradeManager.Instance.FredGetRewardPowerFloat(FredUpgradeTypesFloats.priceMulti);
                        merchantBonuses[Merchants.SamTheMerchant].priceMultiplier = MerchantUpgradeManager.Instance.SamGetRewardPowerFloat(SamUpgradeTypesFloats.priceMulti);
                        merchantBonuses[Merchants.RogerTheMerchant].priceMultiplier = MerchantUpgradeManager.Instance.RogerGetRewardPowerFloat(RogerUpgradeTypesFloats.priceMulti);

                        
                        foreach (Merchants merch in Enum.GetValues(typeof(Merchants)))
                        {
                            print($"multies for  {merch}: xpreward = {merchantBonuses[merch].xpRewardBonus} price multi = {merchantBonuses[merch].priceMultiplier} ");
                            OnUpgradeBought?.Invoke(merch);
                        }
                        break;
                    case MerchantUpgradeTypes.CarlGiveWheatOnComplete:
                        print("GIVE WHEAT ON COMPLETE Carl!");
                        if(!merchantBonuses[merchant].giveCurrencies.Contains(CurrencyTypes.wheat)){
                            merchantBonuses[merchant].giveCurrencies.Add(CurrencyTypes.wheat);
                            print("adding wheat to list");
                        }
                        print("bonus = "+ MerchantUpgradeManager.Instance.CarlGetRewardPowerFloat(CarlUpgradeTypesFloats.giveWheatOnComplete));
                        merchantBonuses[merchant].giveCurrencyOnBarterCompletion[types] = MerchantUpgradeManager.Instance.CarlGetRewardPowerFloat(CarlUpgradeTypesFloats.giveWheatOnComplete);
                        break;

                }
                break;
                
            case Merchants.ChloeTheMerchant:
                switch (merchantUpgradeType)
                {
                    case MerchantUpgradeTypes.ChloeUpgradeXpForAllMerchants:
                        print("UPGRADED xp for all  Chloe!");
                        merchantBonuses[Merchants.BobTheMerchant].xpRewardBonus = MerchantUpgradeManager.Instance.BobGetRewardPowerFloat(BobUpgradeTypesFloats.xpGainBonusMulti);
                        merchantBonuses[Merchants.CarlTheMerchant].xpRewardBonus = MerchantUpgradeManager.Instance.CarlGetRewardPowerFloat(CarlUpgradeTypesFloats.xpGainBonusMulti);
                        merchantBonuses[Merchants.ChloeTheMerchant].xpRewardBonus = MerchantUpgradeManager.Instance.ChloeGetRewardPowerFloat(ChloeUpgradeTypesFloats.xpGainBonusMulti);
                        merchantBonuses[Merchants.FredTheMerchant].xpRewardBonus = MerchantUpgradeManager.Instance.FredGetRewardPowerFloat(FredUpgradeTypesFloats.xpGainBonusMulti);
                        merchantBonuses[Merchants.SamTheMerchant].xpRewardBonus = MerchantUpgradeManager.Instance.SamGetRewardPowerFloat(SamUpgradeTypesFloats.xpGainBonusMulti);
                        merchantBonuses[Merchants.RogerTheMerchant].xpRewardBonus = MerchantUpgradeManager.Instance.RogerGetRewardPowerFloat(RogerUpgradeTypesFloats.xpGainBonusMulti);
                        foreach (Merchants merch in Enum.GetValues(typeof(Merchants)))
                        {
                            print($"testing xp gain for {merch} = {merchantBonuses[merch].xpRewardBonus}");
                            OnUpgradeBought?.Invoke(merch);
                        }
                        break;

                    case MerchantUpgradeTypes.ChloeMultiAllOnFavorPassed:
                        print("UPGRADED multi all on favor passed Chloe!");
                        foreach (Merchants merch in Enum.GetValues(typeof(Merchants)))
                        {
                            OnUpgradeBought?.Invoke(merch);
                        }
                        break;
                    case MerchantUpgradeTypes.ChloeDoubleXpOnNextBarter:
                        print("UPGRADED double xp on next barter Chloe!");

                        break;
                }
                break;
            case Merchants.FredTheMerchant:
                switch (merchantUpgradeType)
                {
                    case MerchantUpgradeTypes.FredAddFlat:
                        print("UPGRADED FLAT FOR FRED!");
                        merchantBonuses[merchant].rewardBaseFlatIncreaseBonus[types] = MerchantUpgradeManager.Instance.FredGetRewardPower(FredUpgradeTypes.rewardFlatFred);

                        break;
                    case MerchantUpgradeTypes.FredAddFlatToMulti:
                        print("UPGRADED FLAT TO MULT FOR FRED!");
                        merchantBonuses[merchant].rewardMultiplierBonus[types] = MerchantUpgradeManager.Instance.FredGetRewardPower(FredUpgradeTypes.rewardMultiFred);
                        break;
                }
                break;
            case Merchants.SamTheMerchant:
                switch (merchantUpgradeType)
                {
                    case MerchantUpgradeTypes.SamAddFlat:
                        print("UPGRADED FLAT FOR SAM!");
                        merchantBonuses[merchant].rewardBaseFlatIncreaseBonus[types] = MerchantUpgradeManager.Instance.SamGetRewardPower(SamUpgradeTypes.rewardFlatSam);

                        break;

                    case MerchantUpgradeTypes.SamAddFlatToMulti:
                        print("UPGRADED FLAT TO MULT FOR SAM!");
                        merchantBonuses[merchant].rewardMultiplierBonus[types] = MerchantUpgradeManager.Instance.SamGetRewardPower(SamUpgradeTypes.rewardMultiSam);
                        break;
                }
                break;
            case Merchants.RogerTheMerchant:
                switch (merchantUpgradeType)
                {
                    case MerchantUpgradeTypes.RogerAddFlat:
                        print("UPGRADED FLAT FOR BOB!");
                        merchantBonuses[merchant].rewardBaseFlatIncreaseBonus[types] = MerchantUpgradeManager.Instance.RogerGetRewardPower(RogerUpgradeTypes.rewardFlatRoger);
                        break;
                    case MerchantUpgradeTypes.RogerAddFlatToMulti:
                        print("UPGRADED FLAT TO MULT FOR SAM!");
                        merchantBonuses[merchant].rewardMultiplierBonus[types] = MerchantUpgradeManager.Instance.RogerGetRewardPower(RogerUpgradeTypes.rewardMultiRoger);
                        break;

                }
                break;
        }
        print("INSIDE UPGRADES!");
        OnUpgradeBought?.Invoke(merchant);
    }


    private void UpdateAllMerchantsXpGain(Merchants merchant, float amount)
    {
                                            //1.05
    float oldMultiplier = merchantBonuses[merchant].xpRewardBonus;
                //  1.10 -      1.05          = 0.05
    float delta = amount - oldMultiplier;
                // 1.05 + 0.05
    merchantBonuses[merchant].xpRewardBonus += delta;
        print($"xp bonus for {merchant} = {merchantBonuses[merchant].xpRewardBonus}");
    }
    private void UpdateAllPriceMultipliers(Merchants merchant, float amount)
    {
    float oldMultiplier = merchantBonuses[merchant].priceMultiplier;

    float delta = amount - oldMultiplier;
    merchantBonuses[merchant].priceMultiplier += delta;
        print($"price multi for {merchant} = {merchantBonuses[merchant].priceMultiplier}");
    }

    private void UpdateAllSpecialBarterChances(Merchants merchant, float amount)
    {
        float oldMultiplier = merchantBonuses[merchant].specialBarterChanceBonus;

        float delta = amount - oldMultiplier;
        merchantBonuses[merchant].specialBarterChanceBonus += delta;

    }

    public void MerchantLevelUp(Merchants merchant, float xpAmount)
    {
        float nextRequiredXp = startValues[(int)merchant].requiredXp * Mathf.Pow(growthRate, merchantInfos[merchant].merchantLevel);
        merchantInfos[merchant].requiredXp = nextRequiredXp;
        merchantInfos[merchant].merchantXp = xpAmount;
        merchantInfos[merchant].merchantLevel += 1;
        OnBarterLevelUp?.Invoke(merchant);
    }

    private bool isRefreshFree()
    {
        float roll = UnityEngine.Random.Range(0, 1f);
        if (roll < freeRefreshChance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void OnRefreshClick()
    {
        if (refreshAmount > 0)
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
            if (!isRefreshFree())
            {
                refreshAmount--;
                StartRefreshTimer();
            }
        }
        else
        {
            print("No more refreshes available!"); // ADD POPUP TEXT HERE!
        }
        UpdateUI();
    }

    public void TESTING_CREATE_TESTINGBARTER()
    {
        GameObject newBarterOffer = Instantiate(TESTING_barterOfferPrefab, barterParentContainer);
        BarterCardHandler barterHandler = newBarterOffer.GetComponent<BarterCardHandler>();
        barterCardHandlers.Add(barterHandler);
        barterHandler.OnBarterClaimed += ForwardEventRaised;
        barterHandler.OnDecreaseFavor += ForwardEventRaisedLoseFavor;
        barterHandler.OnGainFavor += ForwardEventRaisedGainFavor;
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
        float chance = merchantBonuses[(Merchants)chosenMerchant].specialBarterChanceBonus;
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
    private bool isBarterTimed(int chosenMerchant)
    {
        float chance = merchantInfos[(Merchants)chosenMerchant].timedBarterChance;
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

        }else if(isBarterTimed(randomChosenMerchant)){
            newBarterOffer = Instantiate(timedBarterOfferPrefab,barterParentContainer);
        }
        else
        {
            newBarterOffer = Instantiate(barterOfferPrefab, barterParentContainer);
        }

        barterHandler = newBarterOffer.GetComponent<BarterCardHandler>();
        barterHandler.chosenMerchantIndex = randomChosenMerchant;
        barterCardHandlers.Add(barterHandler);

        // UnsubscribeFromCard(barterHandler);
        barterHandler.OnGainFavor += ForwardEventRaisedGainFavor;
        barterHandler.OnDecreaseFavor += ForwardEventRaisedLoseFavor;
        barterHandler.OnBarterClaimed += ForwardEventRaised;
    }

    public void UnsubscribeFromCard(BarterCardHandler handler)
    {
        handler.OnBarterClaimed -= ForwardEventRaised;
        handler.OnDecreaseFavor -= ForwardEventRaisedLoseFavor;
        handler.OnGainFavor -= ForwardEventRaisedGainFavor;
    }

    private void ForwardEventRaised(Merchants _merchant)
    {
        print("claimed!");
        OnBarterClaimed?.Invoke(_merchant);
    }
    private void ForwardEventRaisedGainFavor(Merchants _merchant, int amount){
        merchantInfos[_merchant].favor += amount;
        OnFavorGained?.Invoke(_merchant,merchantInfos[_merchant].favor);
    }
    private void ForwardEventRaisedLoseFavor(Merchants _merchant, int amount){
        merchantInfos[_merchant].favor -= amount;
        OnFavorGained?.Invoke(_merchant, merchantInfos[_merchant].favor);
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

    private void StartRefreshTimer()
    {
        if (refreshCoroutine == null)
        {
            refreshTimer = refreshTimerStart;
            refreshCoroutine = StartCoroutine(RefreshTimerCoroutine());
        }
    }
    private void StopRefreshTimer()
    {
        if (refreshCoroutine != null)
        {
            StopCoroutine(refreshCoroutine);
            refreshCoroutine = null;
        }
    }



    private IEnumerator RefreshTimerCoroutine()
    {

        while (true)
        {
            refreshTimer_txt.text = HelperFunctions.Instance.ConvertSecondsToTime(refreshTimer);
            yield return new WaitForSeconds(1f);

            refreshTimer -= 1f;
            if (refreshTimer <= 0f)
            {
                refreshTimer = 0f;
                refreshAmount++;
                UpdateUI();
                if (refreshAmount >= maxAmountRefresh)
                {
                    StopRefreshTimer();
                }
                else
                {
                    refreshTimer = refreshTimerStart;
                }
            }
        }
        // refreshTimer_txt.text = HelperFunctions.Instance.ConvertSecondsToTime(0f);
    }

    private void UpdateUI()
    {
        refreshAmount_txt.text = refreshAmount.ToString() + " / " + maxAmountRefresh.ToString();
    }

}
