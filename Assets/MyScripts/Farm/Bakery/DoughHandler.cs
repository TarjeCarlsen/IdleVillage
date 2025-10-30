using UnityEngine;

public class DoughHandler : MonoBehaviour
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
        bakeryManager.AddDoughToFurnace();
    }
}
