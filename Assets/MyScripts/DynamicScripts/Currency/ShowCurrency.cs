using TMPro;
using UnityEngine;

public class ShowCurrency : MonoBehaviour
{
    [SerializeField] private CurrencyTypes currencyTypes;
    [SerializeField] private TMP_Text currencyText;


    
    private void OnEnable(){
        MoneyManager.Instance.OnCurrencyChanged += UpdateCurrency;
    }
    private void OnDisable(){
        MoneyManager.Instance.OnCurrencyChanged -= UpdateCurrency;
    }



    private void UpdateCurrency(CurrencyTypes types){
        if(types != currencyTypes) return;
        currencyText.text = MoneyManager.Instance.GetCurrency(currencyTypes).ToStringSmart(1);
    }
}
