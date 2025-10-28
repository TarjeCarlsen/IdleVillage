using System.Collections.Generic;
using UnityEngine;



public enum HouesTypes{
    wheatplant,
    silo,
    windmill,
    bakery,
    tractor,
}
public class HouseManager : MonoBehaviour
{
    public static HouseManager Instance {get; private set;}
    public List<HousePrefabs> housePrefabs;
    [System.Serializable]
    public class HousePrefabs{
        public HouesTypes houesType;
        public GameObject housePrefab;
    }
    private void Awake(){

        Instance = this;
    }
    public void SpawnHouse(HouesTypes type, Transform parent, Transform position){
        GameObject prefab = housePrefabs[(int)type].housePrefab;
        GameObject newHouse = Instantiate(prefab,position.position,Quaternion.identity, parent);
    }
}
