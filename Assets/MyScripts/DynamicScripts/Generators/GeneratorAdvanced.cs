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
    [SerializeField] private Animator resourceAnim;
    [SerializeField] public bool locked = false; // The locked state is for unlocking the auto functionality
    private float timeRemaining;
    private Coroutine generateRoutine;
    public bool stopRequested = false;


    private void Start()
    {

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
        foreach (GenAdvancedInfo info in genAdvancedInfos)
        {
            foreach (GenerateInfo genInfo in info.generateInfo)
            {
                print($"geninfo type = {genInfo.type} amount = {genInfo.amount}");
            }
            foreach (GenerateInfo genInfo in info.payInfo)
            {
                print($"geninfo pay type = {genInfo.type} amount = {genInfo.amount}");
            }
        }
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
            }
        }
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

        foreach (GenAdvancedInfo info in genAdvancedInfos)
        {
            foreach (GenerateInfo genInfo in info.generateInfo)
                MoneyManager.Instance.AddCurrency(genInfo.type, UpgradeManager.Instance.GetAlphabetic(UpgradeIDGlobal.productionPower, genInfo.type));
        }

        StopGenerating();
        UpdateUI();
    }


    private void UpdateUI()
    {
        if(timeRemaining <= 0f){
        time_txt.text = "00:00:00";
        }
        time_txt.text = HelperFunctions.Instance.ConvertSecondsToTime(Mathf.Floor(timeRemaining)).ToString();

        // amountToGenerate_txt.text = UpgradeManager.Instance.GetAlphabetic(UpgradeIDGlobal.productionPower, typeToGenerate).ToString();
    }


    public void StartGeneratingAuto(float time)
    {
        if (CanAfford() && generateRoutine == null)//removed tractor
        {
            if (generatorAnim)
            {
                generatorAnim.SetBool("Activated", true); //hardcoded. Generator auto anim has to be called "Activated"
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

            float currentTime = timeRemaining;

            while (currentTime > 0f)
            {
                currentTime -= Time.deltaTime;
                UpdateUI();
                yield return null;
            }
            foreach (GenAdvancedInfo info in genAdvancedInfos)
            {
                foreach (GenerateInfo geninfo in info.generateInfo)
                {
                    MoneyManager.Instance.AddCurrency(geninfo.type, UpgradeManager.Instance.GetAlphabetic(UpgradeIDGlobal.productionPower, geninfo.type));
                }
            }

            progressBarHandler.ResetProgress();
            UpdateUI();
            currentTime = timeRemaining;

        }
    }



}
