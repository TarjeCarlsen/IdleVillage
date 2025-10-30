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

    private  IEnumerator StartPressingDough(){
        while(true){
        yield return new WaitForSeconds(1f);
             bakeryManager.AddFlourToDough();
             print("pressing");
         }
    }
}
