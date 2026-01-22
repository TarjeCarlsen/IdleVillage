using System;
using System.Collections;
using System.Runtime.CompilerServices;
using LargeNumbers;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GeneratorResources : MonoBehaviour
{
    [SerializeField] public CurrencyTypes typeToGenerate;
    [SerializeField] private CurrencyTypes typeToPay;
    [SerializeField] private AlphabeticNotation amountToPay;
    [SerializeField] ProgressBarHandler progressBarHandler;
    [SerializeField] TMP_Text amountToGenerate_txt;
    [SerializeField] private Animator generatorAnim;
    [SerializeField] private Animator resourceAnim;
    [SerializeField] public bool locked = false; // The locked state is for unlocking the auto functionality
    [SerializeField] private GameObject tractor;
    private bool isManualRunning = false;
    private bool isAutoRunning = false;
    private bool autoQueued = false;
    private float timeRemaining;
    private Coroutine generateRoutine;
    public bool stopRequested = false;
        [SerializeField] private UpgradeHandler upgradeHandler;
    public event Action<CurrencyTypes> OnAutoGenerationStopped;




    private void Awake()
    {
                upgradeHandler = GameObject.FindGameObjectWithTag("ShopPage").GetComponent<UpgradeHandler>();
                tractor.SetActive(UpgradeManager.Instance.GetBool(UpgradeIDGlobal.tractorActivation,CurrencyDummy.Dummy));
    }

    private void Start(){

        UpdateUI();
    }

    private void OnEnable(){
        upgradeHandler.notfiyUpdate += UpdateUI;
    }
    private void OnDisable(){
        upgradeHandler.notfiyUpdate -= UpdateUI;
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

public void OnGenerateAutoClicked(float time)
{
    // If auto is already running → stop it
    if (isAutoRunning)
    {
        stopRequested = true;
        autoQueued = false;
            if (generatorAnim)
        generatorAnim.SetBool("Activated", false);
        return;
    }

    // If manual is running → queue auto
    if (isManualRunning)
    {
        autoQueued = true;
        if (generatorAnim)
        generatorAnim.SetBool("Activated", true);
        return;
    }

    // Otherwise → start auto immediately
    StartGeneratingAuto(time);
}

public void StartGenerating(float time)
{
    if (CanAfford() && generateRoutine == null)
    {
        isManualRunning = true;
        timeRemaining = upgradeHandler.productionTimes[typeToGenerate];
        generateRoutine = StartCoroutine(Generating());
    }
}

private void StopGenerating()
{
    if (generateRoutine != null)
    {
        StopCoroutine(generateRoutine);
        generateRoutine = null;
    }

    isAutoRunning = false;
    isManualRunning = false;
    autoQueued = false;

    progressBarHandler.ResetProgress();

    if (generatorAnim)
        generatorAnim.SetBool("Activated", false);

    OnAutoGenerationStopped?.Invoke(typeToGenerate);
}

private IEnumerator Generating()
{
    Pay(amountToPay);

    float elapsed = 0f;

    while (elapsed < GetCurrentDuration())
    {
        elapsed += Time.deltaTime;
        progressBarHandler.SetProgress(elapsed / GetCurrentDuration());
        yield return null;
    }

    MoneyManager.Instance.AddCurrency(
        typeToGenerate,
        upgradeHandler.CalculateProductionFarm(typeToGenerate)
    );

    isManualRunning = false;
    generateRoutine = null;
    progressBarHandler.ResetProgress();
    UpdateUI();

    // ✅ START AUTO IF QUEUED
    if (autoQueued)
    {
        autoQueued = false;
        StartGeneratingAuto(0f);
    }
}

    private void UpdateUI()
    {
        amountToGenerate_txt.text = UpgradeManager.Instance.GetAlphabetic(UpgradeIDGlobal.productionPower,typeToGenerate).ToString();
    }


public void StartGeneratingAuto(float time)
{
    if (!CanAfford() || generateRoutine != null) return;

    isAutoRunning = true;
    stopRequested = false;

    if (generatorAnim)
        generatorAnim.SetBool("Activated", true);

    generateRoutine = StartCoroutine(GeneratingAuto());
}
private IEnumerator GeneratingAuto()
{
    while (true)
    {
        if (stopRequested || !CanAfford())
        {
            StopGenerating();
            progressBarHandler.ResetProgress();
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
            upgradeHandler.CalculateProductionFarm(typeToGenerate)
        );

        progressBarHandler.ResetProgress();
        UpdateUI();
    }
}

private float GetCurrentDuration()
{
    return upgradeHandler.productionTimes[typeToGenerate];
}

public bool isGeneratorRunning(){
    return isManualRunning;
}
}
