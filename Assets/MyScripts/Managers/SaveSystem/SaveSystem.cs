using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveSystem
{
    private static SaveData _saveData = new SaveData();
    [System.Serializable]
    public struct SaveData
    {
        public CurrencySaveData currencySaveData;
        // public upgradeManagerSaveData upgradeManagerSaveData;
        public HouseManagerSaveData houseManagerSaveData;
        public StorageManagerSaveData storageManagerSaveData;
        public ShopManagerSaveData shopManagerSaveData;
        public InventoryHandlerSaveData[] inventoryHandlerSaveData;
    }

    public static string SaveFileName()
    {
        string saveFile = Application.persistentDataPath + "/idleVillageSave" + ".save";
        return saveFile;
    }

    public static void Save()
    {
        HandleSaveData();

        File.WriteAllText(SaveFileName(), JsonUtility.ToJson(_saveData, true));
    }
    public static void Load()
    {
        string saveContent = File.ReadAllText(SaveFileName());
        _saveData = JsonUtility.FromJson<SaveData>(saveContent);

        HandleLoadData();
    }


/**
// ------------ Template for saving array ------------------//
        OriginalScript[] newScriptArray = GameObject.FindObjectsByType<OriginalScript>(FindObjectsSortMode.None);
        _saveData.ScriptSaveData = new definedSaveArray[newScriptArray.Length];
        for (int i = 0; i < newScriptArray.Length; i++)
        {
            newScriptArray[i].Save(ref _saveData.ScriptSaveData[i]);
        }
// ------------ Template for loading array ------------------//

        OriginalScript[] newScriptArray = GameObject.FindObjectsByType<OriginalScript>(FindObjectsSortMode.None);
        for (int i = 0; i < newScriptArray.Length; i++)
        {
            newScriptArray[i].Load(_saveData.ScriptSaveData[i]);
        }
**/

    //--------------SETUP FOR SAVING SCRIPT-------------------------
    // First comment for name of the script thats beign saved
    // Second comment for function of the script
    public static void HandleSaveData()
    {
        GameManager.Instance.moneyManager.Save(ref _saveData.currencySaveData);
        // GameManager.Instance.upgradeManager.Save(ref _saveData.upgradeManagerSaveData);
        GameManager.Instance.houseManager.Save(ref _saveData.houseManagerSaveData);
        GameManager.Instance.storageManager.Save(ref _saveData.storageManagerSaveData);
        GameManager.Instance.shopManager.Save(ref _saveData.shopManagerSaveData);

        InventoryHandler[]inventoryHandlers = GameObject.FindObjectsByType<InventoryHandler>(FindObjectsSortMode.None);
        _saveData.inventoryHandlerSaveData = new InventoryHandlerSaveData[inventoryHandlers.Length];
        for (int i = 0; i < inventoryHandlers.Length; i++)
        {
            inventoryHandlers[i].Save(ref _saveData.inventoryHandlerSaveData[i]);
        }
    }
    

    public static void HandleLoadData()
    {
        GameManager.Instance.moneyManager.Load(_saveData.currencySaveData);
        // GameManager.Instance.upgradeManager.Load(_saveData.upgradeManagerSaveData);
        GameManager.Instance.houseManager.Load(_saveData.houseManagerSaveData);
        GameManager.Instance.storageManager.Load(_saveData.storageManagerSaveData);
        GameManager.Instance.shopManager.Load(_saveData.shopManagerSaveData);

        InventoryHandler[] inventoryHandlers = GameObject.FindObjectsByType<InventoryHandler>(FindObjectsSortMode.None);
        for (int i = 0; i < inventoryHandlers.Length; i++)
        {
            inventoryHandlers[i].Load(_saveData.inventoryHandlerSaveData[i]);
        }



    }

}
