using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using System;
using Microsoft.Unity.VisualStudio.Editor;

public class InventoryHandler : MonoBehaviour
{

    private Dictionary<HouesTypes, int> houseCounter = new();
    [SerializeField] private List<TextAndImage> inv_text_img;

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
        InitializeHousecounter();
    }

    public void OnEnable()
    {
        for (int i = 0; i < inv_text_img.Count; i++)
        {
            HouesTypes type = (HouesTypes)i;
            inv_text_img[i].draggable.OnPlaced += () => RemoveFromInv(type);
        }
    }

    public void OnDisable()
    {
        for (int i = 0; i < inv_text_img.Count; i++)
        {
            HouesTypes type = (HouesTypes)i;
            inv_text_img[i].draggable.OnPlaced -= () => RemoveFromInv(type);
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

    public void RemoveFromInv(HouesTypes type)
    {
        houseCounter[type]--;
        inv_text_img[(int)type].inventoryTxt.text = houseCounter[type].ToString();
        if (houseCounter[type] < 1)
        {
            inv_text_img[(int)type].parentObject.SetActive(false);
        }
    }

}
