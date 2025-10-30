using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

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

    private BigNumber flourCounter;
    private BigNumber flourToDoughCounter;
    public BigNumber doughInsideFurCounter;

    private bool BreadDone;
    private BigNumber breadDoneAmount;
    public void SetBreadDoneAmount(BigNumber amount) => breadDoneAmount = amount;
    private void Start(){
        UpdateUI();
    }
    public void AddFlourToBowl()
    {
        BigNumber maxStorage = StorageManager.Instance.GetMaxStorage(CurrencyTypes.flour);
        BigNumber amountToAdd = CalculateAmountLeft(CurrencyTypes.flour, SpecialUpgradeTypes.flourDragAmount);
        if (BigNumber.CompareWhole(flourCounter + amountToAdd, maxStorage) > 0) return;
        if (amountToAdd > 0)
        {
            flourCounter += amountToAdd;
            MoneyManager.Instance.SubtractCurrency(CurrencyTypes.flour, amountToAdd);
            UpdateUI();
        }
    }

public void AddFlourToDough()
{
    BigNumber maxStorage = StorageManager.Instance.GetMaxSpecialStorage(SpecialStorageType.flourPerDoughCap);
    BigNumber amountToAdd = BowlCalculateAmountLeft(SpecialUpgradeTypes.flourToDoughClickPower);

    if (amountToAdd <= 0)
        return;

    flourToDoughCounter += amountToAdd;
    flourCounter -= amountToAdd;

    if (BigNumber.CompareWhole(flourToDoughCounter,maxStorage) >= 0)
    {
        MoneyManager.Instance.AddCurrency(CurrencyTypes.dough, 1);
        doughToCrateAnim.Play("PlusOneDough");
        flourToDoughCounter = 0;
    }

    UpdateUI();
}

    public void AddDoughToFurnace()
    {
        if(BreadDone) return;
        BigNumber maxStorage = StorageManager.Instance.GetMaxSpecialStorage(SpecialStorageType.furnaceStorageCap);
        BigNumber amountToAdd = DoughCalculateAmountLeft(SpecialUpgradeTypes.doughDragAmount);
        if (BigNumber.CompareWhole(doughInsideFurCounter + amountToAdd, maxStorage) > 0) return;
        if (amountToAdd > 0)
        {
            doughInsideFurCounter += amountToAdd;
            MoneyManager.Instance.SubtractCurrency(CurrencyTypes.dough, amountToAdd);
            cookingAnimator.SetBool("DoughInside",true);
            UpdateUI();
        }
    }

    private BigNumber CalculateAmountLeft(CurrencyTypes type, SpecialUpgradeTypes specialType)
    {
        BigNumber maxStorage = StorageManager.Instance.GetMaxStorage(type);
        BigNumber dragAmount = HelperFunctions.Instance.GetLeftover(
            UpgradeManager.Instance.GetSpecialProductionAmount(specialType),
            MoneyManager.Instance.GetCurrency(type)
        );
        BigNumber spaceLeft = maxStorage - flourCounter;
        BigNumber amountToAdd = BigNumber.Min(dragAmount, spaceLeft);

        return amountToAdd;
    }

    private BigNumber BowlCalculateAmountLeft(SpecialUpgradeTypes specialType)
    {
        BigNumber maxStorage = StorageManager.Instance.GetMaxSpecialStorage(SpecialStorageType.flourPerDoughCap);
        BigNumber clickAmount = HelperFunctions.Instance.GetLeftover(
            UpgradeManager.Instance.GetSpecialProductionAmount(specialType),
            flourCounter
        );
        BigNumber spaceLeft = maxStorage - flourToDoughCounter;
        BigNumber amountToAdd = BigNumber.Min(clickAmount, spaceLeft);
        return amountToAdd;
    }

    private BigNumber DoughCalculateAmountLeft(SpecialUpgradeTypes specialtype)
    {
        BigNumber maxStorage = StorageManager.Instance.GetMaxSpecialStorage(SpecialStorageType.furnaceStorageCap);
        BigNumber doughDragAmount = HelperFunctions.Instance.GetLeftover(
            UpgradeManager.Instance.GetSpecialProductionAmount(specialtype),
            MoneyManager.Instance.GetCurrency(CurrencyTypes.dough)
        );

        BigNumber spaceLeft = maxStorage - doughInsideFurCounter;
        BigNumber amountToAdd = BigNumber.Min(doughDragAmount, spaceLeft);

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

        if(BigNumber.CompareWhole(MoneyManager.Instance.GetCurrency(CurrencyTypes.flour), new BigNumber(0.5,0)) >0){
            flourFullImage.SetActive(true);
            flourEmptyImage.SetActive(false);

        }else{
            flourEmptyImage.SetActive(true);
            flourFullImage.SetActive(false);
        }
    }
}
