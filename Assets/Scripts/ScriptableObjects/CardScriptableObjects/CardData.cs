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
        public List<CardData> cardDatas;
        public int maxLevel = 10;

    [System.Serializable]
    public class CardDatas{
        public CurrencyTypes type;
        public string price;
        public Sprite sprite;
        public float priceMultiplier;
    }
}
