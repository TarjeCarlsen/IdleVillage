using System;
using LargeNumbers;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BarterCardHandler : MonoBehaviour
{
    private MerchantUpgradeManager merchantUpgradeManager;
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

    // [Header]
    [SerializeField] private bool TESTING_DONT_DESTROY;
    [SerializeField] private float randomValueModifier;
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
    private int chosenMerchantIndex;

    [SerializeField] float tradeValue;
    [SerializeField] float baseXp = 10f;
    [SerializeField] float levelFactor = 0.9f;
    [SerializeField] float valueFactor = 0.5f;
    [SerializeField] float randomFactor;



    private void Start()
    {
        barterManager = GameObject.FindGameObjectWithTag("ShopPage").GetComponent<BarterManager>();
        barterManager.OnBarterXpGain += UpdateXpGain;
        barterManager.OnBarterLevelUp += UpdateReward;

        foreach (BarterCurrencyValues barterCurrency in barterManager.barterCurrencyValues)
        {
            amountOfCurrencies++;
        }
        chosenPriceIndex = GetRandomIndex();
        chosenRewardIndex = GetRandomIndex();
        while (chosenRewardIndex == chosenPriceIndex)
        {
            chosenRewardIndex = GetRandomIndex();
        }
        priceValue = GetRandomValue(chosenPriceIndex);
        rewardValue = GetRandomValue(chosenRewardIndex);
        chosenMerchantIndex = GetRandomMerchant();
        level = barterManager.merchantInfos[(Merchants)chosenMerchantIndex].merchantLevel;
        priceAmount = GetRandomAmount(level);
        rewardAmount = CalculateReward();

        rewardAmount = ApplyBonusesToRewards(chosenMerchantIndex, chosenRewardIndex,rewardAmount);

        UpdateUI();
    }

    private void OnDisable()
    {
        barterManager.OnBarterXpGain -= UpdateXpGain;
        barterManager.OnBarterLevelUp -= UpdateReward;
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

    private int GetRandomMerchant()
    {
        int amountOfMerchants = barterManager.merchantInfos.Count;
        int randomIndex = UnityEngine.Random.Range(0, amountOfMerchants);
        return randomIndex;
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

        randomFactor = UnityEngine.Random.Range(0.7f, 1.5f);
        tradeValue = priceValue + rewardValue * (float)rewardAmount;

        level = barterManager.merchantInfos[(Merchants)chosenMerchantIndex].merchantLevel;
        xpReward = (baseXp + Mathf.Pow(tradeValue, 0.7f))
                  * Mathf.Pow(level + 1, -0.4f)
                  * randomFactor;
        originalRewardAmount = rewardAmount;
        rewardAmount = originalRewardAmount * (1 + barterManager.merchantInfos[(Merchants)chosenMerchantIndex].bonus);

        return rewardAmount;
    }

    private AlphabeticNotation ApplyBonusesToRewards(int merchantIndex,int currencyIndex, AlphabeticNotation amount){
        var bonuses = barterManager.merchantBonuses[(Merchants)merchantIndex];
        AlphabeticNotation result = (amount + bonuses.rewardBaseFlatIncrease[(CurrencyTypes)currencyIndex]) *
                                     bonuses.rewardMultiplier[(CurrencyTypes)currencyIndex];
        return result;
    }
    public void OnClaimClick()
    {
        if (MoneyManager.Instance.GetCurrency(barterManager.barterCurrencyValues[chosenPriceIndex].currencyType) >= priceAmount)
        {
            print("Made barter trade!");
            MadeBarter();
            DestroyCard();
        }
        else
        {
            print("Cant afford barter!");
        }
    }

    public void DestroyCard()
    {
        if (TESTING_DONT_DESTROY) return;
        Destroy(barterCard);
    }

    private void MadeBarter()
    {
        barterManager.BarterOfferBought((Merchants)chosenMerchantIndex, xpReward);

        float xp = barterManager.merchantInfos[(Merchants)chosenMerchantIndex].merchantXp;
        float reqXp = barterManager.merchantInfos[(Merchants)chosenMerchantIndex].requiredXp;

        xpProgressBar.SetProgress(xp / reqXp);
        // print("inside card xp = "+ barterManager.merchantInfos[(Merchants)chosenMerchantIndex].merchantXp);

        UpdateUI();
    }


    private void UpdateReward(Merchants merchants)
    {
        if (merchants != (Merchants)chosenMerchantIndex) return;
        level = barterManager.merchantInfos[(Merchants)chosenMerchantIndex].merchantLevel;
        rewardAmount = originalRewardAmount * (1 + barterManager.merchantInfos[(Merchants)chosenMerchantIndex].bonus);
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
