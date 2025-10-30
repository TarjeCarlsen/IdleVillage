using System.Collections.Generic;
using UnityEngine;
using LargeNumbers;
using System;

public class TesterButtons : MonoBehaviour
{

    [SerializeField] private List<TESTERBUTTONDATA> buttonData;




//testing largenumbers
    // private void Start(){
    //     LargeNumber testingLargeNumber = testLargeValue;
    //     AlphabeticNotation testingAlphabetic = testAlphaValue;


    //     string myLargeString = testingLargeNumber.ToString();
    //     string myAlphaString = testAlphaValue.ToString();



    //     LargeNumber largeBack =new LargeNumber(Double.Parse(myLargeString));
    //     AlphabeticNotation alphaBack = new AlphabeticNotation(double.Parse(myAlphaString));

    // }

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
