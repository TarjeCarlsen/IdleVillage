using UnityEngine;

public class DoughHandler : MonoBehaviour
{
    [SerializeField] private CollectObject collectObject;
    private BakeryManager bakeryManager;

    private void Start(){
        bakeryManager = GameObject.FindGameObjectWithTag("BakeryManager").GetComponent<BakeryManager>();
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
