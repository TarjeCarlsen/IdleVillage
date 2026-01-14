using TMPro;
using UnityEngine;

public class StartGeneratingButton : MonoBehaviour
{
    [SerializeField]GeneratorSimple generatorSimple;
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject cancelButton;
    

    public void OnStopClick(){
        generatorSimple.stopRequested = true;
        startButton.SetActive(true);
        cancelButton.SetActive(false);
    }

    public void ShowAutoButton(){
        startButton.SetActive(true);
        cancelButton.SetActive(false);
    }
    


}
