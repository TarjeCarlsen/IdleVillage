using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using LargeNumbers;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class GenAdvancedInfo
{
    public List<GenerateInfo> generateInfo;
    public List<GenerateInfo> payInfo;

}
[System.Serializable]
public class GenerateInfo
{
    public CurrencyTypes type;
    public AlphabeticNotation amount;
}
public class GeneratorAdvanced : MonoBehaviour
{
    [SerializeField] public List<GenAdvancedInfo> genAdvancedInfos;
    [SerializeField] private TMP_Text time_txt;
    [SerializeField] ProgressBarHandler progressBarHandler;
    [SerializeField] TMP_Text amountToGenerate_txt;
    [SerializeField] private StartGeneratingButton startGeneratingButton;
    [SerializeField] private Animator generatorAnim;
    private float generationAnimLength;
    [SerializeField] private bool usingAnimLengthEqualGenTime = false;
    [SerializeField] private Animator resourceAnim;
    [SerializeField] public bool locked = false; // The locked state is for unlocking the auto functionality
    private List<(CurrencyTypes types, AlphabeticNotation amount)> yields;
    private float timeRemaining;
    private Coroutine generateRoutine;
    public bool stopRequested = false;
    private bool generatorRunning;
    [SerializeField] private CookingHandler cookingHandler;
    [SerializeField]private bool useRecipesForEnergyAuto;

    private void Start()
    {
        if (usingAnimLengthEqualGenTime)
        {
            generationAnimLength = generatorAnim.runtimeAnimatorController.animationClips[0].length;
        }
        UpdateUI();
    }

    public void EditGenAdvancedInfos
    (
        List<GenerateInfo> generateList,
        List<GenerateInfo> payList
        )
    {
        GenAdvancedInfo editableInfo = new GenAdvancedInfo();

        editableInfo.generateInfo = generateList;
        editableInfo.payInfo = payList;


        genAdvancedInfos.Add(editableInfo);
        // foreach (GenAdvancedInfo info in genAdvancedInfos)
        // {
        //     foreach (GenerateInfo genInfo in info.generateInfo)
        //     {
        //         print($"geninfo type = {genInfo.type} amount = {genInfo.amount}");
        //     }
        //     foreach (GenerateInfo genInfo in info.payInfo)
        //     {
        //         print($"geninfo pay type = {genInfo.type} amount = {genInfo.amount}");
        //     }
        // }
    }

    public void ClearGenAdvancedInfos()
    {
        genAdvancedInfos.Clear();
    }

    public bool CanAfford()
    {
        foreach (GenAdvancedInfo info in genAdvancedInfos)
        {
            foreach (GenerateInfo payinfo in info.payInfo)
            {

                if (MoneyManager.Instance.GetCurrency(payinfo.type) < payinfo.amount)
                {
                    return false;
                }
            }
        }
        return true;
    }
    private void Pay(CurrencyTypes _typeToPay, AlphabeticNotation _price)
    {
        MoneyManager.Instance.SubtractCurrency(_typeToPay, _price);
    }

    public void OnToggleGenerateAutoClicked(float time){
        if(generatorRunning){
            StopGenerating();
            generatorRunning = false;
        }else{

        if (generateRoutine == null)
        {
            generatorRunning = true;
            StartGeneratingAuto(time);
        }
        else
        {
            StopGenerating();
        }
        }
    }
    public void OnGenerateAutoClicked(float time)
    {
        if (generateRoutine == null)
        {
            StartGeneratingAuto(time);
        }
        else
        {
            StopGenerating();
        }
    }

    public void StartGenerating(float time)
    {
        if (CanAfford() && generateRoutine == null)
        {
            timeRemaining = time;
            yields = new List<(CurrencyTypes types, AlphabeticNotation amount)>();
            foreach (GenAdvancedInfo info in genAdvancedInfos)
            {
                foreach (GenerateInfo genInfo in info.generateInfo)
                {
                    yields.Add((genInfo.type, genInfo.amount));
                }
            }

            if (usingAnimLengthEqualGenTime && generatorAnim)
            {
                float speed = generationAnimLength / timeRemaining;
                generatorAnim.SetBool("Activated", true);
            }

            generateRoutine = StartCoroutine(Generating());
        }
    }

    public void StopGenerating()
    {
        if (generateRoutine != null)
        {

            StopCoroutine(generateRoutine);
            generateRoutine = null;
            progressBarHandler.ResetProgress();
            if (generatorAnim)
            {
                generatorAnim.SetBool("Activated", false); //hardcoded. Generator auto anim has to be called "Activated"
                generatorAnim.speed = 1f; // reset!
            }
        }
        UpdateUI();
    }

    private IEnumerator Generating()
    {
        foreach (GenAdvancedInfo info in genAdvancedInfos)
        {
            foreach (GenerateInfo payinfo in info.payInfo)
            {

                Pay(payinfo.type, payinfo.amount);
            }
        }
        progressBarHandler.StartProgress(timeRemaining);

        while (timeRemaining > 0f)
        {
            timeRemaining -= Time.deltaTime;
            UpdateUI();
            yield return null;
        }

        foreach (var yield in yields)
        {
            MoneyManager.Instance.AddCurrency(yield.types, yield.amount);
        }

        // foreach (GenAdvancedInfo info in genAdvancedInfos)
        // {
        //     foreach (GenerateInfo genInfo in info.generateInfo)
        // }

        StopGenerating();//
        UpdateUI();
    }


    private void UpdateUI()
    {
        if (timeRemaining <= 0f || generateRoutine == null)
        {
            time_txt.text = "00:00";
        }else{
        time_txt.text = HelperFunctions.Instance.ConvertSecondsToTime(Mathf.Floor(timeRemaining)).ToString();
        }

        // amountToGenerate_txt.text = UpgradeManager.Instance.GetAlphabetic(UpgradeIDGlobal.productionPower, typeToGenerate).ToString();
    }

    public void UpdateTime(float time)
    {
        time_txt.text = HelperFunctions.Instance.ConvertSecondsToTime(Mathf.Floor(time)).ToString();
    }


    public void StartGeneratingAuto(float time)
    {
        if (CanAfford() && generateRoutine == null)//removed tractor
        {
            timeRemaining = time;
            yields = new List<(CurrencyTypes types, AlphabeticNotation amount)>();
            foreach (GenAdvancedInfo info in genAdvancedInfos)
            {
                foreach (GenerateInfo genInfo in info.generateInfo)
                {
                    yields.Add((genInfo.type, genInfo.amount));
                }
            }
            if (usingAnimLengthEqualGenTime && generatorAnim)
            {
                float speed = generationAnimLength / timeRemaining;
                generatorAnim.SetBool("Activated", true);
            }
            else if (generatorAnim)
            {
                generatorAnim.SetBool("Activated", true);
            }
            timeRemaining = time;
            generateRoutine = StartCoroutine(GeneratingAuto());
            UpdateUI();
        }
    }
    private IEnumerator GeneratingAuto()
    {
        while (true)
        {
            if (stopRequested || !CanAfford())
            {
                StopGenerating();
                progressBarHandler.ResetProgress();
                startGeneratingButton.ShowAutoButton(); // makes the player have to re enable auto once the generator runs out of currency
                yield break;
            }
            foreach (GenAdvancedInfo info in genAdvancedInfos)
            {
                foreach (GenerateInfo payinfo in info.payInfo)
                {
                    Pay(payinfo.type, payinfo.amount);

                }
            }
            progressBarHandler.StartProgress(timeRemaining);

            // float currentTime = timeRemaining;
            float startTime = timeRemaining;

            while (timeRemaining > 0f)
            {
                timeRemaining -= Time.deltaTime;
                UpdateUI();
                yield return null;
            }
            foreach (var yield in yields)
            {
                MoneyManager.Instance.AddCurrency(yield.types, yield.amount);
            }

            progressBarHandler.ResetProgress();
            UpdateUI();
            timeRemaining = startTime;

        }
    }



}
