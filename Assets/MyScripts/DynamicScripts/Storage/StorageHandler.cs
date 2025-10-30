using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor;
using UnityEngine;

public class StorageHandler : MonoBehaviour
{
    [SerializeField] private List<StorageData> storageDatas;

    [System.Serializable]
    public class StorageData{
        public CurrencyTypes type;
        public BigNumber defaultAmount;
        public int defaultUnitsPerObject;
    }

private void Awake(){
    InitiateIncrease();
}

    private void InitiateIncrease(){
        foreach(StorageData data in storageDatas){
            StorageManager.Instance.AddStorageAmount(data.type, data.defaultAmount);
            StorageManager.Instance.AddStorageUnits(data.type, data.defaultUnitsPerObject);
        }
    }

}
