using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BakeryManager : MonoBehaviour
{
    [SerializeField] private TMP_Text flourCounter_txt;
    [SerializeField] private TMP_Text flourToDoughCounter_txt;
    [SerializeField] private GameObject doughImageObject;
    [SerializeField] private GameObject fillStagesObject;

    [SerializeField] private int flourPerDough;
    private BigNumber flourCounter;
    private BigNumber flourToDoughCounter;
    private bool doughMade;


public void AddFlourToBowl()
{
    if (doughMade) return;
    BigNumber maxStorage = StorageManager.Instance.GetMaxStorage(CurrencyTypes.flour);
    BigNumber amountToAdd = CalculateAmountLeft(CurrencyTypes.flour, SpecialUpgradeTypes.flourDragAmount);

    if(flourCounter + amountToAdd > maxStorage) return;
        if (amountToAdd > 0)
    {
        flourCounter += amountToAdd;
        MoneyManager.Instance.SubtractCurrency(CurrencyTypes.flour, amountToAdd);
        UpdateUI();
    }
}

public void AddFlourToDough() // stopped here, figure out how to not use 15 flour when only spce for 10
{
    if (doughMade) return;

    BigNumber maxStorage = flourPerDough;
    BigNumber amountToAdd = BowlCalculateAmountLeft(SpecialUpgradeTypes.flourToDoughClickPower);

    if(flourToDoughCounter + amountToAdd >= maxStorage){
        doughMade = true;
        flourToDoughCounter = 0;
        flourCounter = flourCounter - amountToAdd;
    }else{
        flourToDoughCounter = flourToDoughCounter + amountToAdd;
        flourCounter = flourCounter - amountToAdd;
    }
    UpdateUI();
}

    private BigNumber CalculateAmountLeft(CurrencyTypes type,SpecialUpgradeTypes specialType){
        BigNumber maxStorage = StorageManager.Instance.GetMaxStorage(type);
        BigNumber dragAmount = HelperFunctions.Instance.GetLeftover(
            UpgradeManager.Instance.GetSpecialProductionAmount(specialType),
            MoneyManager.Instance.GetCurrency(type)
        );
        BigNumber spaceLeft = maxStorage - flourCounter;
        BigNumber amountToAdd = BigNumber.Min(dragAmount,spaceLeft);

        return amountToAdd;
    }

private BigNumber BowlCalculateAmountLeft(SpecialUpgradeTypes specialType)
{
    BigNumber maxStorage = flourPerDough;
    BigNumber clickAmount = HelperFunctions.Instance.GetLeftover(
        UpgradeManager.Instance.GetSpecialProductionAmount(specialType),
        flourCounter
    );
    BigNumber spaceLeft = maxStorage - flourToDoughCounter;
    BigNumber amountToAdd = BigNumber.Min(clickAmount,spaceLeft);
    return amountToAdd;
}

    private void UpdateUI()
    {
        flourCounter_txt.text = flourCounter.ToString() + "/" + StorageManager.Instance.GetMaxStorage(CurrencyTypes.flour);
        flourToDoughCounter_txt.text = flourToDoughCounter.ToString() +"/" + flourPerDough.ToString();
        if(doughMade){
            doughImageObject.SetActive(true);
            fillStagesObject.SetActive(false);
        }
    }
}
