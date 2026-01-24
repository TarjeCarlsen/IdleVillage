using LargeNumbers;
using UnityEngine;
using System;
using System.Collections;
public class EnergyConsumptionHandler : MonoBehaviour
{
    [SerializeField] private CurrencyTypes currencyTypes = CurrencyTypes.energy;
    [SerializeField] private AlphabeticNotation price;
    [SerializeField] private ProgressBarHandler progressBarHandler;
    public bool energyAutoRunning = false;
    public bool GetEnergyState() => energyAutoRunning;
    private float timeRemaining;
    private Coroutine energyCoroutine;


    private bool CanAfford(){
        if(MoneyManager.Instance.GetCurrency(currencyTypes) > price){
            return true;
        }
        else{
            return false;
        }
    }
    public void OnStartEnergyRoutine(float time){
            if(energyCoroutine == null &&  CanAfford()){
                MoneyManager.Instance.SubtractCurrency(currencyTypes,price);
                timeRemaining = time;
                energyCoroutine = StartCoroutine(AutoEnabled());
            }
    }

    public void OnStopEnergyRoutine(){
        if(energyCoroutine != null){
            StopCoroutine(energyCoroutine);
            energyCoroutine = null;
            energyAutoRunning = false;
        }
    }
private IEnumerator AutoEnabled(){

        while (true)
        {
            while (!CanAfford()){
                energyAutoRunning = false;
                yield return null;
            }
            energyAutoRunning = true;
            progressBarHandler.StartProgress(timeRemaining);
            float startTime = timeRemaining;

            while (timeRemaining > 0f)
            {
                timeRemaining -= Time.deltaTime;
                yield return null;
            }
            progressBarHandler.ResetProgress();
            timeRemaining = startTime;

        }
}




}
