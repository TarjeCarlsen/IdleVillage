using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Card/CardData")]
public class CardData : ScriptableObject
{

        public string header_txt;
        public string descriptionText_txt;
        public List<Sprite>content_images;
        public List<CardDataInfo> cardDataInfo;
        public int maxLevel = 10;
        public bool useLevels;

    [System.Serializable]
    public class CardDataInfo{
        public CurrencyTypes type;
        public string price;
        public Sprite sprite;
        public float priceMultiplier;
    }
}
