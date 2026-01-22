using System.Collections.Generic;
using LargeNumbers;
using TMPro;
using UnityEngine;

public class SaleCardHandler : MonoBehaviour
{

[SerializeField] private PriceInfo priceinfo;
[SerializeField] private GameObject prefabFarm;

[SerializeField] private FarmCreator farmCreator;
[SerializeField] private TMP_Text price_txt;



[System.Serializable]
public class PriceInfo{
    public AlphabeticNotation price;
    public CurrencyTypes priceType;
}

private void Awake(){
    farmCreator = GameObject.FindGameObjectWithTag("ShopSaleCards").GetComponent<FarmCreator>();
}
private void Start(){
    
    price_txt.text = priceinfo.price.ToString();
}

public void OnBuyClick(){
    if(MoneyManager.Instance.GetCurrency(priceinfo.priceType) >= priceinfo.price){
        MoneyManager.Instance.SubtractCurrency(priceinfo.priceType, priceinfo.price);
        farmCreator.CreateFarm(prefabFarm);
    }

}

private void GetFarm(){

}
}
