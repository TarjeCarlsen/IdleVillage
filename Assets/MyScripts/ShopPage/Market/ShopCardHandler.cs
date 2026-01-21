using System.Collections;
using System.Runtime.InteropServices;
using LargeNumbers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopCardHandler : MonoBehaviour
{
    [SerializeField] PopUpTextHandler popUpTextHandler;
    [SerializeField] private GameObject prefabListingToCreate;
    [SerializeField] private Transform parentToSpawnUnder;
    [SerializeField] private Color percentColor;
    [SerializeField] private TMP_Text marketPrice_txt;
    [SerializeField] private TMP_Text time_txt;
    [SerializeField] private TMP_Text percent_txt;
    [SerializeField] private TMP_Text result_txt;

    [SerializeField] private AlphabeticNotation startPrice;
    [SerializeField] private AlphabeticNotation maxAdjustPriceMulti = new AlphabeticNotation(2); // HARDCODED MAX AMOUNT OF MARKETPRICE SLIDER SHOULD MOVE
    private AlphabeticNotation currentPrice;
    private AlphabeticNotation result;
    // [SerializeField]private float defaultTime = 60; 
    // private string uniqueID;
    private string cardName;

    [SerializeField] private SliderHandler sliderHandlerPrice;
    [SerializeField] private SliderHandler sliderHandlerAmount;


    private Color originalPercentColor;
    [SerializeField, Tooltip("Controls how steeply chance falls as price increases")]
    private float steepness = 2f;

    [SerializeField, Tooltip("Price ratio where chance is roughly 50%")]
    private float midpoint = 1.5f;

    [SerializeField, Tooltip("Minimum possible chance (never reaches 0)")]
    private float minChance = 0.00001f;
    private double chance;
    float atLeastOneSalePercent;
    private float rawTimeFloat = 60;
    private string time;

    private void Awake(){
        sliderHandlerPrice.SetMaxValueFromScript(startPrice * maxAdjustPriceMulti);
        sliderHandlerPrice.ResetSliderValues();
        sliderHandlerPrice.SetSliderSpecific(startPrice, .5f);
        originalPercentColor = percent_txt.color;
    }

    private void Start(){
        parentToSpawnUnder = GameObject.FindGameObjectWithTag("ListingContainer").GetComponent<Transform>();
        time = HelperFunctions.Instance.ConvertSecondsToTime(rawTimeFloat);
        cardName = gameObject.name;
        CalculatePercent();
        UpdateUI();
    }
    private void OnEnable(){
        sliderHandlerPrice.OnSliderDragging += CalculatePercent;
        sliderHandlerAmount.OnSliderDragging += CalculatePercent;
        // sliderHandlerPrice.OnStoppedSliderDrag += CalculatePercent;        
        sliderHandlerPrice.OnStoppedSliderDrag += UpdateUI;        
        // sliderHandlerAmount.OnStoppedSliderDrag += UpdateUI;        
    }
    private void OnDisable(){
        sliderHandlerPrice.OnSliderDragging -= CalculatePercent;
        sliderHandlerAmount.OnSliderDragging -= CalculatePercent;
        // sliderHandlerPrice.OnStoppedSliderDrag -= CalculatePercent;        
        sliderHandlerPrice.OnStoppedSliderDrag -= UpdateUI;        
        // sliderHandlerAmount.OnStoppedSliderDrag -= UpdateUI;        
    }

    public void OnListButtonClicked(){
        CreateListing();
    }

    private void CreateListing(){
        AlphabeticNotation maxListings = StorageManager.Instance.GetMaxSpecialStorage(SpecialStorageType.shopAmountListings);
        int currentListingAmount = ShopManager.Instance.GetCurrentListingAmount();
        if(currentListingAmount >= maxListings){
            if(popUpTextHandler != null)popUpTextHandler.RunPopUp("Max listings active!");
            return;
        }
        if(CanAfford() && sliderHandlerAmount.sliderValue > 0 && rawTimeFloat > 0 && sliderHandlerPrice.sliderValue > 0){
            ShopManager.ListingData listingData= new ShopManager.ListingData();
            MoneyManager.Instance.SubtractCurrency(sliderHandlerAmount.maxValueCurrencytype, sliderHandlerAmount.sliderValue);
            GameObject newListing =  Instantiate(prefabListingToCreate,parentToSpawnUnder);
            ListingHandler handler = newListing.GetComponent<ListingHandler>();
            string uniqueID = HelperFunctions.Instance.GenerateUniqueId();

            listingData.cancelAmount = sliderHandlerAmount.sliderValue;
            listingData.cancelType = sliderHandlerAmount.maxValueCurrencytype;
            listingData.currentTime = rawTimeFloat;
            listingData.chance = chance;
            listingData.result = result;
            listingData.uniqueID = uniqueID;
            listingData.listingHandler = handler;
            listingData.shopCardName = gameObject.name;

            handler.SetSellingAmount(result);
            handler.SetTime(rawTimeFloat);
            handler.SetChance(chance);
            handler.SetCancelAmount(sliderHandlerAmount.sliderValue);
            handler.SetCancelCurrency(sliderHandlerAmount.maxValueCurrencytype);
            sliderHandlerAmount.ResetSliderValues();
            sliderHandlerPrice.ResetSliderValues();
            sliderHandlerPrice.SetSliderSpecific(startPrice, .5f);
            handler.SetUniqueID(uniqueID);
            ShopManager.Instance.AddListing(uniqueID, listingData);
            ShopManager.Instance.UpdateCollectAmount(result,true);
        }else{
            if(popUpTextHandler != null)popUpTextHandler.RunPopUp("Invalid amount chosen!");
        }
    }

    public void CreateListingFromLoad(
        AlphabeticNotation loadResult, float loadRawTimeFloat, double loadChance,
        AlphabeticNotation loadCancelAmount, CurrencyTypes loadMaxValueCurrencyType,
        string loadUniqueID, string loadShopCardName, bool loadListingSold, int loadAmountOfCustomers
        ){
        ShopManager.ListingData listingData = new ShopManager.ListingData();
        GameObject newListing = Instantiate(prefabListingToCreate,parentToSpawnUnder);
        ListingHandler handler = newListing.GetComponent<ListingHandler>();

            listingData.result = loadResult;
            listingData.currentTime = loadRawTimeFloat;
            listingData.chance = loadChance;
            listingData.cancelAmount = loadCancelAmount;
            listingData.cancelType = loadMaxValueCurrencyType;
            listingData.uniqueID = loadUniqueID;
            listingData.shopCardName = loadShopCardName;
            listingData.listingSold = loadListingSold;
            listingData.listingHandler = handler;
            listingData.amountOfCustomersInterested = loadAmountOfCustomers;

            handler.SetSellingAmount(loadResult);
            handler.SetTime(loadRawTimeFloat);
            handler.SetChance(loadChance);
            handler.SetCancelAmount(loadCancelAmount);
            handler.SetCancelCurrency(loadMaxValueCurrencyType);
            handler.SetUniqueID(loadUniqueID);
            handler.SetListingSold(loadListingSold);
            handler.SetAmountCustomers(loadAmountOfCustomers);

            if(loadListingSold)
            {
             handler.StopActiveListing();
             ShopManager.Instance.UpdateCollectAmount(loadResult,true);
             ShopManager.Instance.UpdateListing(loadUniqueID,loadListingSold);
            }

            ShopManager.Instance.AddListing(loadUniqueID, listingData);
    }

    private bool CanAfford(){
        if(MoneyManager.Instance.GetCurrency(sliderHandlerAmount.maxValueCurrencytype) >= sliderHandlerAmount.sliderValue){
            return true;
        }
        return false;
    }
    public void OnTimeButtonClicked(float timeFloat){
        time = HelperFunctions.Instance.ConvertSecondsToTime(timeFloat);
        rawTimeFloat = timeFloat;
        CalculatePercent();
        UpdateUI();
    }

    private void CalculatePercent()
    {
        float averageAmountOfCustomers = rawTimeFloat / (UpgradeManager.Instance.GetFloat(UpgradeIDGlobal.market_time_between_customers,CurrencyDummy.Dummy) / 2);
        currentPrice = sliderHandlerPrice.sliderValue;

        // --- Convert to double ratio safely ---
        double marketValue = startPrice.Standard();
        double playerValue = currentPrice.Standard();

        // Protect against division by zero
        if (marketValue <= 0)
            marketValue = 0.000001d;

                 //4           // 20              //5
        double priceRatio = playerValue / marketValue;

        // --- Logistic demand curve ---           //4   /  1,5= 2,666^1,5 = 4,35 =          1/5,35
        chance = 1.0 / (1.0 + System.Math.Pow(priceRatio / midpoint, steepness));

                                //  0,17ish    0.001    
        chance = System.Math.Max(chance, minChance);

        // --- Convert to % and display ---
        float percent = (float)(chance * 100f);
        double atLeastOneSaleChance = 1.0 - System.Math.Pow(1.0 - chance, averageAmountOfCustomers);
        atLeastOneSalePercent = (float)(atLeastOneSaleChance * 100f);

        UpdateUI();
    }

    private void UpdateUI(){
        marketPrice_txt.text = startPrice.ToStringSmart(1);
        result = sliderHandlerPrice.sliderValue * sliderHandlerAmount.sliderValue;
        result_txt.text = result.ToStringSmart(1);
        time_txt.text = time;

            // --- Update percent text color ---
    float percent = (float)(chance * 100f);
    if (atLeastOneSalePercent <= 0f)
    {
        // restore the original TMP color
        percent_txt.color = originalPercentColor;
    }
    // pick color based on percent range
    if (atLeastOneSalePercent >= 80f)
        percentColor = new Color(0f, 1f, 0f);               // bright green
    else if (atLeastOneSalePercent >= 60f)
        percentColor = new Color(0.4f, 1f, 0.4f);           // softer green
    else if (atLeastOneSalePercent >= 40f)
        percentColor = new Color(1f, 0.65f, 0f);            // orange
    else if (atLeastOneSalePercent >= 20f)
        percentColor = new Color(0.8f, 0.2f, 0.2f);         // subtle red
    else
        percentColor = new Color(1f, 0f, 0f);               // bright red

    percent_txt.color = percentColor;

    // update text (if not already done in CalculatePercent)
    percent_txt.text = $"{atLeastOneSalePercent:F1}%";
    }
}
