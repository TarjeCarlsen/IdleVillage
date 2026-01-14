using System;
using System.Collections;
using System.Runtime.CompilerServices;
using LargeNumbers;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GeneratorResources : MonoBehaviour
{
    [SerializeField] private CurrencyTypes typeToGenerate;
    [SerializeField] private CurrencyTypes typeToPay;
    [SerializeField] private AlphabeticNotation amountToPay;
    [SerializeField] private ResourceGenerator resourceGenerator;
    [SerializeField] ProgressBarHandler progressBarHandler;
    [SerializeField] TMP_Text amountToGenerate_txt;
    [SerializeField] private StartGeneratingButton startGeneratingButton;
    [SerializeField] private Animator generatorAnim;
    [SerializeField] private Animator resourceAnim;
    [SerializeField] public bool locked = false; // The locked state is for unlocking the auto functionality
    [SerializeField] private GameObject tractor;
    private float timeRemaining;
    private Coroutine generateRoutine;
    public bool stopRequested = false;
        [SerializeField] private FarmManager farmManager;
    public event Action<CurrencyTypes> OnAutoGenerationStarted;
    public event Action<CurrencyTypes> OnAutoGenerationStopped;




    private void Start()
    {
                farmManager = GameObject.FindGameObjectWithTag("ShopPage").GetComponent<FarmManager>();
                tractor.SetActive(UpgradeManager.Instance.GetBool(UpgradeIDGlobal.tractorActivation,CurrencyDummy.Dummy));
                print("tractor state = "+ UpgradeManager.Instance.GetBool(UpgradeIDGlobal.tractorActivation,CurrencyDummy.Dummy));
        UpdateUI();
    }

    public bool CanAfford( )
    {
        if (MoneyManager.Instance.GetCurrency(typeToPay) >= amountToPay)
        {
            return true;
        }
        return false;
    }

    private void Pay(AlphabeticNotation price)
    {
        MoneyManager.Instance.SubtractCurrency(typeToPay, price);
    }

    public void OnGenerateAutoClicked(float time){
        if(generateRoutine == null){
            StartGeneratingAuto(time);
        }else{
            StopGenerating();
        }
    }

    public void StartGenerating(float time)
    {
        if (CanAfford() && generateRoutine == null)
        {
            OnAutoGenerationStarted?.Invoke(typeToGenerate);
            timeRemaining = farmManager.productionTimes[typeToGenerate];
            generateRoutine = StartCoroutine(Generating());
        }
    }

    public void StopGenerating()
    {
        if (generateRoutine != null)
        {
            
            OnAutoGenerationStopped?.Invoke(typeToGenerate);
            StopCoroutine(generateRoutine);
            generateRoutine = null;
            progressBarHandler.ResetProgress();
        if(generatorAnim){
            generatorAnim.SetBool("Activated", false); //hardcoded. Generator auto anim has to be called "Activated"
        }
        }
    }

private IEnumerator Generating()
{
    Pay(amountToPay);

    float elapsed = 0f;

    while (elapsed < GetCurrentDuration())
    {
        elapsed += Time.deltaTime;

        float duration = GetCurrentDuration();
        progressBarHandler.SetProgress(elapsed / duration);

        yield return null;
    }

    MoneyManager.Instance.AddCurrency(
        typeToGenerate,
        UpgradeManager.Instance.GetAlphabetic(
            UpgradeIDGlobal.productionPower,
            typeToGenerate
        )
    );

    StopGenerating();
    UpdateUI();
}


    private void UpdateUI()
    {
        amountToGenerate_txt.text = UpgradeManager.Instance.GetAlphabetic(UpgradeIDGlobal.productionPower,typeToGenerate).ToString();
    }


    public void StartGeneratingAuto(float time)
    {
        if (CanAfford() && generateRoutine == null)//removed tractor
        {
        if(generatorAnim){
            generatorAnim.SetBool("Activated", true); //hardcoded. Generator auto anim has to be called "Activated"
        }
            OnAutoGenerationStarted?.Invoke(typeToGenerate);
            timeRemaining = farmManager.productionTimes[typeToGenerate];
            generateRoutine = StartCoroutine(GeneratingAuto());
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
            startGeneratingButton.ShowAutoButton();
            yield break;
        }

        Pay(amountToPay);

        float elapsed = 0f;

        while (elapsed < GetCurrentDuration())
        {
            elapsed += Time.deltaTime;

            float duration = GetCurrentDuration();
            progressBarHandler.SetProgress(elapsed / duration);

            yield return null;
        }

        MoneyManager.Instance.AddCurrency(
            typeToGenerate,
            UpgradeManager.Instance.GetAlphabetic(
                UpgradeIDGlobal.productionPower,
                typeToGenerate
            )
        );

        progressBarHandler.ResetProgress();
        UpdateUI();
    }
}

private float GetCurrentDuration()
{
    return farmManager.productionTimes[typeToGenerate];
}
}
