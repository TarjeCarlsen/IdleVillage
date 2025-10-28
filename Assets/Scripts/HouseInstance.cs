using System;
using UnityEngine;

public class HouseInstance : MonoBehaviour
{
    [SerializeField]private HouseData houseData;
    public string uniqueId;
    public HouesTypes GetHouseType() => houseData.houesType;

    private void Awake(){
        uniqueId = houseData.houesType + "_" + Guid.NewGuid().ToString();
    }
}
