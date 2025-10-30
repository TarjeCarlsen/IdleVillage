using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using LargeNumbers;
using LargeNumbers.Example;
using System;
using System.Collections.Generic;

public class BakeryManager : MonoBehaviour
{
    [SerializeField] private TMP_Text flourCounter_txt;
    [SerializeField] private TMP_Text flourToDoughCounter_txt;
    [SerializeField] private TMP_Text doughInsideFurnace_txt;
    [SerializeField] private TMP_Text breadDone_txt;
    // [SerializeField] private GameObject doughImageObject;
    // [SerializeField] private GameObject fillStagesObject;
    [SerializeField] private GameObject bowl;
    [SerializeField] private GameObject doughPress;
    [SerializeField] private List<GameObject> fillStages;
    [SerializeField] private GameObject flourFullImage;
    [SerializeField] private GameObject flourEmptyImage;

    [SerializeField] private Animator doughToCrateAnim;
    [SerializeField] private Animator cookingAnimator;
    [SerializeField] private Animator doughPressAnimator;

    public event Action OnFlourDropped;

    public AlphabeticNotation flourCounter;
    private AlphabeticNotation flourToDoughCounter;
    public AlphabeticNotation doughInsideFurCounter;

    private AlphabeticNotation prevCounter;
    private bool BreadDone;
    private AlphabeticNotation breadDoneAmount;
    public void SetBreadDoneAmount(AlphabeticNotation amount) => breadDoneAmount = amount;

    private void Start(){
        UpdateUI();
    }

    public void OnEnable(){
        UpgradeManager.Instance.OnActivationUnlock += ActivateDoughPress;
    }
    public void OnDisable(){
        UpgradeManager.Instance.OnActivationUnlock -= ActivateDoughPress;
    }
    
    public void AddFlourToBowl()
    {
        prevCounter = flourCounter;

        AlphabeticNotation maxStorage = StorageManager.Instance.GetMaxStorage(CurrencyTypes.flour);
        AlphabeticNotation amountToAdd = CalculateAmountLeft(CurrencyTypes.flour, SpecialUpgradeTypes.flourDragAmount);
        if (flourCounter + amountToAdd > maxStorage) return;
        if (amountToAdd > 0)
        {
            flourCounter += amountToAdd;
            MoneyManager.Instance.SubtractCurrency(CurrencyTypes.flour, amountToAdd);
            UpdateUI();
            OnFlourDropped?.Invoke();
        }
    }

public void AddFlourToDough()
{
    prevCounter = flourCounter;

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
        // Debug.Log($"{name} animator id: {cookingAnimator.GetInstanceID()} on object {cookingAnimator.gameObject.name}");
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

    private void ActivateDoughPress(){
        doughPress.SetActive(true);
        bowl.SetActive(false);
        UpgradeManager.Instance.OnActivationUnlock -= ActivateDoughPress;
    }

    private void FillStagesBowl(){

        for (AlphabeticNotation i = prevCounter; i < flourCounter; i=i+1)
        {
            int index = (int)i;
            if (index >= fillStages.Count)
                break;
    print($"prevcounter = " + prevCounter);
            fillStages[index].SetActive(true);
        }
    }

private void EmptyStagesBowl()
{
    for (AlphabeticNotation i = prevCounter - 1; i >= flourCounter; i = i - 1)
    {
        int index = (int)i;
        if (index < 0 || index >= fillStages.Count)
            continue;

        fillStages[index].SetActive(false);
    }
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

    public void StartPressAnim(){
        doughPressAnimator.SetBool("hasFlour", true);
    }
    public void StopPressAnim(){
        doughPressAnimator.SetBool("hasFlour", false);
    }

    public void UpdateUI()
    {
        flourCounter_txt.text = flourCounter.ToString() + "/" + StorageManager.Instance.GetMaxStorage(CurrencyTypes.flour);
        flourToDoughCounter_txt.text = flourToDoughCounter.ToString() + "/" +StorageManager.Instance.GetMaxSpecialStorage(SpecialStorageType.flourPerDoughCap).ToString();
        doughInsideFurnace_txt.text = doughInsideFurCounter.ToString() + "/" + StorageManager.Instance.GetMaxSpecialStorage(SpecialStorageType.furnaceStorageCap);
        breadDone_txt.text = breadDoneAmount.ToString();

        if(MoneyManager.Instance.GetCurrency(CurrencyTypes.flour) > new AlphabeticNotation(0.5,0)){
            flourFullImage.SetActive(true);
            flourEmptyImage.SetActive(false);

        }else{
            flourEmptyImage.SetActive(true);
            flourFullImage.SetActive(false);
        }

    if(UpgradeManager.Instance.GetActivationUnlock(ActivationUnlocks.doughpress)){
        ActivateDoughPress();
    }else{

        if (flourCounter > prevCounter)
        {
            FillStagesBowl();
        }
        else if (flourCounter < prevCounter)
        {
            EmptyStagesBowl();
        }
    }
    }
}
