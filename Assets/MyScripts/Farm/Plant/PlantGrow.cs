using UnityEngine;
using LargeNumbers;
public class PlantGrow : MonoBehaviour
{

    [SerializeField] Animator plant_animator;
    [SerializeField] GameObject displayImage;
    [SerializeField] private float desiredGrowDuration = 5f; // SWAP THIS WITH UPGRADABLE TIME WHEN IMPLEMENTING UPRADEMANAGER
    // [SerializeField] Draggable draggable;
    public bool plantGrown = false;


// private void OnEnable(){
//     draggable.OnPlaced += StartGrowing;
// }
// private void OnDisable(){
//     draggable.OnPlaced -= StartGrowing;

// }

private void Awake(){
    StartGrowing();
}
private void StartGrowing(){
    displayImage.SetActive(false);
    plantGrown = false;
    plant_animator.SetBool("plantGrown", false);

    AnimatorClipInfo[] clips = plant_animator.GetCurrentAnimatorClipInfo(0); //Sets the desired time based on amount of clips
    if(clips.Length > 0){
        AnimationClip growClip = clips[0].clip;
        float originalLength = growClip.length;
        float speed = originalLength / desiredGrowDuration;
        plant_animator.speed = speed;
    }
}

    public void FinishedGrowingFromAnim(){
    plant_animator.SetBool("plantGrown", true);
    plantGrown = true;
    }

    public void OnPlantClicked(){
        if(!plantGrown) return;
        StartGrowing();
        AlphabeticNotation bonus = UpgradeManager.Instance.GetProductionPower(CurrencyTypes.wheat);
        MoneyManager.Instance.AddCurrency(CurrencyTypes.wheat, bonus);
    }
}
