using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using System;
using UnityEngine.UI;
public class InventoryHandler : MonoBehaviour
{

    private Dictionary<HouesTypes, int> houseCounter = new();
    [SerializeField] private List<TextAndImage> inv_text_img;
    [SerializeField] private Transform parentToSpawnUnder;
    private HouseManager houseManager;

    [System.Serializable]
    public class TextAndImage
    {
        public GameObject parentObject;
        public Draggable draggable;
        public Image inventoryImg;
        public TMP_Text inventoryTxt;
    }

    private void Awake()
    {
        houseManager =GameObject.FindGameObjectWithTag("GameManager").GetComponent<HouseManager>();;
        InitializeHousecounter();
    }

    public void OnEnable()
    {
        for (int i = 0; i < inv_text_img.Count; i++)
        {
            HouesTypes type = (HouesTypes)i;
            inv_text_img[i].draggable.OnPlaced += (draggable) => RemoveFromInv(type, draggable);

        }
    }

    public void OnDisable()
    {
        for (int i = 0; i < inv_text_img.Count; i++)
        {
            HouesTypes type = (HouesTypes)i;
            inv_text_img[i].draggable.OnPlaced -= (draggable) => RemoveFromInv(type, draggable);

        }
    }


    private void InitializeHousecounter()
    {
        foreach (HouesTypes type in Enum.GetValues(typeof(HouesTypes)))
        {
            houseCounter[type] = 0;
        }
    }
    public void AddToInv(HouesTypes type)
    {
        houseCounter[type]++;
        if (houseCounter[type] > 1)
        {
            inv_text_img[(int)type].inventoryTxt.text = houseCounter[type].ToString();
        }
        inv_text_img[(int)type].parentObject.SetActive(true);
    }

    public void RemoveFromInv(HouesTypes type, Draggable draggable)
    {
        houseCounter[type]--;
        inv_text_img[(int)type].inventoryTxt.text = houseCounter[type].ToString();
        if (houseCounter[type] < 1)
        {
            inv_text_img[(int)type].parentObject.SetActive(false);
        }
        SpawnOnPlaced(type, draggable.transform);
    }

    public void SpawnOnPlaced(HouesTypes type, Transform position){
        HouseManager.Instance.SpawnHouse(type, parentToSpawnUnder, position);
    }






public void Save(ref InventoryHandlerSaveData data){
    data.inventoryDatas = new List<InventoryData>();
    foreach(HouesTypes type in Enum.GetValues(typeof(HouesTypes))){
        int count = houseCounter[type];
        if(count > 0){
            data.inventoryDatas.Add(new InventoryData{
                houseType = type.ToString(),
                counter_txt = count,
            });
        }
    }

    }

public void Load(InventoryHandlerSaveData data){
    InitializeHousecounter();

    foreach(TextAndImage entry in inv_text_img){
        entry.parentObject.SetActive(false);
        entry.inventoryTxt.text = "";
    }
    foreach (InventoryData item in data.inventoryDatas)
    {
        if (Enum.TryParse<HouesTypes>(item.houseType, out HouesTypes type))
        {
            houseCounter[type] = item.counter_txt;
            var uiEntry = inv_text_img[(int)type];
            uiEntry.parentObject.SetActive(true);
            uiEntry.inventoryTxt.text = item.counter_txt > 1 ? item.counter_txt.ToString() : "";
        }
    }
}



}

[System.Serializable]
public struct InventoryHandlerSaveData{
    public List<InventoryData> inventoryDatas;
}


[System.Serializable]
public class InventoryData{
    public string houseType;
    public int counter_txt;
}