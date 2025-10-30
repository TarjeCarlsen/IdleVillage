using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TesterButtons : MonoBehaviour
{

    [SerializeField] private List<TESTERBUTTONDATA> buttonData;

    [System.Serializable]
    public class TESTERBUTTONDATA{
        public BigNumber amount;
        public CurrencyTypes currencyType;
    }


    public void OnButtonClick(){
        foreach(TESTERBUTTONDATA data in buttonData){
            MoneyManager.Instance.AddCurrency(data.currencyType, data.amount);
        }
    }
}
