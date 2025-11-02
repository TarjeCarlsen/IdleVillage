using UnityEngine;

public class BowlCreateDough : MonoBehaviour
{
    
    [SerializeField] private BakeryManager bakeryManager;
    public void OnBowlClicked(){
        bakeryManager.AddFlourToDough();
    }
}
