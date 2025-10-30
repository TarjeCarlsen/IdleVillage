using System;
using System.Collections;
using UnityEngine;

public class DoughPressHandler : MonoBehaviour
{

    private Coroutine doughPressRoutine;
    [SerializeField] private BakeryManager bakeryManager;
    

    private void Start(){
        StartGenerating();
    }
    private void StartGenerating(){
        if(doughPressRoutine == null){
            doughPressRoutine = StartCoroutine(StartPressingDough());
        }
    }   
    private void StopGenerating(){
        if(doughPressRoutine != null){
            StopCoroutine(doughPressRoutine);
            doughPressRoutine = null;
        }
    }
    private void OnEnable(){
        bakeryManager.OnFlourDropped += StartGenerating;
    }
    private void OnDisable(){
        bakeryManager.OnFlourDropped -= StartGenerating;
    }

    private  IEnumerator StartPressingDough(){
        while(true){
        yield return new WaitForSeconds(UpgradeManager.Instance.GetTimePower(TimeUpgradeTypes.doughPressCycleTime));
             bakeryManager.AddFlourToDough();
             if(bakeryManager.flourCounter <= 0){
                StopGenerating();
             }
         }
    }
}
