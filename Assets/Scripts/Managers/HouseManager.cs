using System.Collections.Generic;
using UnityEngine;



public enum HouesTypes{
    wheatplant,
    silo,
    bakery,
    windmill,
    tractor,
}
public class HouseManager : MonoBehaviour
{
    public static HouseManager Instance {get; private set;}
    public List<HousePrefabs> housePrefabs;
    private List<HouseInstance> spawnedHouses = new();
    private Transform parentObject; 
    // private bool houseCreated;

    [System.Serializable]
    public class HousePrefabs{
        public HouesTypes houesType;
        public GameObject housePrefab;
        public HouseData houseData;
    }
    private void Awake(){
        if(Instance != null && Instance != this){
            Destroy(gameObject);
            return;
        }
        Instance = this;  
    }
    public void SpawnHouse(HouesTypes type, Transform parent, Transform position){
        GameObject prefab = housePrefabs[(int)type].housePrefab;
        GameObject newHouse = Instantiate(prefab,position.position,Quaternion.identity, parent);
        HouseInstance instance = newHouse.GetComponent<HouseInstance>();
        instance.CreateNewId();
        parentObject = parent;
        // houseCreated = true;
        if(instance != null) spawnedHouses.Add(instance);
    }



    public void Save(ref HouseManagerSaveData data){
    data.uniqueIds = new List<string>();
    data.positions = new List<Vector3>();
    data.types = new List<HouesTypes>();
    // data.houseCreated = new List<bool>();

    if(parentObject != null){
        data.parentObjectName = parentObject.name;
    }else{
        data.parentObjectName = "";
    }
for (int i = 0; i < spawnedHouses.Count; i++) {
    var house = spawnedHouses[i];
    if (house == null) {
        continue;
    }
}
    foreach (var house in spawnedHouses)
    {
        data.uniqueIds.Add(house.uniqueId);
        data.positions.Add(house.transform.position);
        data.types.Add(house.GetHouseType());
        // data.houseCreated.Add(houseCreated);
    }
    }
public void Load(HouseManagerSaveData data) {
    // Try to find parent by name
    if (!string.IsNullOrEmpty(data.parentObjectName)) {
        GameObject foundParent = GameObject.Find(data.parentObjectName);
        if (foundParent != null) {
            parentObject = foundParent.transform;
        } else {
        }
    }

    foreach (var old in spawnedHouses) {
        if (old != null)
            Destroy(old.gameObject);
    }
    spawnedHouses.Clear();

    for (int i = 0; i < data.uniqueIds.Count; i++) {
        HouesTypes type = data.types[i];
        Vector3 pos = data.positions[i];

        HousePrefabs prefabData = housePrefabs[(int)type];
        GameObject prefab = housePrefabs[(int)type].housePrefab;
        GameObject newHouse = Instantiate(prefab, pos, Quaternion.identity, parentObject);

        HouseInstance instance = newHouse.GetComponent<HouseInstance>();
        if (instance != null) {
            instance.uniqueId = data.uniqueIds[i];
            instance.AssignData(prefabData.houseData); // NEW
            spawnedHouses.Add(instance);
            // instance.SetHouseCreated(data.houseCreated[i]);
        }
    }
}

}


[System.Serializable]
public struct HouseManagerSaveData{
    public List<string> uniqueIds;
    public List<Vector3> positions;
    public List<HouesTypes> types;
     public string parentObjectName;
    //  public List<bool> houseCreated;

}