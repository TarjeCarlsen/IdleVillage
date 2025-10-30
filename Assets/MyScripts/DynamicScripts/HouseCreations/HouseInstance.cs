using System;
using UnityEngine;

public class HouseInstance : MonoBehaviour
{
    [SerializeField]private HouseData houseData;
    public string uniqueId;
    // public 
    // public bool houseCreated;
    // public bool GetHouseCreated()=> houseCreated;
    public HouesTypes GetHouseType() => houseData.houesType;
    public Action OnNewIdCreated;
    public Action OnHouseCreated;

    // private void Awake(){
    // CreateNewId();
    // }
    public void AssignData(HouseData data) {
    houseData = data;
}

public void CreateNewId(){
    uniqueId = houseData.houesType + "_" + Guid.NewGuid().ToString();
    OnNewIdCreated?.Invoke();
}
    // public void SetHouseCreated(bool state){
    //     houseCreated = state;
    //     OnHouseCreated?.Invoke();
    // }
}
