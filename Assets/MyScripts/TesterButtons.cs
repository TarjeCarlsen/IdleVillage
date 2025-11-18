using System.Collections.Generic;
using UnityEngine;
using LargeNumbers;
using System;

public class TesterButtons : MonoBehaviour
{

    [SerializeField] private List<TESTERBUTTONDATA> buttonData;
    [SerializeField] AlphabeticNotation value_to_test;



//testing largenumbers
    private void Start(){
        AlphabeticNotation testAlphaValue = new AlphabeticNotation(value_to_test);

        string myAlphaString = testAlphaValue.ToString();


        // AlphabeticNotation test2 = new AlphabeticNotation(Double.Parse(myAlphaString));
        AlphabeticNotation.GetAlphabeticNotationFromString(myAlphaString, out var newNumber);


    }

    [System.Serializable]
    public class TESTERBUTTONDATA{
        public LargeNumbers.AlphabeticNotation amount;
        public CurrencyTypes currencyType;
    }


    public void OnButtonClick(){
        foreach(TESTERBUTTONDATA data in buttonData){
            MoneyManager.Instance.AddCurrency(data.currencyType, data.amount);
        }
    }
}
