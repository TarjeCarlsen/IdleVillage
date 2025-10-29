using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public MoneyManager moneyManager {get; set;}
    public HouseManager houseManager {get; set;}
    public StorageManager storageManager {get; set;}


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
        houseManager = FindFirstObjectByType<HouseManager>();
        storageManager = FindFirstObjectByType<StorageManager>();
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
