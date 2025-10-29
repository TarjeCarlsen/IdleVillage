using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StorageHandler : MonoBehaviour
{
    [SerializeField] List<StorageData> storageDatas;
    private bool hasBeenPlaced = false;

    [System.Serializable]
    public class StorageData{
        public CurrencyTypes type;
        public BigNumber defaultAmount;
        public int defaultUnitsPerObject;
    }

    private void Awake(){
        if(!hasBeenPlaced)InitiateIncrease();
    }

    private void InitiateIncrease(){
        foreach(StorageData data in storageDatas){
            StorageManager.Instance.AddStorageAmount(data.type, data.defaultAmount);
            StorageManager.Instance.AddStorageUnits(data.type, data.defaultUnitsPerObject);
        }
        hasBeenPlaced = true;
    }


}
