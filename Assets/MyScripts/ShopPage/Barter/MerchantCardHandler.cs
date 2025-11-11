using System.Runtime.CompilerServices;
using NUnit.Framework;
using TMPro;
using UnityEngine;

public class MerchantCardHandler : MonoBehaviour
{
    [SerializeField] private BarterManager barterManager;
    [SerializeField] UpgradeApplier upgradeApplier;
    [SerializeField] private Merchants merchant;
    [SerializeField] private CurrencyTypes currencyToUpgrade;

    [SerializeField] private TMP_Text pointCost_txt;
    [SerializeField] private TMP_Text header_lvl_txt;
    [SerializeField] private GameObject cardObejct;
    [SerializeField] private int skillPointCost;

    private int upgradeLevel = 0;
    [SerializeField] private int maxLevel = 10;
    public static event System.Action<MerchantCardHandler> OnAnyCardOpened;
private void Awake(){
    barterManager = GameObject.FindGameObjectWithTag("ShopPage").GetComponent<BarterManager>();
    UpdateUI();
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
    if(CanAfford() && upgradeLevel < maxLevel){
        barterManager.merchantInfos[merchant].skillPoints -= skillPointCost;
        upgradeApplier.ApplyUpgrade();
        barterManager.UpgradeBought(merchant,currencyToUpgrade);
        upgradeLevel++;
        UpdateUI();
        print("bought upgrade!");
    }else{
        print("Cannot afford upgrade or reached max lvl!");
    }
}

private void UpdateUI(){
    pointCost_txt.text =barterManager.merchantInfos[merchant].skillPoints.ToString() + "/"+ skillPointCost.ToString();
    header_lvl_txt.text = string.Format("Lv.{0:F0} / Lv.{1:F0}", upgradeLevel, maxLevel);
}

}
