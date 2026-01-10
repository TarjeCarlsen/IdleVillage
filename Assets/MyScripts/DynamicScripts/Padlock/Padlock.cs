using LargeNumbers;
using UnityEngine;
using UnityEngine.UI;

public class Padlock : MonoBehaviour
{
    [SerializeField] public PadlockInfo padlockInfo; 
    [SerializeField] private GameObject objectToSetInactive;
    [SerializeField] private Animator padlockAnim;
    [SerializeField] private Image padlockImage;
    [SerializeField] private Image padlockButton;
[System.Serializable]
public class PadlockInfo{
    [SerializeField] public CurrencyTypes priceType;
    [SerializeField] public AlphabeticNotation priceAmount;

}



public void OnUnlockClicked(){
    AlphabeticNotation price = padlockInfo.priceAmount;
    CurrencyTypes type = padlockInfo.priceType;

    if(MoneyManager.Instance.GetCurrency(padlockInfo.priceType) >= price){
        print("can afford unlock");
        MoneyManager.Instance.SubtractCurrency(type, price);
        padlockAnim.SetTrigger("UnlockPadlock");
        padlockImage.raycastTarget = false;
        padlockButton.raycastTarget = false;
    }
}

public void DestroyPadlock(){
        objectToSetInactive.SetActive(false);
}
}
