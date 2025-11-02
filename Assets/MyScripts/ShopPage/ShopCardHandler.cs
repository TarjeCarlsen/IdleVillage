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
    [SerializeField] private Color percentColor;
    private Color originalPercentColor;
    private int amountOfCustomersInterested = 1; //currently setting amount of interested statically in the scritp, change to get affected
                                             //by upgrades to the shop that make more customers want the items to improve
    [SerializeField]private float timeBetweenSellChecks = 1f; // static time check between listings, can be changed with upgradable                                             
        [Header("Chance Settings")]
    [SerializeField, Tooltip("Controls how steeply chance falls as price increases")]
    private float steepness = 2f;

    [SerializeField, Tooltip("Price ratio where chance is roughly 50%")]
    private float midpoint = 1.5f;

    [SerializeField, Tooltip("Minimum possible chance (never reaches 0)")]
    private float minChance = 0.001f;
    private double chance;
    private string time;
    private Coroutine ListingCoroutine;

    private void Awake(){
        sliderHandlerPrice.SetMaxValueFromScript(startPrice * maxAdjustPriceMulti);
        originalPercentColor = percent_txt.color;
        time = "00:00";
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
        StartListing();
    }
    public void OnTimeButtonClicked(float timeFloat){
        time = HelperFunctions.Instance.ConvertSecondsToTime(timeFloat);
        UpdateUI();
    }

    private void CalculatePercent()
    {
        AlphabeticNotation currentPrice = sliderHandlerPrice.sliderValue;

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
        // percent_txt.text = $"{percent:F1}%";

        // Optional debug or display example
        //ItemSold();
        // You could simulate sale result here if needed:
        // bool sold = Random.value <= chance;
        // result_txt.text = sold ? "Sold!" : "Not sold";
    }

    private bool ItemSold(){
        float randomValue = Random.value;
        print($"chance was {chance} rolled - {randomValue}");
        return randomValue <= chance;
    }

    private void StartListing(){
        if(ListingCoroutine == null){
            ListingCoroutine = StartCoroutine(CheckForSoldItem());
        }
    }

    private void CloseListing(){
        if(ListingCoroutine != null){
            StopCoroutine(CheckForSoldItem());
            ListingCoroutine = null;
        }
    }

    private IEnumerator CheckForSoldItem(){ // coroutine should be placed on the item listing itself, not shopcard
        while(true){
        yield return new WaitForSeconds(timeBetweenSellChecks);
        CalculatePercent();
        if(ItemSold()){
            CloseListing();
            print("item sold!");
            break;
        }
            print("item Not sold!");
        }
    }

    private void UpdateUI(){
        marketPrice_txt.text = startPrice.ToStringSmart(1);
        result_txt.text = (sliderHandlerPrice.sliderValue * sliderHandlerAmount.sliderValue).ToStringSmart(1);
        time_txt.text = time;

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
