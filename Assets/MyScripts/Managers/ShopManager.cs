using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
    [SerializeField] private TMP_Text currentCollectAmount_txt;
    [SerializeField] private GameObject collectAllButton;
    public Dictionary<string, ListingData> listingObjects = new Dictionary<string, ListingData>();
    [SerializeField] private Transform ShopCardHolder;
    public List<ShopCardHandler> shopCardList;
    
    private AlphabeticNotation collectAmount;
    public int currentListings;
    public int GetCurrentListingAmount()=> currentListings;
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
        public int amountOfCustomersInterested;
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
    UpdateUI();
    UpdateCollectAmount(new AlphabeticNotation(0),true);
}
    public void AddListing(string id, ListingData data){
        listingObjects[id] = data;
        currentListings++;
        UpdateUI();
    }

    public void RemoveListing(string id){
        currentListings--;
        listingObjects.Remove(id);
        UpdateUI();
    }

    public void UpdateListing(string id, bool state){
        if(listingObjects.ContainsKey(id)){
            listingObjects[id].listingSold = state;
        }
        

    }

    public void UpdateCollectAmount(AlphabeticNotation amount, bool addSub){
        collectAmount = collectAmount +(addSub ? amount : -amount);
        if(collectAmount > 0)
        {
            collectAllButton.SetActive(true);
        }else{
            collectAllButton.SetActive(false);
        }
        UpdateUI();
    }

    public void OnCollectAllClick(){
        List<string> keys = new List<string>(listingObjects.Keys);
        foreach(string key in keys){
            if(listingObjects[key].listingSold){
                listingObjects[key].listingHandler.OnCollectButtonClicked();
            }
        }
        
        currentCollectAmount_txt.text = new AlphabeticNotation(0).ToString();
    }

    private void UpdateUI(){
        currentCollectAmount_txt.text = collectAmount.ToStringSmart(1);
        currentListingAmount_txt.text = currentListings.ToString() +"/" +  StorageManager.Instance.GetMaxSpecialStorage(SpecialStorageType.shopAmountListings).ToString();
    }



    public void Save(ref ShopManagerSaveData data){
        data.listingSaveDatas = new List<ListingSaveData>();

        //data.collectAllButton = collectAllButton.activeSelf;
        //data.collectAmount = collectAmount;

        foreach(KeyValuePair<string,ListingData> pair in listingObjects){
        pair.Value.currentTime = pair.Value.listingHandler.GetCurrentTime();
        pair.Value.listingSold = pair.Value.listingHandler.GetListingSold();
        pair.Value.amountOfCustomersInterested = pair.Value.listingHandler.GetAmountCustomers();

            data.listingSaveDatas.Add( new ListingSaveData {
                result = pair.Value.result,
                currentTime = pair.Value.currentTime,
                chance = pair.Value.chance,
                cancelAmount = pair.Value.cancelAmount,
                cancelType = pair.Value.cancelType,
                uniqueID = pair.Value.uniqueID,
                shopCardName = pair.Value.shopCardName,
                listingSold = pair.Value.listingSold,
                amountOfCustomersInterested = pair.Value.amountOfCustomersInterested,
            });
        }
    }
    public void Load(ShopManagerSaveData data){
        //collectAllButton.SetActive(data.collectAllButton);
        // UpdateCollectAmount(data.collectAmount, true);

        foreach(var pair in listingObjects){
            pair.Value.listingHandler.StopActiveListing();
            Destroy(pair.Value.listingHandler.gameObject);
        }

        collectAmount = new AlphabeticNotation(0);
        collectAllButton.SetActive(false);
        // currentCollectAmount_txt.text = collectAmount.ToStringSmart(0);

        listingObjects.Clear();
        currentListings = 0;


        foreach(ListingSaveData listingData in data.listingSaveDatas){
            foreach(ShopCardHandler card in shopCardList){
                if(card.name == listingData.shopCardName){
                    card.CreateListingFromLoad(listingData.result,listingData.currentTime,
                                                listingData.chance, listingData.cancelAmount,
                                                listingData.cancelType, listingData.uniqueID,
                                                listingData.shopCardName, listingData.listingSold,
                                                listingData.amountOfCustomersInterested
                                                );
                                                break;
                }
            }
        }
        UpdateUI();
    }
}

[System.Serializable]
public struct ShopManagerSaveData{
    public bool collectAllButton;
    public AlphabeticNotation collectAmount;
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
        public int amountOfCustomersInterested;
}