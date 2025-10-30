using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UnlockNode : MonoBehaviour
{
    [SerializeField] private CardInfo cardInfo;
    // [SerializeField] private List<CardInfo> cardFromPreNode;
    [SerializeField] private Image nodeImage;
    private Color originalColor;
    [SerializeField] private GameObject buttonObject; 
    [SerializeField] private bool defaultUnlocked;
    [SerializeField] private List<RequirementData> requirementDatas;

[System.Serializable]
    public class RequirementData{
        public CardInfo cardFromPreNode;
        public int requiredLevel;
    }
    private void Start(){
        originalColor = nodeImage.color;
        if(defaultUnlocked){
            UnlockMyNode();
        }else{
            LockMyNode();
        }
    }
    private void OnEnable(){
        for (int i = 0; i < requirementDatas.Count; i++)
        {
            requirementDatas[i].cardFromPreNode.OnBought += CheckRequirements;
        }
    }
    private void OnDisable(){
        for (int i = 0; i < requirementDatas.Count; i++)
        {
            requirementDatas[i].cardFromPreNode.OnBought -= CheckRequirements;
        }

    }

    private void CheckRequirements(){
        for (int i = 0; i < requirementDatas.Count; i++)
        {
            if(requirementDatas[i].requiredLevel > requirementDatas[i].cardFromPreNode.level ){
                return;
            }
            UnlockMyNode();
        }
    }
    public void UnlockMyNode(){
        buttonObject.SetActive(true);
        nodeImage.color = originalColor;
        for (int i = 0; i < requirementDatas.Count; i++)
        {
            requirementDatas[i].cardFromPreNode.OnBought -= CheckRequirements;
        }
    }

    public void LockMyNode(){
        buttonObject.SetActive(false);
        nodeImage.color = new Color(0f,0f,0f,0.5f);
    }

}
