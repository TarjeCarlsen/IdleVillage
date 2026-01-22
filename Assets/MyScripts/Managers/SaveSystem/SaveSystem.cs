using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
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
        public KitchenManagerSaveData kitchenManagerSaveData;
        public CookingHandlerSaveData[] cookingHandlerSaveData;
        public NewRecipeHandlerSaveData[] newRecipeHandlerSaveDatas;

        // public InventoryHandlerSaveData[] inventoryHandlerSaveData; old
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

    /**


// ------------ Template for saving using helperfunction ------------- //

        _saveData.ScriptSaveData =SaveAll<OriginalScript, definedSaveArray>(
        (OriginalScript h, ref definedSaveArray d) => h.Save(ref d));

// ------------ Template for loading unique id objects --------------- //

        LoadById<OriginalScript, definedSaveArray>(_saveData.ScriptSaveData,
            handler => handler.uniqueId,
            data => data.uniqueId,
            (handler, data) => handler.Load(data)
        );

**/
    //--------------SETUP FOR SAVING SCRIPT-------------------------
    // First comment for name of the script thats beign saved
    // Second comment for function of the script



    public static void HandleSaveData()
    {

        GameManager.Instance.moneyManager.Save(ref _saveData.currencySaveData);
        // GameManager.Instance.upgradeManager.Save(ref _saveData.upgradeManagerSaveData);
        GameManager.Instance.storageManager.Save(ref _saveData.storageManagerSaveData);
        GameManager.Instance.shopManager.Save(ref _saveData.shopManagerSaveData);
        GameManager.Instance.kitchenManager.Save(ref _saveData.kitchenManagerSaveData);


        _saveData.cookingHandlerSaveData =SaveAll<CookingHandler, CookingHandlerSaveData>(
        (CookingHandler h, ref CookingHandlerSaveData d) => h.Save(ref d));

        _saveData.newRecipeHandlerSaveDatas = SaveAll<newRecipeHandler, NewRecipeHandlerSaveData>(
            (newRecipeHandler h, ref NewRecipeHandlerSaveData d) =>        h.Save(ref d)
            );



    }

    public static void HandleLoadData()
    {
        GameManager.Instance.moneyManager.Load(_saveData.currencySaveData);
        // GameManager.Instance.upgradeManager.Load(_saveData.upgradeManagerSaveData);
        GameManager.Instance.storageManager.Load(_saveData.storageManagerSaveData);
        GameManager.Instance.shopManager.Load(_saveData.shopManagerSaveData);
        GameManager.Instance.kitchenManager.Load(_saveData.kitchenManagerSaveData);

        LoadById<CookingHandler, CookingHandlerSaveData>(_saveData.cookingHandlerSaveData,
            handler => handler.uniqueId,
            data => data.uniqueId,
            (handler, data) => handler.Load(data)
        );

        LoadById<newRecipeHandler, NewRecipeHandlerSaveData>(_saveData.newRecipeHandlerSaveDatas,
            handler => handler.uniqueId,
            data => data.uniqueId,
            (handler, data) => handler.Load(data)
        );

    }







    // ------ helper functions ------
    public delegate void SaveDelegate<THandler, TSaveData>(
    THandler handler,
    ref TSaveData data
);

public static TSaveData[] SaveAll<THandler, TSaveData>(
    SaveDelegate<THandler, TSaveData> saveAction
)
    where THandler : MonoBehaviour
    where TSaveData : struct
{
    THandler[] handlers =
        GameObject.FindObjectsByType<THandler>(FindObjectsSortMode.None);

    TSaveData[] saveDataArray = new TSaveData[handlers.Length];

    for (int i = 0; i < handlers.Length; i++)
    {
        saveAction(handlers[i], ref saveDataArray[i]);
    }

    return saveDataArray;
}


        public static void LoadById<THandler, TSaveData>(
        TSaveData[] saveDataArray,
        Func<THandler, string> getHandlerId,
        Func<TSaveData, string> getSaveDataId,
        Action<THandler, TSaveData> loadAction
    )
        where THandler : MonoBehaviour
    {
        if (saveDataArray == null || saveDataArray.Length == 0)
            return;

        THandler[] handlers =
            GameObject.FindObjectsByType<THandler>(FindObjectsSortMode.None);

        Dictionary<string, TSaveData> lookup = new();

        foreach (var data in saveDataArray)
        {
            lookup[getSaveDataId(data)] = data;
        }

        foreach (var handler in handlers)
        {
            string id = getHandlerId(handler);

            if (lookup.TryGetValue(id, out var data))
            {
                loadAction(handler, data);
            }
        }
    }


public static void LoadAll<THandler, TSaveData>(
    TSaveData[] saveDataArray,
    Action<THandler, TSaveData> loadAction
)
    where THandler : MonoBehaviour
{
    if (saveDataArray == null)
        return;

    THandler[] handlers =
        GameObject.FindObjectsByType<THandler>(FindObjectsSortMode.None);

    int count = Mathf.Min(handlers.Length, saveDataArray.Length);

    for (int i = 0; i < count; i++)
    {
        loadAction(handlers[i], saveDataArray[i]);
    }
}

}
