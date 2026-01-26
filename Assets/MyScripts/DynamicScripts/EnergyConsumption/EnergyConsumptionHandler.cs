using LargeNumbers;
using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Events;
using TMPro;
public class EnergyConsumptionHandler : MonoBehaviour
{
    [SerializeField] private CurrencyTypes currencyTypes = CurrencyTypes.energy;
    [SerializeField] private AlphabeticNotation price;
    [SerializeField] private ProgressBarHandler progressBarHandler;
    public bool energyAutoRunning = false;
    public bool GetEnergyState() => energyAutoRunning;
    public bool IsEnergyRoutineRunning => energyCoroutine != null;
    private float timeRemaining;
    private Coroutine energyCoroutine;
    private bool RestartEnergy = false;
    public event Action EnergyExausted;
    public event Action EnergyReStarted;
    [SerializeField]private TMP_Text start_stop_txt;




    private void Start(){
        UpdateUI();
    }
    public bool CanAfford(){
        if(MoneyManager.Instance.GetCurrency(currencyTypes) >= price){
            return true;
        }
        else{
            return false;
        }
    }
    public void OnStartEnergyRoutine(float time){
            if(energyCoroutine == null &&  CanAfford()){
                energyAutoRunning = true;
                timeRemaining = time;
                energyCoroutine = StartCoroutine(AutoEnabled());
                MoneyManager.Instance.SubtractCurrency(currencyTypes,price);
                UpdateUI();
            }
    }

    public void OnStopEnergyRoutine(){
        if(energyCoroutine != null){
            StopCoroutine(energyCoroutine);
            energyCoroutine = null;
            energyAutoRunning = false;
            RestartEnergy = false;
            UpdateUI();
        }
    }


private IEnumerator AutoEnabled(){

        while (true)
        {
            if(!CanAfford()){
                EnergyExausted?.Invoke();
                RestartEnergy = true;
                UpdateUI();
            }

            while (!CanAfford()){
                yield return null;
            }
            if(RestartEnergy){
                RestartEnergy = false;
                EnergyReStarted?.Invoke();
            }
            // energyAutoRunning = true;
            progressBarHandler.StartProgress(timeRemaining);
            float startTime = timeRemaining;
            UpdateUI();

            while (timeRemaining > 0f)
            {
                timeRemaining -= Time.deltaTime;
                yield return null;
            }
            if(MoneyManager.Instance.GetCurrency(currencyTypes) - price >= 0){
                MoneyManager.Instance.SubtractCurrency(currencyTypes,price);
            }
            progressBarHandler.ResetProgress();
            timeRemaining = startTime;
            UpdateUI();
        }
}

private void UpdateUI(){
    if(RestartEnergy){
    start_stop_txt.text = $"<size=70%><color=#00107E>Exhausted</color></size>";
    }else if(energyAutoRunning){
    start_stop_txt.text = $"<color=#7A1E1E>Stop</color>";
    }else if(!energyAutoRunning){
    start_stop_txt.text = $"<color=#1E7F1E>Start</color>";
    }
}


}
