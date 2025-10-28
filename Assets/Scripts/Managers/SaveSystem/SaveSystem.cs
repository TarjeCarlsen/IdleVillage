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
        public HouseManagerSaveData houseManagerSaveData;

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


    //--------------SETUP FOR SAVING SCRIPT-------------------------
    // First comment for name of the script thats beign saved
    // Second comment for function of the script

    public static void HandleSaveData()
    {
        GameManager.Instance.moneyManager.Save(ref _saveData.currencySaveData);
        GameManager.Instance.houseManager.Save(ref _saveData.houseManagerSaveData);
    }
    

    public static void HandleLoadData()
    {
        GameManager.Instance.moneyManager.Load(_saveData.currencySaveData);
        GameManager.Instance.houseManager.Load(_saveData.houseManagerSaveData);

    }

}
