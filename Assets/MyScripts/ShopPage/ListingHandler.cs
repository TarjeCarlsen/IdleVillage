using System.Collections;
using LargeNumbers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ListingHandler : MonoBehaviour
{
    [SerializeField] private TMP_Text header_txt;
    [SerializeField] private TMP_Text time_txt;
    [SerializeField] private TMP_Text amount_txt;
    [SerializeField] private GameObject cancellButton;
    [SerializeField] private GameObject collectButton;
    [SerializeField] private GameObject soldImage;
    // [SerializeField] private GameObject listingObject;
    public double chance;
    public AlphabeticNotation cancelAmount;
    public CurrencyTypes cancelCurrency;
    public AlphabeticNotation sellingAmount;
    private Coroutine ListingCoroutine;
    private Coroutine timerCoroutine;
    private float timeRemaining;

    [SerializeField] private float totalListingTime = 10f;
    [SerializeField] private float timeBetweenSellChecks = 1f; // static time check between listings, can be changed with upgradable                                             


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
    public void SetSellingAmount(AlphabeticNotation amount) 
    {
        sellingAmount = amount;
        amount_txt.text = amount.ToStringSmart(1);
    }
    public AlphabeticNotation GetSellingAmount() => sellingAmount;

    private void Awake(){
        if(itemDidSell) StopActiveListing();
    }
    private void Start()
    {
        if(timeRemaining == 0){
        timeRemaining = totalListingTime;
        }

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
        yield return new WaitForSeconds(timeBetweenSellChecks);

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

    private void UpdateUI()
    {
        if (itemDidSell)
        {
            header_txt.text = "Sold!";
            collectButton.SetActive(true);
            cancellButton.SetActive(false);
            soldImage.SetActive(true);
        }else{
            header_txt.text = "Time remaining";
            collectButton.SetActive(false);
            cancellButton.SetActive(true);
            soldImage.SetActive(false);
        }
    }
}
