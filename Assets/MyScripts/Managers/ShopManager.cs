using System.Collections.Generic;
using System.Xml;
using LargeNumbers;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance {get; private set;}

    [SerializeField] private TMP_Text currentListingAmount_txt;
    public Dictionary<string, ListingData> listingObjects = new Dictionary<string, ListingData>();
    [SerializeField] private Transform ShopCardHolder;
    public List<ShopCardHandler> shopCardList;
    
    private int maxAmountOfListings;
    private int currentListings;
    [System.Serializable]
    public class ListingData{
        public AlphabeticNotation result;
        public float currentTime;
        public double chance;
        public AlphabeticNotation cancelAmount;
        public CurrencyTypes cancelType;
        public string uniqueID;
        public ListingHandler listingHandler;
        public string shopCardName;
        public bool listingSold;
    }
void Awake()
{
    if (Instance != null && Instance != this)
    {
        Destroy(gameObject);
        return;
    }
    Instance = this;
    foreach(Transform child in ShopCardHolder){
        shopCardList.Add(child.GetComponent<ShopCardHandler>());
    }
}
    public void AddListing(string id, ListingData data){ // add all information about the listing object for save/load
        listingObjects[id] = data;
        // print($"listing info : result = {listingObjects[listing].result} time = {listingObjects[listing].currentTime} cancelam = {listingObjects[listing].cancelAmount} type = {listingObjects[listing].cancelType} id = {listingObjects[listing].uniqueID}");
        currentListings++;
        // print($"ShopManager instance: {GetInstanceID()} | listings count = {listingObjects.Count}");
        UpdateUI();
    }

    public void RemoveListing(string id){
        currentListings--;
        listingObjects.Remove(id);
        UpdateUI();
    }



    private void UpdateUI(){
        currentListingAmount_txt.text = currentListings.ToString() +"/" +  maxAmountOfListings.ToString();
    }



    public void Save(ref ShopManagerSaveData data){
        data.listingSaveDatas = new List<ListingSaveData>();

        foreach(KeyValuePair<string,ListingData> pair in listingObjects){
        pair.Value.currentTime = pair.Value.listingHandler.GetCurrentTime();
        pair.Value.listingSold = pair.Value.listingHandler.GetListingSold();

            data.listingSaveDatas.Add( new ListingSaveData {
                result = pair.Value.result,
                currentTime = pair.Value.currentTime,
                chance = pair.Value.chance,
                cancelAmount = pair.Value.cancelAmount,
                cancelType = pair.Value.cancelType,
                uniqueID = pair.Value.uniqueID,
                shopCardName = pair.Value.shopCardName,
                listingSold = pair.Value.listingSold,
            });
        }
    }
    public void Load(ShopManagerSaveData data){
        foreach(var pair in listingObjects){
            // pair.Value.listingHandler.CloseListing();
            Destroy(pair.Value.listingHandler.gameObject);
        }
        listingObjects.Clear();
        currentListings = 0;


        foreach(ListingSaveData listingData in data.listingSaveDatas){
            foreach(ShopCardHandler card in shopCardList){
                if(card.name == listingData.shopCardName){
                    card.CreateListingFromLoad(listingData.result,listingData.currentTime,
                                                listingData.chance, listingData.cancelAmount,
                                                listingData.cancelType, listingData.uniqueID,
                                                listingData.shopCardName, listingData.listingSold);
                                                break;
                }
            }
        }
    }
}

[System.Serializable]
public struct ShopManagerSaveData{
    public List<ListingSaveData> listingSaveDatas;
}
[System.Serializable]
public struct ListingSaveData{
       public AlphabeticNotation result;
        public float currentTime;
        public double chance;
        public AlphabeticNotation cancelAmount;
        public CurrencyTypes cancelType;
        public string uniqueID;
        public string shopCardName;
        public bool listingSold;
}