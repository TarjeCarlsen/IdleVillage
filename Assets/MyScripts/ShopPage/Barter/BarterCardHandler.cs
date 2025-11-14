using System;
using LargeNumbers;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BarterCardHandler : MonoBehaviour
{
    private BarterManager barterManager;
    [SerializeField] private ProgressBarHandler xpProgressBar;
    [SerializeField] private ProgressBarHandler xpIncreaseProgressBar;

    [SerializeField] private TMP_Text Lvl_text_txt;
    [SerializeField] private TMP_Text price_txt;
    [SerializeField] private TMP_Text reward_txt;
    [SerializeField] private TMP_Text xp_txt;
    [SerializeField] private TMP_Text xpReward_txt;
    [SerializeField] private Image merchantIcon_img;
    [SerializeField] private Image priceIcon_img;
    [SerializeField] private Image rewardIcon_img;
    [SerializeField] private GameObject barterCard;
    [SerializeField] private bool isSpecialBarterOffer = false;

    // [Header]
    [SerializeField] private bool TESTING_DONT_DESTROY;
    [SerializeField] private float amountModifier;
    [SerializeField] private float growthFactor = 1.5f;
    public int level;
    [SerializeField] private int startValue;

    private AlphabeticNotation priceAmount;
    private float priceValue;
    private int chosenPriceIndex;
    private AlphabeticNotation rewardAmount;
    private AlphabeticNotation originalRewardAmount;
    private float rewardValue;
    private float xpReward;
    private int chosenRewardIndex;
    private int amountOfCurrencies;
    public int chosenMerchantIndex;

    public event Action <Merchants> OnBarterClaimed;
    [SerializeField] float tradeValue;
    [SerializeField] float baseXp = 10f;
    [SerializeField] float originalXp;
    [SerializeField] float randomFactor;
    [SerializeField] private float minRandom = 0.7f;
    [SerializeField] private float maxRandom = 1.5f;



    [Header("TESTING VALUES")]
    [SerializeField] private bool isTesting;
    [SerializeField] private int _TESTING_indexForMerchant;
    [SerializeField] private int _TESTING_chosenPriceIndex;
    [SerializeField] private int _TESTING_chosenRewardIndex;
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

        priceValue = _TESTING_priceValue;
        rewardValue = _TESTING_rewardValue;
        chosenMerchantIndex = _TESTING_indexForMerchant;
        chosenPriceIndex = _TESTING_chosenPriceIndex;
        chosenRewardIndex = _TESTING_chosenRewardIndex;
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

        foreach (BarterCurrencyValues barterCurrency in barterManager.barterCurrencyValues)
        {
            amountOfCurrencies++;
        }

        if (isTesting)
        {
            TESTINGFUNCTION();
        }
        else
        {
            int maxRandomIterations = 50;
            int counter = 0;
            chosenRewardIndex = GetRandomRewardIndex();
            chosenPriceIndex = GetRandomIndex();
            while (chosenRewardIndex == chosenPriceIndex)
            {
                chosenRewardIndex = GetRandomRewardIndex();
                counter++;
                if(counter >= maxRandomIterations){
                    chosenPriceIndex = GetRandomIndex();
                    maxRandomIterations = 0;
                }
            }
            priceValue = GetRandomValue(chosenPriceIndex);
            rewardValue = GetRandomValue(chosenRewardIndex);
            level = barterManager.merchantInfos[(Merchants)chosenMerchantIndex].merchantLevel; // if problems occur with chosenmerchantindex
                                                                                               // initialize the chosenmerchantindex with a  getter from bartermanager
            priceAmount = GetRandomAmount(level);
        }


        originalRewardAmount = CalculateReward();
        originalXp = xpReward;

        xpReward = ApplyBonusesToXp(chosenMerchantIndex, chosenRewardIndex, originalRewardAmount);
        rewardAmount = ApplyBonusesToRewards(chosenMerchantIndex, originalRewardAmount);

        UpdateUI();
    }

    private void OnDisable()
    {
        barterManager.OnBarterXpGain -= UpdateXpGain;
        barterManager.OnBarterLevelUp -= UpdateReward;
        barterManager.OnUpgradeBought -= UpdateBonuses;
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
    private int GetRandomRewardIndex(){
        int totalWeigth = 0;

        foreach(var kvp in barterManager.merchantInfos[(Merchants)chosenMerchantIndex].rewardCurrencyWeigth){
            int baseWeight = kvp.Value;
            int bonusWeigth = barterManager.merchantBonuses[(Merchants)chosenMerchantIndex].rewardCurrencyWeigthBonus[kvp.Key];
            int effectiveWeigth = baseWeight + bonusWeigth;

            totalWeigth += effectiveWeigth;
        }

        int randomValue = UnityEngine.Random.Range(0,totalWeigth);
        int currentSum = 0;
        int currentIndex = 0;

        foreach(var kvp in barterManager.merchantInfos[(Merchants)chosenMerchantIndex].rewardCurrencyWeigth){
            int baseWeight = kvp.Value;
            int bonusWeigth = barterManager.merchantBonuses[(Merchants)chosenMerchantIndex].rewardCurrencyWeigthBonus[kvp.Key];
            int effectiveWeigth = baseWeight + bonusWeigth;
            currentSum += effectiveWeigth;
            if(randomValue < currentSum){ //DOESNT ENTER THE IF, BUG FOUND!
                return currentIndex;
            }
            currentIndex++;
        }
        
        // Safety fallback
        return 0;

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

    private AlphabeticNotation CalculateReward()
    {
        float valueRatio = priceValue / rewardValue;
        rewardAmount = priceAmount * valueRatio;

        randomFactor = UnityEngine.Random.Range(minRandom, maxRandom);
        tradeValue = priceValue + rewardValue * (float)rewardAmount;

        level = barterManager.merchantInfos[(Merchants)chosenMerchantIndex].merchantLevel;
        xpReward = (baseXp + Mathf.Pow(tradeValue, 0.7f))
                  * Mathf.Pow(level + 1, -0.4f)
                  * randomFactor;
        originalRewardAmount = rewardAmount;
        // rewardAmount = originalRewardAmount * (1 + barterManager.merchantInfos[(Merchants)chosenMerchantIndex].bonus);

        return rewardAmount;
    }

    private void UpdateBonuses(Merchants _merchants)
    {
        if (_merchants != (Merchants)chosenMerchantIndex) return;
        rewardAmount = ApplyBonusesToRewards(chosenMerchantIndex, originalRewardAmount);
        xpReward = ApplyBonusesToXp(chosenMerchantIndex, chosenRewardIndex, originalRewardAmount);
        UpdateUI();
    }
    private AlphabeticNotation ApplyBonusesToRewards(int merchantIndex, AlphabeticNotation amount)
    {
        AlphabeticNotation result;
        int bartersInArow = barterManager.merchantInfos[(Merchants)merchantIndex].completedInArow;
        AlphabeticNotation stackingMulti = barterManager.merchantBonuses[(Merchants)merchantIndex].stackingMulit -1;

        AlphabeticNotation stackingBonus = new AlphabeticNotation(1) + (stackingMulti * bartersInArow);
        if(isSpecialBarterOffer){
            result = (((amount + barterManager.merchantBonuses[(Merchants)merchantIndex].rewardBaseFlatIncreaseBonus[(CurrencyTypes)chosenRewardIndex]) *
                    barterManager.merchantBonuses[(Merchants)merchantIndex].rewardMultiplierBonus[(CurrencyTypes)chosenRewardIndex])
                    *barterManager.merchantBonuses[(Merchants)chosenMerchantIndex].specialBarterOfferMulti)* stackingBonus;
        }else{
            result = ((amount + barterManager.merchantBonuses[(Merchants)merchantIndex].rewardBaseFlatIncreaseBonus[(CurrencyTypes)chosenRewardIndex]) *
                                    barterManager.merchantBonuses[(Merchants)merchantIndex].rewardMultiplierBonus[(CurrencyTypes)chosenRewardIndex])* stackingBonus;
        }

        return result;
    }

    private float ApplyBonusesToXp(int merchantIndex, int currencyIndex, AlphabeticNotation amount){
        float result = originalXp * barterManager.merchantBonuses[(Merchants)merchantIndex].xpRewardBonus; // HARDCODED FOR TESTING
        return result;
    }

    public void OnClaimClick()
    {
        if (MoneyManager.Instance.GetCurrency(barterManager.barterCurrencyValues[chosenPriceIndex].currencyType) >= priceAmount)
        {
            MadeBarter();
            barterManager.merchantInfos[(Merchants)chosenMerchantIndex].completedBartersForMerchant++;
            if(barterManager.isMerchantSameAsLast((Merchants)chosenMerchantIndex)){
                barterManager.merchantInfos[(Merchants)chosenMerchantIndex].completedInArow++;
            }else{
                barterManager.merchantInfos[(Merchants)chosenMerchantIndex].completedInArow = 1;
            }
            OnBarterClaimed?.Invoke((Merchants)chosenMerchantIndex);
            DestroyCard();
        }
        else
        {
        }
    }

    public void DestroyCard()
    {
        if (TESTING_DONT_DESTROY) return;
        barterManager.UnsubscribeFromCard(this);
        Destroy(barterCard);
    }

    private void MadeBarter()
    {
        barterManager.BarterOfferBought((Merchants)chosenMerchantIndex, xpReward);

        float xp = barterManager.merchantInfos[(Merchants)chosenMerchantIndex].merchantXp;
        float reqXp = barterManager.merchantInfos[(Merchants)chosenMerchantIndex].requiredXp;

        xpProgressBar.SetProgress(xp / reqXp);

        UpdateUI();
    }

    private void UpdateReward(Merchants merchants)
    {
        if (merchants != (Merchants)chosenMerchantIndex) return;
        level = barterManager.merchantInfos[(Merchants)chosenMerchantIndex].merchantLevel;
        reward_txt.text = "x" + rewardAmount.ToStringSmart(0);
        Lvl_text_txt.text = level.ToString();
    }
    
    private void UpdateXpGain(Merchants merchants)
    {
        if (merchants != (Merchants)chosenMerchantIndex) return;
        level = barterManager.merchantInfos[(Merchants)chosenMerchantIndex].merchantLevel;
        float xp = barterManager.merchantInfos[(Merchants)chosenMerchantIndex].merchantXp;
        float reqXp = barterManager.merchantInfos[(Merchants)chosenMerchantIndex].requiredXp;

        Lvl_text_txt.text = "Lv." + level.ToString();
        xp_txt.text = string.Format("{0:F0} / {1:F0}",
                                                barterManager.merchantInfos[(Merchants)chosenMerchantIndex].merchantXp,
                                                barterManager.merchantInfos[(Merchants)chosenMerchantIndex].requiredXp);
        xpReward_txt.text = $"+{xpReward:F0}xp";

        xpProgressBar.SetProgress(xp / reqXp);
        xpIncreaseProgressBar.SetProgress((xpReward + barterManager.merchantInfos[(Merchants)chosenMerchantIndex].merchantXp) /
                                                   barterManager.merchantInfos[(Merchants)chosenMerchantIndex].requiredXp);
    }
    private void UpdateUI()
    {
        float xp = barterManager.merchantInfos[(Merchants)chosenMerchantIndex].merchantXp;
        float reqXp = barterManager.merchantInfos[(Merchants)chosenMerchantIndex].requiredXp;
        level = barterManager.merchantInfos[(Merchants)chosenMerchantIndex].merchantLevel;

        merchantIcon_img.sprite = barterManager.merchantInfos[(Merchants)chosenMerchantIndex].merchantIcon_sprite;

        Lvl_text_txt.text = "Lv." + level.ToString();
        price_txt.text = "x" + priceAmount.ToStringSmart(0);
        reward_txt.text = "x" + rewardAmount.ToStringSmart(0);
        xp_txt.text = string.Format("{0:F0} / {1:F0}",
                                                barterManager.merchantInfos[(Merchants)chosenMerchantIndex].merchantXp,
                                                barterManager.merchantInfos[(Merchants)chosenMerchantIndex].requiredXp);
        xpReward_txt.text = $"+{xpReward:F0}xp";

        xpProgressBar.SetProgress(xp / reqXp);
        xpIncreaseProgressBar.SetProgress((xpReward + barterManager.merchantInfos[(Merchants)chosenMerchantIndex].merchantXp) /
                                                   barterManager.merchantInfos[(Merchants)chosenMerchantIndex].requiredXp);

        priceIcon_img.sprite = barterManager.barterCurrencyValues[chosenPriceIndex].currencySprite;
        rewardIcon_img.sprite = barterManager.barterCurrencyValues[chosenRewardIndex].currencySprite;
    }
}
