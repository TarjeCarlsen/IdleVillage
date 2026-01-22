using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public MoneyManager moneyManager {get; set;}
    public StorageManager storageManager {get; set;}
    public UpgradeManager upgradeManager{get; set;}
    public ShopManager shopManager {get; set;} 
    public KitchenManager kitchenManager {get; set;}

    // public HouseManager houseManager {get; set;} // old
    private void Awake(){
        if(Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }else{
            Destroy(gameObject);
        }
        Application.runInBackground = true;
    }

    private void Start(){
        moneyManager = FindFirstObjectByType<MoneyManager>();
        upgradeManager = FindFirstObjectByType<UpgradeManager>();
        storageManager = FindFirstObjectByType<StorageManager>();
        shopManager = FindFirstObjectByType<ShopManager>();
        kitchenManager = FindFirstObjectByType<KitchenManager>();
        
        // houseManager = FindFirstObjectByType<HouseManager>(); old
    }


    public void SaveGameBTN(){
        SaveSystem.Save();
    }
    public void LoadGameBTN(){
        SaveSystem.Load();
    }

    private void Update(){
        if(Input.GetKeyDown(KeyCode.S)){
            SaveSystem.Save();
            print("Saving!");
        }
        if(Input.GetKeyDown(KeyCode.L)){
            SaveSystem.Load();
            print("Loading!");

        }
    }

}
