using UnityEngine;

[CreateAssetMenu(fileName = "HouseData", menuName = "House Creation/HouseData")]
public class HouseData : ScriptableObject
{
    
    public GameObject prefabToSpawn;
    public HouesTypes houesType;
}
