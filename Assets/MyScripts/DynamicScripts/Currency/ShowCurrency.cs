using LargeNumbers;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class ShowCurrency : MonoBehaviour
{
    [SerializeField] private ProgressBarHandler progressBarHandler;
    [SerializeField] private CurrencyTypes currencyTypes;
    [SerializeField] private TMP_Text currencyText;
    [SerializeField] private bool useStorageValues = false;
    [SerializeField] private bool useProgressBarRelativeToStorage = false;
    
    private void OnEnable(){
        MoneyManager.Instance.OnCurrencyChanged += UpdateCurrency;
        UpdateCurrency(currencyTypes);
    }
    private void OnDisable(){
        MoneyManager.Instance.OnCurrencyChanged -= UpdateCurrency;
    }



    private void UpdateCurrency(CurrencyTypes types){
        if(types != currencyTypes) return;
        if(useStorageValues){
        currencyText.text = MoneyManager.Instance.GetCurrency(currencyTypes).ToStringSmart(1)
        + "/"
        + StorageManager.Instance.GetMaxStorage(types).ToStringSmart(1);
        ;

        AlphabeticNotation current = MoneyManager.Instance.GetCurrency(types);
        AlphabeticNotation max = StorageManager.Instance.GetMaxStorage(types);

        AlphabeticNotation ratio = current/max;
        float fill = HelperFunctions.Instance.GetFill01(current,max);
        progressBarHandler.SetProgress(fill);
        print("fill = "+ fill);
        print($"current money = {MoneyManager.Instance.GetCurrency(types)} storage = {StorageManager.Instance.GetMaxStorage(types)}");
        print($"current percent filled = {MoneyManager.Instance.GetCurrency(types) / StorageManager.Instance.GetMaxStorage(types) *100}");
        }else{
        currencyText.text = MoneyManager.Instance.GetCurrency(currencyTypes).ToStringSmart(1);
    }
        }
}
