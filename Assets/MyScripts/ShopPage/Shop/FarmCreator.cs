using UnityEngine;

public class FarmCreator : MonoBehaviour
{
    [SerializeField] private Transform parentToSpawnUnder;

    public void CreateFarm(GameObject farm ){
        Instantiate(farm,parentToSpawnUnder);
    }
}
