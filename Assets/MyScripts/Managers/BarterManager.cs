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
}
public class BarterManager : MonoBehaviour
{
    public List<BarterCurrencyValues> barterCurrencyValues;
    public Dictionary<Merchants, MerchantInfo> merchantInfos;
    // public Dictionary<Merchants, MerchantBonuses> merchantBonuses;
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
    private int baseMaxRefreshes;
    private float freeRefreshChance = 0f;
    private float baseFreeRefreshChance = 0f;
    [SerializeField] private float refreshTimerStart = 5f;
    [SerializeField] private float baseRefreshTimerStart;
    [SerializeField] private float chanceForTimedOffer = 0.05f;
    private float refreshTimer;
    [SerializeField] private float growthRate; // tweak this to adjust scaling for how fast lvl requirement xp increases
    [SerializeField] private int maxAmountOfBarters;

    [SerializeField] private bool isTesting;
    public Merchants previousMerchantCompleted;

    private UpgradeID unUsedUpgradeId;
    private IsWhatDatatype unUsedIsWhatDatatype;
    private CurrencyTypes unUsedCurrencytype;

    public event Action<Merchants> OnBarterLevelUp;
    public event Action<Merchants> OnBarterXpGain;
    public event Action<UpgradeID,IsWhatDatatype,Merchants, CurrencyTypes> OnUpgradeBought;
    public event Action<UpgradeID,IsWhatDatatype,Merchants, CurrencyTypes> OnBarterClaimed;
    public event Action<Merchants, int> OnFavorGained;
    public event Action OnResetStackingMerchants;


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
        public int originalAppearChanceWeight;
        public int completedBartersForMerchant = 0;
        public int completedInArow = 0;
        public float rewardRecieveChance = 1f;
        public float originalRecieveChance = 1f;
        
    }





    private void Awake()
    {
        InitializeMerchantInfos();
        // InitializeMerchantBonuses();
        baseMaxRefreshes = maxAmountRefresh;
        baseRefreshTimerStart = refreshTimerStart;
        foreach (Merchants merchant in Enum.GetValues(typeof(Merchants)))
        {
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
        if (isTesting)
        {
            foreach (Merchants _merchant in Enum.GetValues(typeof(Merchants)))
            {
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
            // info.InitializeRewardWeights();
            merchantInfos.Add(merchant, info);
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

    public void UpgradeBought(UpgradeID upgradeID, IsWhatDatatype isWhatDatatype,Merchants merchant, CurrencyTypes types)
    { // SETS THE UPGRADE FOR EACH OF THE MERCHANTS. Set specific upgrades here
        // print($"upgrade id = {upgradeID} merchant {merchant}");
        switch(upgradeID){ // call global upgrades for the shop page or production
            case UpgradeID.extraRefreshAmount:
            IncreaseMaxRefreshes(upgradeID);
            break;

            case UpgradeID.chanceToNotConsumeRefresh:
            IncreaseFreeRefreshChance(upgradeID);
            break;

            case UpgradeID.refreshTimeReduction:
            ReduceRefreshTime(upgradeID);
             break;
            // break; //removed 03.01.26 - not working correctly
            //  case UpgradeID.oneTimeFavorModifyLose:
            //     foreach(Merchants merch in Enum.GetValues(typeof(Merchants))){
            //         ForwardEventRaisedLoseFavor(merch,MerchantUpgradeManager.Instance.GetInt(UpgradeID.oneTimeFavorModifyLose,merch,CurrencyDummy.Dummy));
            //     }                
            //  break;
            //  case UpgradeID.oneTimeFavorModifyGain:
            //  print("inside gain for "+ merchant);
            //     foreach(Merchants merch in Enum.GetValues(typeof(Merchants))){
            //         ForwardEventRaisedGainFavor(merch,MerchantUpgradeManager.Instance.GetInt(UpgradeID.oneTimeFavorModifyGain,merch,CurrencyDummy.Dummy));
            //     }                
        }
        // print($"called from merchant {merchant}");
        OnUpgradeBought?.Invoke(upgradeID, isWhatDatatype, merchant,types);
        UpdateUI();
    }






    public void MerchantLevelUp(Merchants merchant, float xpAmount)
    {
        float nextRequiredXp = startValues[(int)merchant].requiredXp * Mathf.Pow(growthRate, merchantInfos[merchant].merchantLevel);
        merchantInfos[merchant].requiredXp = nextRequiredXp;
        merchantInfos[merchant].merchantXp = xpAmount;
        merchantInfos[merchant].merchantLevel += 1;
        OnBarterLevelUp?.Invoke(merchant);
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

        foreach(Merchants merch in Enum.GetValues(typeof(Merchants))){
            totalWeigth += MerchantUpgradeManager.Instance.GetInt(UpgradeID.merchantAppearWeigth, merch, CurrencyDummy.Dummy);
        }
        int randomValue = UnityEngine.Random.Range(0, totalWeigth);
        int currentSum = 0;
        int currentIndex = 0;

        foreach(Merchants merch in Enum.GetValues(typeof(Merchants))){
            currentSum += MerchantUpgradeManager.Instance.GetInt(UpgradeID.merchantAppearWeigth,merch,CurrencyDummy.Dummy);
            if(randomValue < currentSum){
                return currentIndex;
            }
            currentIndex++;
        }
        return 0;
    }
        // foreach (var segment in merchantInfos.Values)
        // {
        //     totalWeigth += segment.appearChanceWeigth;
        // }
        // int randomValue = UnityEngine.Random.Range(0, totalWeigth);
        // int currentSum = 0;
        // int currentIndex = 0;
        // foreach (var segment in merchantInfos.Values)
        // {
        //     currentSum += segment.appearChanceWeigth;
        //     if (randomValue < currentSum)
        //     {
        //         return currentIndex;
        //     }
        //     currentIndex++;
        // }

        // // Safety fallback
        // return 0;

    private bool isBarterSpecial(Merchants chosenMerchant)
    {
        // float chance = merchantBonuses[chosenMerchant].specialBarterChanceBonus; // REMOVED WHEN WORKING ON UNIFIED
        float chance = MerchantUpgradeManager.Instance.GetFloat(UpgradeID.specialBarterChance,chosenMerchant,CurrencyDummy.Dummy); 
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
    private bool isBarterTimed(Merchants chosenMerchant)
    {
        float chance = merchantInfos[chosenMerchant].timedBarterChance;
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
        Merchants randomChosenMerchant = (Merchants)GetRandomMerchant();
        GameObject newBarterOffer;
        BarterCardHandler barterHandler;

        if (isBarterSpecial(randomChosenMerchant))
        {
            newBarterOffer = Instantiate(specialBarterOfferPrefab, barterParentContainer);
        }
        else if (isBarterTimed(randomChosenMerchant))
        {
            newBarterOffer = Instantiate(timedBarterOfferPrefab, barterParentContainer);
        }
        else
        {
            newBarterOffer = Instantiate(barterOfferPrefab, barterParentContainer);
        }

        barterHandler = newBarterOffer.GetComponent<BarterCardHandler>();
        barterHandler.chosenMerchant = randomChosenMerchant;
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



    private void ForwardEventRaised(Merchants _merchant, CurrencyTypes _type)
    {
        foreach(CurrencyTypes currency in Enum.GetValues(typeof(CurrencyTypes))){
            OnBarterClaimed?.Invoke(unUsedUpgradeId,unUsedIsWhatDatatype,_merchant,currency);
        }
    }


    private void ForwardEventRaisedGainFavor(Merchants _merchant, int amount)
    {
        merchantInfos[_merchant].favor += amount;
        OnFavorGained?.Invoke(_merchant, merchantInfos[_merchant].favor);
    }
    private void ForwardEventRaisedLoseFavor(Merchants _merchant, int amount)
    {
        merchantInfos[_merchant].favor -= amount;
        OnFavorGained?.Invoke(_merchant, merchantInfos[_merchant].favor);
    }


    public void ResetStackingSameMerchant(){
        OnResetStackingMerchants?.Invoke();
    }
    public bool isMerchantSameAsLast(Merchants _merchant)
    {
        if (previousMerchantCompleted != _merchant)
        {
            previousMerchantCompleted = _merchant;
            foreach (Merchants merchant in Enum.GetValues(typeof(Merchants)))
            {
                merchantInfos[merchant].completedInArow = 0;
            }
            return false;
        }
        return true;
    }


// --------------- OneTime upgrades LOGIC ---------------- //

private void DecreaseFavor(){

}
// --------------- REFRESH LOGIC ---------------- //
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
            //add popup!
        }
        UpdateUI();
    }

    private void IncreaseMaxRefreshes(UpgradeID upgradeID ){
        if(upgradeID != UpgradeID.extraRefreshAmount) return;
        int refreshCounter = 0;
        foreach(Merchants merch in Enum.GetValues(typeof(Merchants))){
            refreshCounter += MerchantUpgradeManager.Instance.GetInt(upgradeID,merch,CurrencyDummy.Dummy);
        }
        maxAmountRefresh = baseMaxRefreshes + refreshCounter;
    }
    private void IncreaseFreeRefreshChance(UpgradeID upgradeID ){
        if(upgradeID != UpgradeID.chanceToNotConsumeRefresh) return;
        float chanceCounter = 0f;
        foreach(Merchants merch in Enum.GetValues(typeof(Merchants))){
            chanceCounter += MerchantUpgradeManager.Instance.GetFloat(upgradeID,merch,CurrencyDummy.Dummy);
        }
            freeRefreshChance = baseFreeRefreshChance + chanceCounter;
    }

    private void ReduceRefreshTime(UpgradeID upgradeID){
        float reductionCounter = 1f;
        foreach(Merchants merch in Enum.GetValues(typeof(Merchants))){
            reductionCounter =  reductionCounter- MerchantUpgradeManager.Instance.GetFloat(upgradeID,merch,CurrencyDummy.Dummy);
        }
        refreshTimerStart = baseRefreshTimerStart * reductionCounter;
        if(refreshTimerStart < 0.01) refreshTimerStart = 0.01f;

        StopRefreshTimer();
        StartRefreshTimer();
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




        // public Dictionary<CurrencyTypes, int> rewardCurrencyWeigth;

        // public void InitializeRewardWeights()
        // {
        //     rewardCurrencyWeigth = new Dictionary<CurrencyTypes, int>();
        //     foreach (CurrencyTypes type in Enum.GetValues(typeof(CurrencyTypes)))
        //     {
        //         if (type == CurrencyTypes.wheat)
        //         {
        //             rewardCurrencyWeigth[type] = 1; // TESTING, REMOVE WHEN DONE!
        //         }
        //         else
        //         {
        //             rewardCurrencyWeigth[type] = 1; // or whatever default weight you want
        //         }
        //     }
        // }
    // [System.Serializable]
    // public class MerchantBonuses
    // {
    //     public Dictionary<CurrencyTypes, AlphabeticNotation> rewardMultiplierBonus = new();
    //     public Dictionary<CurrencyTypes, AlphabeticNotation> rewardBaseFlatIncreaseBonus = new();
    //     public Dictionary<CurrencyTypes, AlphabeticNotation> totalBonus = new();
    //     public Dictionary<CurrencyTypes, int> rewardCurrencyWeigthBonus = new();
    //     public Dictionary<CurrencyTypes, float> giveCurrencyOnBarterCompletion = new();
    //     public List<CurrencyTypes> giveCurrencies = new();
    //     public AlphabeticNotation stackingMulit = new AlphabeticNotation(1);
    //     public float xpRewardBonus = 1f;
    //     public float specialBarterOfferMulti = 100;
    //     public float specialBarterChanceBonus = 0f;
    //     public int refreshAditionBonus = 0;
    //     public float freeRefreshChanceBonus = 0;
    //     public float reducedRefreshTimeBonus = 0;
    //     public float chanceToNotConsumeClaimBonus = 0f;
    //     public float priceMultiplier = 1f;
    //     public float favorMultiBonus = 1f;

    //     public void InitializeDefaults()
    //     {
    //         foreach (CurrencyTypes type in Enum.GetValues(typeof(CurrencyTypes)))
    //         {
    //             rewardMultiplierBonus[type] = new AlphabeticNotation(1); // 1 means no multiplier (neutral)
    //             rewardBaseFlatIncreaseBonus[type] = new AlphabeticNotation(0); // 0 means no flat bonus
    //             giveCurrencyOnBarterCompletion[type] = 0f;
    //             totalBonus[type] = rewardMultiplierBonus[type] * rewardBaseFlatIncreaseBonus[type];
    //             rewardCurrencyWeigthBonus[type] = 0;
    //         }
    //     }
    // }

    //     private void InitializeMerchantBonuses()
    // {
    //     merchantBonuses = new Dictionary<Merchants, MerchantBonuses>();

    //     foreach (Merchants merchant in Enum.GetValues(typeof(Merchants)))
    //     {
    //         MerchantBonuses info = new MerchantBonuses();
    //         info.InitializeDefaults();
    //         merchantBonuses.Add(merchant, info);
    //     }
    // }