using UnityEngine;

public class FlourHandler : MonoBehaviour
{
    [SerializeField] private CollectObject collectObject;
    private BakeryManager bakeryManager;

    private void Start(){
         bakeryManager = GetComponentInParent<BakeryManager>();
    }
    private void OnEnable(){
        collectObject.OnCollect += Collect; 
    }
    private void OnDisable(){
        collectObject.OnCollect -= Collect; 
    }

    private void Collect(){
        bakeryManager.AddFlourToBowl();
    }
}
