using System.Collections;
using LargeNumbers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class ListingHandler : MonoBehaviour
{
    [SerializeField] private TMP_Text statAmount_txt; // - stat selling amount box
    [SerializeField] private TMP_Text percentageCustomers_txt; // - percentage within customer box
    [SerializeField] private Sprite sellingIconSprite;
    [SerializeField] private Sprite expiredIconSprite;
    [SerializeField] private Image originalCurrencyUsed;
    [SerializeField] private Image expiredCurrencyCollect;
    [SerializeField] private Image resultIcon;
    [SerializeField] private TMP_Text header_txt;
    [SerializeField] private TMP_Text time_txt;
    [SerializeField] private TMP_Text amount_txt;
    [SerializeField] private TMP_Text amountOfCustomers_txt;
    [SerializeField] private GameObject cancellButton;
    [SerializeField] private GameObject collectButton;
    [SerializeField] private GameObject expiredButton;
    [SerializeField] private GameObject soldImage;
    // [SerializeField] private GameObject listingObject;
    public double chance;
    public AlphabeticNotation cancelAmount;
    public CurrencyTypes cancelCurrency;
    public AlphabeticNotation sellingAmount;
    private Coroutine ListingCoroutine;
    private Coroutine timerCoroutine;
    private float timeRemaining;
    private Color percentColor;
    private Color originalPercentColor;

    // [SerializeField] private float totalListingTime = 10f;
    [SerializeField] private float timeBetweenSellChecks;  
    private int amountOfCustomersInterested;                                       

    public void SetTime(float amount) => timeRemaining = amount;
    public float GetCurrentTime() => timeRemaining;
    public void SetChance(double amount) => chance = amount;
    public void SetCancelAmount(AlphabeticNotation amount) => cancelAmount = amount;
    public void SetCancelCurrency(CurrencyTypes type) => cancelCurrency = type;
    public string uniqueID;
    public string SetUniqueID(string id) => uniqueID = id;
    public string GetUniqueID() => uniqueID;
    public bool itemDidSell = false;
    public void SetListingSold(bool state) => itemDidSell = state;
    public bool GetListingSold() => itemDidSell;
    public int GetAmountCustomers() => amountOfCustomersInterested;
    public int SetAmountCustomers(int amount) => amountOfCustomersInterested = amount;
    public void SetSellingAmount(AlphabeticNotation amount) 
    {
        sellingAmount = amount;
        amount_txt.text = amount.ToStringSmart(1);
    }
    public AlphabeticNotation GetSellingAmount() => sellingAmount;

    private void Awake(){
        if(itemDidSell) StopActiveListing();
        resultIcon.sprite = sellingIconSprite;
        originalPercentColor = percentageCustomers_txt.color;
    }
    private void Start()
    {
        // if(timeRemaining == 0){
        // timeRemaining = totalListingTime;
        // }
        originalCurrencyUsed.sprite = expiredIconSprite;
        expiredCurrencyCollect.sprite = expiredIconSprite;
        timeBetweenSellChecks = UpgradeManager.Instance.GetFloat(UpgradeIDGlobal.market_time_between_customers,CurrencyDummy.Dummy);
        StartCoroutine(WaitForOneFrame());
    }

    private IEnumerator WaitForOneFrame(){
        yield return null;
        StartListing();
        UpdateUI();
    }

    public void OnCancelButtonClicked(){
        MoneyManager.Instance.AddCurrency(cancelCurrency,cancelAmount);
        StopActiveListing();
        Destroy(gameObject);
        ShopManager.Instance.RemoveListing(uniqueID);
    }
    public void OnCollectButtonClicked(){
        MoneyManager.Instance.AddCurrency(CurrencyTypes.money, sellingAmount);
        ShopManager.Instance.UpdateCollectAmount(sellingAmount, false);
        StopActiveListing();
        Destroy(gameObject);
        ShopManager.Instance.RemoveListing(uniqueID);
    }

    private bool ItemSold()
    {
        float randomValue = Random.value;
        return randomValue <= chance;
    }

    private void StartListing()
    {
        if(itemDidSell) return;
        if (ListingCoroutine == null)
        {
            ListingCoroutine = StartCoroutine(CheckForSoldItem());
        }
        if (timerCoroutine == null)
        {
            timerCoroutine = StartCoroutine(UpdateTimer());
        }
    }

    public void StopActiveListing()
    {
        if (ListingCoroutine != null)
        {
            StopCoroutine(CheckForSoldItem());
            ListingCoroutine = null;
        }
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }
        UpdateUI();
    }
private IEnumerator CheckForSoldItem()
{
    while (timeRemaining > 0f && !itemDidSell)
    {
    float randomTimeBetweenChecks = UnityEngine.Random.Range(0, timeBetweenSellChecks); // = 2s
        yield return new WaitForSeconds(randomTimeBetweenChecks); // venter i ^
        amountOfCustomersInterested++; // USE THIS FOR DISPLAYING AMOUNT OF BUYERS INTERESTED
        if(amountOfCustomers_txt != null)amountOfCustomers_txt.text = amountOfCustomersInterested.ToString();
        itemDidSell = ItemSold();
        if (itemDidSell)
        {
            ShopManager.Instance.UpdateCollectAmount(sellingAmount,true);
            ShopManager.Instance.UpdateListing(uniqueID,true);
            StopActiveListing();
            yield break;
        }

    }

    StopActiveListing(); // time ran out
}

private IEnumerator UpdateTimer()
{
    while (timeRemaining > 0f && !itemDidSell)
    {
        time_txt.text = HelperFunctions.Instance.ConvertSecondsToTime(timeRemaining);
        yield return new WaitForSeconds(1f);

        timeRemaining -= 1f;
        if (timeRemaining < 0f)
            timeRemaining = 0f;
    }

    time_txt.text = HelperFunctions.Instance.ConvertSecondsToTime(0f);
}

private void PercentVisuals(){
    double percent = chance * 100f;
    if(percent < 0.1f){
    percentageCustomers_txt.text = $"<0.1%";
    }else{
    percentageCustomers_txt.text = $"{percent:F1}%";
    }

    if (percent <= 0f)
    {
        // restore the original TMP color
        percentageCustomers_txt.color = originalPercentColor;
    }
    // pick color based on percent range
    if (percent >= 80f)
        percentageCustomers_txt.color = new Color(0f, 1f, 0f);               // bright green
    else if (percent >= 60f)
        percentageCustomers_txt.color = new Color(0.4f, 1f, 0.4f);           // softer green
    else if (percent >= 40f)
        percentageCustomers_txt.color = new Color(1f, 0.65f, 0f);            // orange
    else if (percent >= 20f)
        percentageCustomers_txt.color = new Color(0.8f, 0.2f, 0.2f);         // subtle red
    else
        percentageCustomers_txt.color = new Color(1f, 0f, 0f);               // bright red
        
}

    private void UpdateUI()
    {
        if (itemDidSell)
        {
            header_txt.text = "";
            collectButton.SetActive(true);
            cancellButton.SetActive(false);
            expiredButton.SetActive(false);
            soldImage.SetActive(true);
        }else{
            header_txt.text = "Time remaining";
            collectButton.SetActive(false);
            expiredButton.SetActive(false);
            cancellButton.SetActive(true);
            soldImage.SetActive(false);
            if(timeRemaining == 0){
            header_txt.text = "Offer Expired!";
            expiredButton.SetActive(true);
            cancellButton.SetActive(false);
            collectButton.SetActive(false);
            resultIcon.sprite = expiredIconSprite;
            amount_txt.text = cancelAmount.ToStringSmart(1);
            }
        }
        PercentVisuals();
        statAmount_txt.text = cancelAmount.ToStringSmart(1);
        amountOfCustomers_txt.text = amountOfCustomersInterested.ToString();
    }
}
