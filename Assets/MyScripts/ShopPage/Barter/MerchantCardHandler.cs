using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class MerchantCardHandler : MonoBehaviour
{
    [SerializeField] private BarterManager barterManager;
    [SerializeField] UpgradeApplier upgradeApplier;
    [SerializeField] private Merchants merchant;
    [SerializeField] private CurrencyTypes currencyToUpgrade;

    [SerializeField] private TMP_Text pointCost_txt;
    [SerializeField] private GameObject cardObejct;
    [SerializeField] private int skillPointCost;

    public static event System.Action<MerchantCardHandler> OnAnyCardOpened;
private void Awake(){
    barterManager = GameObject.FindGameObjectWithTag("ShopPage").GetComponent<BarterManager>();
}

    private void OnEnable()
    {
        OnAnyCardOpened += HandleOtherCardOpened;
    }

    private void OnDisable()
    {
        OnAnyCardOpened -= HandleOtherCardOpened;
    }

private bool CanAfford(){
    return barterManager.merchantInfos[merchant].skillPoints >= skillPointCost;
}


    private void HandleOtherCardOpened(MerchantCardHandler openedCard)
    {
        print("inside handle other ");
        if (openedCard != this)
        {
            // Close this card if it's not the one clicked
            cardObejct.SetActive(false);
        }
    }
    public void OnOpenCardClick()
    {
        // Notify all others first
        OnAnyCardOpened?.Invoke(this);

        // Then open this card
        cardObejct.SetActive(true);
    }


public void OnUpgradeClick(){
    if(CanAfford()){
        barterManager.merchantInfos[merchant].skillPoints -= skillPointCost;
        upgradeApplier.ApplyUpgrade();
        barterManager.UpgradeBought(merchant,currencyToUpgrade);
        print("bought upgrade!");
    }else{
        print("Cannot afford upgrade!");
    }
}

private void UpdateUI(){
    pointCost_txt.text = skillPointCost.ToString();
}

}
