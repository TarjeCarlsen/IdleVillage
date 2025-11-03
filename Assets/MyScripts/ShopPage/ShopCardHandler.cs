using System.Collections;
using LargeNumbers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopCardHandler : MonoBehaviour
{
    [SerializeField] private TMP_Text marketPrice_txt;
    [SerializeField] private TMP_Text time_txt;
    [SerializeField] private TMP_Text percent_txt;
    [SerializeField] private TMP_Text result_txt;

    [SerializeField] private AlphabeticNotation startPrice;
    [SerializeField] private AlphabeticNotation maxAdjustPriceMulti = new AlphabeticNotation(10);
    [SerializeField] private SliderHandler sliderHandlerPrice;
    [SerializeField] private SliderHandler sliderHandlerAmount;
    [SerializeField] private GameObject prefabListingToCreate;
    [SerializeField] private Transform parentToSpawnUnder;
    [SerializeField] private Color percentColor;
    [SerializeField]private float defaultTime = 60; 
    private AlphabeticNotation currentPrice;
    private AlphabeticNotation result;

    private Color originalPercentColor;
    [SerializeField, Tooltip("Controls how steeply chance falls as price increases")]
    private float steepness = 2f;

    [SerializeField, Tooltip("Price ratio where chance is roughly 50%")]
    private float midpoint = 1.5f;

    [SerializeField, Tooltip("Minimum possible chance (never reaches 0)")]
    private float minChance = 0.001f;
    private double chance;
    private float rawTimeFloat = 60;
    private string time;

    private void Awake(){

        sliderHandlerPrice.SetMaxValueFromScript(startPrice * maxAdjustPriceMulti);
        originalPercentColor = percent_txt.color;
    }

    private void Start(){
        time = HelperFunctions.Instance.ConvertSecondsToTime(rawTimeFloat);
        UpdateUI();
    }
    private void OnEnable(){
        sliderHandlerPrice.OnSliderDragging += CalculatePercent;
        sliderHandlerPrice.OnStoppedSliderDrag += CalculatePercent;        
        sliderHandlerPrice.OnStoppedSliderDrag += UpdateUI;        
        sliderHandlerAmount.OnStoppedSliderDrag += UpdateUI;        
    }
    private void OnDisable(){
        sliderHandlerPrice.OnSliderDragging -= CalculatePercent;
        sliderHandlerPrice.OnStoppedSliderDrag -= CalculatePercent;        
        sliderHandlerPrice.OnStoppedSliderDrag -= UpdateUI;        
        sliderHandlerAmount.OnStoppedSliderDrag -= UpdateUI;        
    }

    public void OnListButtonClicked(){
        CreateListing();
    }

    private void CreateListing(){
        if(CanAfford() && sliderHandlerAmount.sliderValue > 0 && rawTimeFloat > 0 && sliderHandlerPrice.sliderValue > 0){
            MoneyManager.Instance.SubtractCurrency(sliderHandlerAmount.maxValueCurrencytype, sliderHandlerAmount.sliderValue);
            GameObject newListing =  Instantiate(prefabListingToCreate,parentToSpawnUnder);
            ListingHandler handler = newListing.GetComponent<ListingHandler>();
            handler.SetSellingAmount(result);
            handler.SetTime(rawTimeFloat);
            handler.SetChance(chance);
            handler.SetCancelAmount(sliderHandlerAmount.sliderValue);
            handler.SetCancelCurrency(sliderHandlerAmount.maxValueCurrencytype);
            sliderHandlerAmount.ResetSliderValues();
            sliderHandlerPrice.ResetSliderValues();
        }else{
            print("time or slider is 0. chose a time!"); //POPUP TAG. IMPLEMENT POPUP FOR CHOSE A TIME AND AMOUNT
        }
    }

    private bool CanAfford(){
            print("slider value = "+ sliderHandlerAmount.sliderValue);
        if(MoneyManager.Instance.GetCurrency(sliderHandlerAmount.maxValueCurrencytype) >= sliderHandlerAmount.sliderValue){
            return true;
        }
        return false;
    }
    public void OnTimeButtonClicked(float timeFloat){
        time = HelperFunctions.Instance.ConvertSecondsToTime(timeFloat);
        rawTimeFloat = timeFloat;
        print("time float = "+ timeFloat);
        UpdateUI();
    }

    private void CalculatePercent()
    {
        currentPrice = sliderHandlerPrice.sliderValue;

        // --- Convert to double ratio safely ---
        double marketValue = startPrice.Standard();
        double playerValue = currentPrice.Standard();

        // Protect against division by zero
        if (marketValue <= 0)
            marketValue = 0.000001d;

        double priceRatio = playerValue / marketValue;

        // --- Logistic demand curve ---
        chance = 1.0 / (1.0 + System.Math.Pow(priceRatio / midpoint, steepness));
        chance = System.Math.Max(chance, minChance);

        // --- Convert to % and display ---
        float percent = (float)(chance * 100f);
        UpdateUI();
    }

    private void UpdateUI(){
        marketPrice_txt.text = startPrice.ToStringSmart(1);
        result = sliderHandlerPrice.sliderValue * sliderHandlerAmount.sliderValue;
        result_txt.text = result.ToStringSmart(1);
        time_txt.text = time;
        print("TIME = "+ time);

            // --- Update percent text color ---
    float percent = (float)(chance * 100f);
    if (percent <= 0f)
    {
        // restore the original TMP color
        percent_txt.color = originalPercentColor;
    }
    // pick color based on percent range
    if (percent >= 80f)
        percentColor = new Color(0f, 1f, 0f);               // bright green
    else if (percent >= 60f)
        percentColor = new Color(0.4f, 1f, 0.4f);           // softer green
    else if (percent >= 40f)
        percentColor = new Color(1f, 0.65f, 0f);            // orange
    else if (percent >= 20f)
        percentColor = new Color(0.8f, 0.2f, 0.2f);         // subtle red
    else
        percentColor = new Color(1f, 0f, 0f);               // bright red

    percent_txt.color = percentColor;

    // update text (if not already done in CalculatePercent)
    percent_txt.text = $"{percent:F1}%";
    }
}
