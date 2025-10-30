using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using LargeNumbers;
using LargeNumbers.Example;

public class BakeryManager : MonoBehaviour
{
    [SerializeField] private TMP_Text flourCounter_txt;
    [SerializeField] private TMP_Text flourToDoughCounter_txt;
    [SerializeField] private TMP_Text doughInsideFurnace_txt;
    [SerializeField] private TMP_Text breadDone_txt;
    [SerializeField] private GameObject doughImageObject;
    [SerializeField] private GameObject fillStagesObject;
    [SerializeField] private GameObject flourFullImage;
    [SerializeField] private GameObject flourEmptyImage;

    [SerializeField] private Animator doughToCrateAnim;
    [SerializeField] private Animator cookingAnimator;

    private AlphabeticNotation flourCounter;
    private AlphabeticNotation flourToDoughCounter;
    public AlphabeticNotation doughInsideFurCounter;

    private bool BreadDone;
    private AlphabeticNotation breadDoneAmount;
    public void SetBreadDoneAmount(AlphabeticNotation amount) => breadDoneAmount = amount;
    private void Start(){
        UpdateUI();
    }
    public void AddFlourToBowl()
    {
        AlphabeticNotation maxStorage = StorageManager.Instance.GetMaxStorage(CurrencyTypes.flour);
        AlphabeticNotation amountToAdd = CalculateAmountLeft(CurrencyTypes.flour, SpecialUpgradeTypes.flourDragAmount);
        if (flourCounter + amountToAdd > maxStorage) return;
        if (amountToAdd > 0)
        {
            flourCounter += amountToAdd;
            MoneyManager.Instance.SubtractCurrency(CurrencyTypes.flour, amountToAdd);
            UpdateUI();
        }
    }

public void AddFlourToDough()
{
    AlphabeticNotation maxStorage = StorageManager.Instance.GetMaxSpecialStorage(SpecialStorageType.flourPerDoughCap);
    AlphabeticNotation amountToAdd = BowlCalculateAmountLeft(SpecialUpgradeTypes.flourToDoughClickPower);

    if (amountToAdd <= 0)
        return;

    flourToDoughCounter += amountToAdd;
    flourCounter -= amountToAdd;

    if (flourToDoughCounter >= maxStorage)
    {
        MoneyManager.Instance.AddCurrency(CurrencyTypes.dough, new AlphabeticNotation(1));
        doughToCrateAnim.Play("PlusOneDough");
        flourToDoughCounter = new AlphabeticNotation(0);
    }

    UpdateUI();
}

    public void AddDoughToFurnace()
    {
        if(BreadDone) return;
        AlphabeticNotation maxStorage = StorageManager.Instance.GetMaxSpecialStorage(SpecialStorageType.furnaceStorageCap);
        AlphabeticNotation amountToAdd = DoughCalculateAmountLeft(SpecialUpgradeTypes.doughDragAmount);
        if (doughInsideFurCounter + amountToAdd > maxStorage) return;
        if (amountToAdd > 0)
        {
            doughInsideFurCounter += amountToAdd;
            MoneyManager.Instance.SubtractCurrency(CurrencyTypes.dough, amountToAdd);
            cookingAnimator.SetBool("DoughInside",true);
            UpdateUI();
        }
    }

    private AlphabeticNotation CalculateAmountLeft(CurrencyTypes type, SpecialUpgradeTypes specialType)
    {
        AlphabeticNotation maxStorage = StorageManager.Instance.GetMaxStorage(type);
        AlphabeticNotation dragAmount = HelperFunctions.Instance.GetLeftover(
            UpgradeManager.Instance.GetSpecialProductionAmount(specialType),
            MoneyManager.Instance.GetCurrency(type)
        );
        AlphabeticNotation spaceLeft = maxStorage - flourCounter;
        AlphabeticNotation amountToAdd = AlphabeticNotationUtils.Min(dragAmount, spaceLeft);

        return amountToAdd;
    }

    private AlphabeticNotation BowlCalculateAmountLeft(SpecialUpgradeTypes specialType)
    {
        AlphabeticNotation maxStorage = StorageManager.Instance.GetMaxSpecialStorage(SpecialStorageType.flourPerDoughCap);
        AlphabeticNotation clickAmount = HelperFunctions.Instance.GetLeftover(
            UpgradeManager.Instance.GetSpecialProductionAmount(specialType),
            flourCounter
        );
        AlphabeticNotation spaceLeft = maxStorage - flourToDoughCounter;
        AlphabeticNotation amountToAdd = AlphabeticNotationUtils.Min(clickAmount, spaceLeft);
        return amountToAdd;
    }

    private AlphabeticNotation DoughCalculateAmountLeft(SpecialUpgradeTypes specialtype)
    {
        AlphabeticNotation maxStorage = StorageManager.Instance.GetMaxSpecialStorage(SpecialStorageType.furnaceStorageCap);
        AlphabeticNotation doughDragAmount = HelperFunctions.Instance.GetLeftover(
            UpgradeManager.Instance.GetSpecialProductionAmount(specialtype),
            MoneyManager.Instance.GetCurrency(CurrencyTypes.dough)
        );

        AlphabeticNotation spaceLeft = maxStorage - doughInsideFurCounter;
        AlphabeticNotation amountToAdd = AlphabeticNotationUtils.Min(doughDragAmount, spaceLeft);

        return amountToAdd;

    }
    public void StartCookingAnim(){
        cookingAnimator.SetBool("TurnedOn",true);
       cookingAnimator.SetBool("DoughInside", false);
        cookingAnimator.SetBool("Empty",false);
        cookingAnimator.SetBool("BreadDone",false);
    }
    public void StopCookingAnim(){
        cookingAnimator.SetBool("BreadDone",true);
        cookingAnimator.SetBool("TurnedOn",false);
       cookingAnimator.SetBool("DoughInside", false);
        cookingAnimator.SetBool("Empty",false);
        BreadDone = true;
    }
    public void HarvestBreadAnim(){
        cookingAnimator.SetBool("Empty",true);
        cookingAnimator.SetBool("BreadDone",false);
        BreadDone = false;

    }

    public void UpdateUI()
    {
        print("storage = "+ StorageManager.Instance.GetMaxStorage(CurrencyTypes.money));
        flourCounter_txt.text = ConvertNumbers.Instance.FormatNumber(flourCounter).ToString() + "/" + ConvertNumbers.Instance.FormatNumber(StorageManager.Instance.GetMaxStorage(CurrencyTypes.flour));
        flourToDoughCounter_txt.text = ConvertNumbers.Instance.FormatNumber(flourToDoughCounter).ToString() + "/" +ConvertNumbers.Instance.FormatNumber(StorageManager.Instance.GetMaxSpecialStorage(SpecialStorageType.flourPerDoughCap)).ToString();
        doughInsideFurnace_txt.text = ConvertNumbers.Instance.FormatNumber(doughInsideFurCounter).ToString() + "/" + ConvertNumbers.Instance.FormatNumber(StorageManager.Instance.GetMaxSpecialStorage(SpecialStorageType.furnaceStorageCap));
        breadDone_txt.text = ConvertNumbers.Instance.FormatNumber(breadDoneAmount).ToString();

        if(MoneyManager.Instance.GetCurrency(CurrencyTypes.flour) > new AlphabeticNotation(0.5,0)){
            flourFullImage.SetActive(true);
            flourEmptyImage.SetActive(false);

        }else{
            flourEmptyImage.SetActive(true);
            flourFullImage.SetActive(false);
        }
    }
}
