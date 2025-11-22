using System;
using System.Collections.Generic;
using LargeNumbers;
using TMPro;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BarterCardHandler : MonoBehaviour
{
    private BarterManager barterManager;
    [SerializeField] private ProgressBarHandler xpProgressBar;
    [SerializeField] private ProgressBarHandler xpIncreaseProgressBar;
    [SerializeField] private PopUpTextHandler popUpTextHandler;

    [SerializeField] private TMP_Text Lvl_text_txt;
    [SerializeField] private TMP_Text price_txt;
    [SerializeField] private TMP_Text reward_txt;
    [SerializeField] private TMP_Text xp_txt;
    [SerializeField] private TMP_Text timer_txt;
    [SerializeField] private TMP_Text xpReward_txt;
    [SerializeField] private TMP_Text favorGain_txt;
    [SerializeField] private Image merchantIcon_img;
    [SerializeField] private Image priceIcon_img;
    [SerializeField] private Image rewardIcon_img;
    [SerializeField] private GameObject barterCard;
    [SerializeField] private GameObject timedOutButton;
    [SerializeField] private bool isSpecialBarterOffer = false;
    [SerializeField] private bool isTimedBarterOffer = false;
    private Coroutine timedBarter_coroutine;
    [SerializeField] private float startTime;
    private float timeRemaining;


    // [Header]
    [SerializeField] private bool TESTING_DONT_DESTROY;
    [SerializeField] private float amountModifier;
    [SerializeField] private float growthFactor = 1.5f;
    public int level;
    [SerializeField] private int startValue;

    private AlphabeticNotation priceAmount;
    private AlphabeticNotation originalPriceAmount;
    private float priceValue;
    private CurrencyTypes chosenPrice;
    private AlphabeticNotation rewardAmount;
    private AlphabeticNotation originalRewardAmount;
    private float rewardValue;
    private float xpReward;
    private float originalXpReward;
    private CurrencyTypes chosenReward;
    private int amountOfCurrencies;
    public Merchants chosenMerchant;
    private int favor;
    [SerializeField] private int baseFavorGain;
    private int unModifiedFavor;
    private Dictionary<CurrencyTypes, float> giveCurrencies = new();

    public event Action<Merchants, CurrencyTypes> OnBarterClaimed;
    public event Action<Merchants, int> OnGainFavor;
    public event Action<Merchants, int> OnDecreaseFavor;
    [SerializeField] float tradeValue;
    [SerializeField] float baseXp = 10f;
    // [SerializeField] float originalXp;
    [SerializeField] float randomFactor;
    [SerializeField] private float minRandom = 0.7f;
    [SerializeField] private float maxRandom = 1.5f;
    // [SerializeField] private int chloeFavorThresholdForMulti = 200;

    private UpgradeID unUsedUpgradeID = UpgradeID.RewardFlat;
    private IsWhatDatatype unUsedIsWhatDatatype = IsWhatDatatype.isAlphabeticnotationDatatype;

    [Header("TESTING VALUES")]
    [SerializeField] private bool isTesting;
    [SerializeField] private Merchants _TESTING_indexForMerchant;
    [SerializeField] private CurrencyTypes _TESTING_chosenPriceIndex;
    [SerializeField] private CurrencyTypes _TESTING_chosenRewardIndex;
    [SerializeField] private float _TESTING_priceValue;
    [SerializeField] private float _TESTING_rewardValue;
    [SerializeField] private int _TESTING_level;
    [SerializeField] private float _TESTING_xpReward;
    [SerializeField] private AlphabeticNotation _TESTING_priceAmount;
    [SerializeField] private AlphabeticNotation _TESTING_rewardAmount;
    [SerializeField] private float _TESTING_minRandom;
    [SerializeField] private float _TESTING_maxRandom;

    private void TESTINGFUNCTION()
    {
        priceAmount = _TESTING_priceAmount;
        originalPriceAmount = priceAmount;
        priceValue = _TESTING_priceValue;
        rewardValue = _TESTING_rewardValue;
        chosenMerchant = _TESTING_indexForMerchant;
        chosenPrice = _TESTING_chosenPriceIndex;
        chosenReward = _TESTING_chosenRewardIndex;
        level = _TESTING_level;
        minRandom = _TESTING_minRandom;
        maxRandom = _TESTING_maxRandom;
        baseXp = _TESTING_xpReward;
    }

    private void Start()
    {
        barterManager = GameObject.FindGameObjectWithTag("ShopPage").GetComponent<BarterManager>();
        barterManager.OnBarterXpGain += UpdateXpGain;
        barterManager.OnBarterLevelUp += UpdateReward;
        barterManager.OnUpgradeBought += UpdateBonuses;
        barterManager.OnBarterClaimed += UpdateBonuses;
        barterManager.OnResetStackingMerchants += ResetStacks;
        InitializeBarterOffer();
        UpdateBonuses(unUsedUpgradeID, unUsedIsWhatDatatype, chosenMerchant, chosenReward);


        // InitializeGiveBonuses();
        // originalXp = xpReward;

        // xpReward = ApplyBonusesToXp(chosenMerchantIndex);
        // rewardAmount = ApplyBonusesToRewards(chosenMerchantIndex, originalRewardAmount);


        UpdateUI();
    }

    private void InitializeBarterOffer()
    {
        foreach (BarterCurrencyValues barterCurrency in barterManager.barterCurrencyValues)
        {
            amountOfCurrencies++;
        }

        if (isTesting)
        {
            TESTINGFUNCTION();
            originalRewardAmount = CalculateReward();
        }
        else
        {
            int maxRandomIterations = 50;
            int counter = 0;
            chosenReward = (CurrencyTypes)GetRandomRewardIndex();
            chosenPrice = (CurrencyTypes)GetRandomIndex();
            while (chosenReward == chosenPrice)
            {
                chosenReward = (CurrencyTypes)GetRandomRewardIndex();
                counter++;
                if (counter >= maxRandomIterations)
                {
                    chosenPrice = (CurrencyTypes)GetRandomIndex();
                    maxRandomIterations = 0;
                }
            }

            priceValue = GetRandomValue((int)chosenPrice);
            rewardValue = GetRandomValue((int)chosenReward);
            level = barterManager.merchantInfos[chosenMerchant].merchantLevel; // if problems occur with chosenmerchantindex
                                                                               // initialize the chosenmerchantindex with a  getter from bartermanager
            priceAmount = GetRandomAmount(level);
            originalPriceAmount = priceAmount;
            originalRewardAmount = CalculateReward();
            originalXpReward = xpReward;
            // ApplyBonusesToPrice();
        }

        if (isTimedBarterOffer)
        {
            StartTimedBarterOffer();
            priceAmount = priceAmount * 2; // multipliers for timed barter offers
            rewardAmount = rewardAmount * 2; // multipliers for timed barter offers
            xpReward = xpReward * 3; // multipliers for timed barter offers
            favor = GetRandomFavor();
            // favor = ApplyBonusToFavor();
        }

    }

    private void OnDisable()
    {
        barterManager.OnBarterXpGain -= UpdateXpGain;
        barterManager.OnBarterLevelUp -= UpdateReward;
        barterManager.OnUpgradeBought -= UpdateBonuses;
        barterManager.OnBarterClaimed -= UpdateBonuses;
        barterManager.OnResetStackingMerchants -= ResetStacks;
        barterManager.UnsubscribeFromCard(this);

    }

    private int GetRandomIndex()
    {
        return UnityEngine.Random.Range(0, amountOfCurrencies);
    }

    private float GetRandomValue(int index)
    {
        float defaultValue = barterManager.barterCurrencyValues[index].defaultCurrencyValue;
        float randomValue = UnityEngine.Random.Range(defaultValue / 2, defaultValue * 2);
        return randomValue;
    }
    // private int GetRandomRewardIndex()
    // {

    //     int totalWeigth = 0;
    //         print($"outside "+MerchantUpgradeManager.Instance.merchantUpgrades[chosenMerchant].rewardWeigths.Count);
    //     foreach (var kvp in MerchantUpgradeManager.Instance.merchantUpgrades[chosenMerchant].rewardWeigths)
    //     {
    //         print($"bonus weigths for {chosenMerchant} type {kvp.Key} = {kvp.Value}");
    //         int baseWeight = kvp.Value;
    //         // int bonusWeigth = barterManager.merchantBonuses[(Merchants)chosenMerchantIndex].rewardCurrencyWeigthBonus[kvp.Key];
    //         // int effectiveWeigth = baseWeight + bonusWeigth;//REMOVED WHEN WORKING ON UNIFIED
    //         int effectiveWeigth = baseWeight;

    //         totalWeigth += effectiveWeigth;
    //     }

    //     int randomValue = UnityEngine.Random.Range(0, totalWeigth);
    //     int currentSum = 0;
    //     int currentIndex = 0;

    //     foreach (var kvp in MerchantUpgradeManager.Instance.merchantUpgrades[chosenMerchant].rewardWeigths)
    //     {
    //         print($"bonus weigths for {chosenMerchant} type {kvp.Key} = {kvp.Value}");
    //         int baseWeight = kvp.Value;
    //         // int bonusWeigth = barterManager.merchantBonuses[(Merchants)chosenMerchantIndex].rewardCurrencyWeigthBonus[kvp.Key];//REMOVED WHEN WORKING ON UNIFIED
    //         // int effectiveWeigth = baseWeight + bonusWeigth; //REMOVED WHEN WORKING ON UNIFIED
    //         int effectiveWeigth = baseWeight;
    //         currentSum += effectiveWeigth;
    //         if (randomValue < currentSum)
    //         {
    //             return currentIndex;
    //         }
    //         currentIndex++;
    //     }

    //     // Safety fallback
    //     return 0;

    // }
    private int GetRandomRewardIndex()
    {
        int totalWeight = 0;
        // 1. Sum all weights
        foreach (CurrencyTypes type in Enum.GetValues(typeof(CurrencyTypes)))
        {
            int weight = MerchantUpgradeManager.Instance
                .GetInt(UpgradeID.RewardWeight, chosenMerchant, type);
            totalWeight += weight;
        }
        // 2. Pick random number
        int randomValue = UnityEngine.Random.Range(0, totalWeight);
        int currentSum = 0;
        int index = 0;
        // 3. Select reward based on random weight
        foreach (CurrencyTypes type in Enum.GetValues(typeof(CurrencyTypes)))
        {
            int weight = MerchantUpgradeManager.Instance
                .GetInt(UpgradeID.RewardWeight, chosenMerchant, type);
            currentSum += weight;
            if (randomValue < currentSum)
                return index;
            index++;
        }
        return 0; // fallback
    }

    private AlphabeticNotation GetRandomAmount(int level)
    {
        AlphabeticNotation baseValue = new AlphabeticNotation(startValue);
        double scaledValue = Mathf.Pow(level, growthFactor) * amountModifier;

        float randomMin = 0.5f;
        float randomMax = 1.5f;
        AlphabeticNotation randomMultiplier = new AlphabeticNotation(UnityEngine.Random.Range(randomMin, randomMax));
        AlphabeticNotation finalAmount = new AlphabeticNotation(baseValue + scaledValue * randomMultiplier);

        return finalAmount;
    }


    private int GetRandomFavor()
    {
        float roll = UnityEngine.Random.Range(0, 1f);
        float result = roll * baseFavorGain;
        if (result < 1) result = 1;
        int res = (int)Mathf.Round(result);
        unModifiedFavor = res;
        return res;
    }

    private AlphabeticNotation CalculateReward()
    {
        float valueRatio = priceValue / rewardValue;
        rewardAmount = priceAmount * valueRatio;

        randomFactor = UnityEngine.Random.Range(minRandom, maxRandom);
        tradeValue = priceValue + rewardValue * (float)rewardAmount;

        level = barterManager.merchantInfos[chosenMerchant].merchantLevel;
        xpReward = (baseXp + Mathf.Pow(tradeValue, 0.7f))
                  * Mathf.Pow(level + 1, -0.4f)
                  * randomFactor;
        originalRewardAmount = rewardAmount;
        // rewardAmount = originalRewardAmount * (1 + barterManager.merchantInfos[(Merchants)chosenMerchantIndex].bonus);

        return rewardAmount;
    }
    private void UpdateBonuses(UpgradeID upgradeID, IsWhatDatatype isWhatDatatype, Merchants _merchants, CurrencyTypes types)
    {
        if (_merchants != chosenMerchant) return;
        ApplyBonusesToReward(chosenMerchant, types);
        ApplyBonusesToXp(chosenMerchant);
        ApplyBonusToFavor();
        ApplyBonusesToPrice();
        // InitializeGiveBonuses();
        UpdateUI();
    }

    private void ApplyBonusesToReward(Merchants merchant, CurrencyTypes type)
    {
        if (type != chosenReward || merchant != chosenMerchant) return;
        
        float favorBasedMulti = MerchantUpgradeManager.Instance.GetFloat(UpgradeID.multiRewardBasedOnFavor,chosenMerchant,chosenReward) * barterManager.merchantInfos[chosenMerchant].favor;
        float stackingMulti = (MerchantUpgradeManager.Instance.GetFloat(UpgradeID.stackingMulti, chosenMerchant, chosenReward) - 1) * barterManager.merchantInfos[chosenMerchant].completedInArow;
        float baseMulti = MerchantUpgradeManager.Instance.GetFloat(UpgradeID.RewardMulti, merchant, type);
        float totalMulti = favorBasedMulti + stackingMulti + baseMulti;
        AlphabeticNotation flat = MerchantUpgradeManager.Instance.GetAlphabetic(UpgradeID.RewardFlat, merchant, type);

        if(isSpecialBarterOffer){
        rewardAmount = ((originalRewardAmount + flat) * totalMulti) * MerchantUpgradeManager.Instance.GetFloat(UpgradeID.specialBarterRewardMulti,chosenMerchant,type);
        }else{
        rewardAmount = (originalRewardAmount + flat) * totalMulti;
        }

        UpdateUI();
    }

    private void ApplyBonusToFavor(){
        float multi = MerchantUpgradeManager.Instance.GetFloat(UpgradeID.favorGainMulti,chosenMerchant,CurrencyDummy.Dummy);
        int flat = MerchantUpgradeManager.Instance.GetInt(UpgradeID.flatFavorGain,chosenMerchant,CurrencyDummy.Dummy);

        float result = (unModifiedFavor + flat) * multi;
        favor = (int) Mathf.Round(result);
}

private void ApplyBonusesToPrice(){ // have to add in flat when that gets implemented
    float multi = MerchantUpgradeManager.Instance.GetFloat(UpgradeID.priceMulti,chosenMerchant,chosenPrice);
    AlphabeticNotation result = originalPriceAmount * multi;
    priceAmount = result;
}

    // private AlphabeticNotation ApplyBonusesToRewards(int merchantIndex, AlphabeticNotation amount)
    // {
    //     favor = ApplyBonusToFavor();
    //     AlphabeticNotation result;
    //     int bartersInArow = barterManager.merchantInfos[(Merchants)merchantIndex].completedInArow;
    //     AlphabeticNotation stackingMulti = barterManager.merchantBonuses[(Merchants)merchantIndex].stackingMulit -1;    
    //     AlphabeticNotation stackingBonus = new AlphabeticNotation(1) + (stackingMulti * bartersInArow);
    //     if(isSpecialBarterOffer){
    //         result = (((amount + barterManager.merchantBonuses[(Merchants)merchantIndex].rewardBaseFlatIncreaseBonus[(CurrencyTypes)chosenRewardIndex]) *
    //                 barterManager.merchantBonuses[(Merchants)merchantIndex].rewardMultiplierBonus[(CurrencyTypes)chosenRewardIndex])
    //                 *barterManager.merchantBonuses[(Merchants)chosenMerchantIndex].specialBarterOfferMulti)* stackingBonus;
    //     }else{
    //         result = ((amount + barterManager.merchantBonuses[(Merchants)merchantIndex].rewardBaseFlatIncreaseBonus[(CurrencyTypes)chosenRewardIndex]) *
    //                                 barterManager.merchantBonuses[(Merchants)merchantIndex].rewardMultiplierBonus[(CurrencyTypes)chosenRewardIndex])* stackingBonus;
    //     }   
    //     if(barterManager.merchantInfos[Merchants.ChloeTheMerchant].favor > chloeFavorThresholdForMulti){
    //         // result = result * MerchantUpgradeManager.Instance.ChloeGetRewardPowerFloat(ChloeUpgradeTypesFloats.multiAllOnFavorPassed); // REMOVED WORKING UNIFIED
    //     }
    //     return result;
    // }
    private void ResetStacks()
    {
        foreach (Merchants merch in Enum.GetValues(typeof(Merchants)))
        {
            foreach (CurrencyTypes type in Enum.GetValues(typeof(CurrencyTypes)))
                ApplyBonusesToReward(merch, type);
        }
    }

    private void ApplyBonusesToXp(Merchants merchant)
    {
        if (merchant != chosenMerchant) return;
        float multi = MerchantUpgradeManager.Instance.GetFloat(UpgradeID.XpGainMulti, merchant, CurrencyDummy.Dummy);
        print($"merchant {merchant} xp multi = {multi} bonus = {MerchantUpgradeManager.Instance.GetFloat(UpgradeID.XpGainMulti, merchant, CurrencyDummy.Dummy)}");
        xpReward = originalXpReward * multi;

    }
    private void BonusesHandledOnClaimed(Merchants _merchant)
    {
        if (_merchant != chosenMerchant) return;

    }





    private bool NoRewardCheck()
    {
        float roll = UnityEngine.Random.Range(0, 1f);
        float chance = barterManager.merchantInfos[chosenMerchant].rewardRecieveChance;
        // print($"rolled = {roll} chance = {chance}");
        if (roll < chance)
        {
            // print("REWARD GIVEN!");
            return false;
        }
        else
        {
            popUpTextHandler.RunPopUpFadeUp($"No reward for you!");
            // print("NO REWARD GIVEN!");
            return true;
        }
    }





    // ---------------- COMPLETE BARTER OFFER --------------- //
    public void OnClaimClick()
    {
        if (MoneyManager.Instance.GetCurrency(barterManager.barterCurrencyValues[(int)chosenPrice].currencyType) >= priceAmount)
        {
            if (NoRewardCheck())
            {
                // DestroyCard();
                return;
            }
            barterManager.merchantInfos[chosenMerchant].completedBartersForMerchant++;
            if (barterManager.isMerchantSameAsLast(chosenMerchant))
            {
                barterManager.merchantInfos[chosenMerchant].completedInArow++;
            }
            else
            {
                barterManager.ResetStackingSameMerchant();
                barterManager.merchantInfos[chosenMerchant].completedInArow = 1;
            }
            // ApplyBonusGiveCurrency();
            CompleteBarter();
            OnBarterClaimed?.Invoke(chosenMerchant, chosenReward);
            GiveBonusCurrenies(chosenMerchant);

            if (isTimedBarterOffer)
            {
                OnGainFavor?.Invoke(chosenMerchant, favor);
                StopTimedBarterOffer();
            }

            if(isClaimConsumed()){
                DestroyCard();
            }
        }
    }


    private void GiveBonusCurrenies(Merchants _merchant){
        if(_merchant != chosenMerchant) return;
        AlphabeticNotation amountToGive;
        foreach(CurrencyTypes type in Enum.GetValues(typeof(CurrencyTypes))){
            print("amount from upgrade" + MerchantUpgradeManager.Instance.GetFloat(UpgradeID.bonusGiveCurrency,_merchant,type));
            amountToGive = MoneyManager.Instance.GetCurrency(type) * MerchantUpgradeManager.Instance.GetFloat(UpgradeID.bonusGiveCurrency,_merchant,type);
            MoneyManager.Instance.AddCurrency(type, amountToGive);
            print($"gave amount {amountToGive} type {type}");
        }
    }
    private bool isClaimConsumed(){// REMOVED WHEN WORKING ON UNIFIED
        // float chance = barterManager.merchantBonuses[(Merchants)chosenMerchantIndex].chanceToNotConsumeClaimBonus;
        float chance = 0f; // change to the upgradable once implemented
        float roll = UnityEngine.Random.Range(0f, 1f);
        if (roll > chance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void CompleteBarter()
    {
        barterManager.BarterOfferBought(chosenMerchant, xpReward);

        float xp = barterManager.merchantInfos[chosenMerchant].merchantXp;
        float reqXp = barterManager.merchantInfos[chosenMerchant].requiredXp;

        xpProgressBar.SetProgress(xp / reqXp);

        UpdateUI();
    }

    public void DestroyCard()
    {
        if (TESTING_DONT_DESTROY) return;
        barterManager.UnsubscribeFromCard(this);
        Destroy(barterCard);
    }


    // ------------- timed barter offer ----------------- //
    public void OnTimedOutClick()
    {
        DestroyCard();
    }
    private float GetRandomTime()
    {
        float randomMultiplier = UnityEngine.Random.Range(.5f, 1.5f);
        float chosenTime = randomMultiplier * startTime;
        return chosenTime;
    }
    private void StopTimedBarterOffer()
    {
        if (timedBarter_coroutine != null)
        {
            StopCoroutine(timedBarter_coroutine);
            timedBarter_coroutine = null;
        }
    }

    private void StartTimedBarterOffer()
    {
        timeRemaining = GetRandomTime();
        if (timedBarter_coroutine == null)
        {
            timedBarter_coroutine = StartCoroutine(StartTimedBarter_Coroutine());
        }
    }

    private IEnumerator StartTimedBarter_Coroutine()
    {
        while (true)
        {
            timer_txt.text = HelperFunctions.Instance.ConvertSecondsToTime(timeRemaining);
            yield return new WaitForSeconds(1f);
            timeRemaining -= 1f;
            if (timeRemaining <= 0f)
            {
                timeRemaining = 0f;
                timedOutButton.SetActive(true);
                OnDecreaseFavor?.Invoke(chosenMerchant, favor);
                StopTimedBarterOffer();
            }
        }
    }

    // -------------- UPDATE ------------- //

    private void UpdateReward(Merchants merchants)
    {
        if (merchants != chosenMerchant) return;
        level = barterManager.merchantInfos[chosenMerchant].merchantLevel;
        reward_txt.text = "x" + rewardAmount.ToStringSmart(0);
        Lvl_text_txt.text = level.ToString();
    }

    private void UpdateXpGain(Merchants merchants)
    {
        if (merchants != chosenMerchant) return;
        level = barterManager.merchantInfos[chosenMerchant].merchantLevel;
        float xp = barterManager.merchantInfos[chosenMerchant].merchantXp;
        float reqXp = barterManager.merchantInfos[chosenMerchant].requiredXp;

        Lvl_text_txt.text = "Lv." + level.ToString();
        xp_txt.text = string.Format("{0:F0} / {1:F0}",
                                                barterManager.merchantInfos[chosenMerchant].merchantXp,
                                                barterManager.merchantInfos[chosenMerchant].requiredXp);
        xpReward_txt.text = $"+{xpReward:F0}xp";

        xpProgressBar.SetProgress(xp / reqXp);
        xpIncreaseProgressBar.SetProgress((xpReward + barterManager.merchantInfos[chosenMerchant].merchantXp) /
                                                   barterManager.merchantInfos[chosenMerchant].requiredXp);
    }
    private void UpdateUI()
    {
        float xp = barterManager.merchantInfos[chosenMerchant].merchantXp;
        float reqXp = barterManager.merchantInfos[chosenMerchant].requiredXp;
        level = barterManager.merchantInfos[chosenMerchant].merchantLevel;

        merchantIcon_img.sprite = barterManager.merchantInfos[chosenMerchant].merchantIcon_sprite;

        Lvl_text_txt.text = "Lv." + level.ToString();
        price_txt.text = "x" + priceAmount.ToStringSmart(0);
        reward_txt.text = "x" + rewardAmount.ToStringSmart(0);
        xp_txt.text = string.Format("{0:F0} / {1:F0}",
                                                barterManager.merchantInfos[chosenMerchant].merchantXp,
                                                barterManager.merchantInfos[chosenMerchant].requiredXp);
        xpReward_txt.text = $"+{xpReward:F0}xp";

        xpProgressBar.SetProgress(xp / reqXp);
        xpIncreaseProgressBar.SetProgress((xpReward + barterManager.merchantInfos[chosenMerchant].merchantXp) /
                                                   barterManager.merchantInfos[chosenMerchant].requiredXp);

        priceIcon_img.sprite = barterManager.barterCurrencyValues[(int)chosenPrice].currencySprite;
        rewardIcon_img.sprite = barterManager.barterCurrencyValues[(int)chosenReward].currencySprite;

        OnGainFavor?.Invoke(chosenMerchant, 0);
        if (isTimedBarterOffer && favorGain_txt != null) favorGain_txt.text = favor.ToString();
    }
}
// private int ApplyBonusToFavor(){
//     float result  = unModifiedFavor * barterManager.merchantBonuses[(Merchants)chosenMerchantIndex].favorMultiBonus;
//     int res = (int)Mathf.Round(result); 
//     return res;
// }
// private void ApplyBonusesToPrice(){ //REMOVED WHEN WORKING ON UNIFIED
//     priceAmount =  barterManager.merchantBonuses[(Merchants)chosenMerchantIndex].priceMultiplier * originalPriceAmount;
// }

// private void InitializeGiveBonuses(){
//     foreach(CurrencyTypes type in barterManager.merchantBonuses[(Merchants)chosenMerchantIndex].giveCurrencies){
//         giveCurrencies[type] = barterManager.merchantBonuses[(Merchants)chosenMerchantIndex].giveCurrencyOnBarterCompletion[type];
//     }
// }


// private float ApplyBonusesToXp(int merchantIndex){ // REMOVED WHEN WORKING ON UNIFIED
//     float result = originalXp * barterManager.merchantBonuses[(Merchants)merchantIndex].xpRewardBonus; // HARDCODED FOR TESTING

//     return result;
// }

// private void ApplyBonusesToXpBaseOnPrevious(Merchants _merchant){
//     if(_merchant != (Merchants)chosenMerchantIndex) return;
//     // bool chloeUpgradeActivated = MerchantUpgradeManager.Instance.ChloeGetRewardPowerBool(ChloeUpgradeTypesBool.doubleXpOnNextBarter); //REMOVED WORKING UNIFIED
//     // bool shouldDoubleXp = chloeUpgradeActivated && barterManager.previousMerchantCompleted == Merchants.ChloeTheMerchant; //REMOVED WORKING UNIFIED

//     // xpReward = shouldDoubleXp ? originalXp * 2 : originalXp * 1;//REMOVED WORKING UNIFIED
//     UpdateUI();
// }



// private void ApplyBonusGiveCurrency(){
//     foreach(var kvp in giveCurrencies){
//     AlphabeticNotation amount = kvp.Value * MoneyManager.Instance.GetCurrency(kvp.Key);
//     MoneyManager.Instance.AddCurrency(kvp.Key,amount);
//     }

// }