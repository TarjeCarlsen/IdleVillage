using TMPro;
using UnityEngine;

public class StartGeneratingButton : MonoBehaviour
{
    [SerializeField]GeneratorSimple generatorSimple;
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject cancelButton;
    
    public void OnGrainGenerationClick(){
        generatorSimple.StartGenerating(UpgradeManager.Instance.GetTimePower(TimeUpgradeTypes.windmillGrainTime));
    }
    public void OnFlourGenerationClick(){
        generatorSimple.StartGenerating(UpgradeManager.Instance.GetTimePower(TimeUpgradeTypes.windmillFlourTime));
    }

    public void OnGrainAutomaticClick(){
        if(!generatorSimple.CanAfford()) return;
        generatorSimple.stopRequested = false;
        generatorSimple.StartGeneratingAuto(UpgradeManager.Instance.GetTimePower(TimeUpgradeTypes.windmillGrainTime));
        startButton.SetActive(false);
        cancelButton.SetActive(true);
        
    }
    public void OnFlourAutomaticClick(){
        if(!generatorSimple.CanAfford()) return;
        generatorSimple.stopRequested = false;
        generatorSimple.StartGeneratingAuto(UpgradeManager.Instance.GetTimePower(TimeUpgradeTypes.windmillFlourTime));
        cancelButton.SetActive(true);
        startButton.SetActive(false);
    }
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
