using TMPro;
using UnityEngine;

public class MerchantInfo : MonoBehaviour
{
    [SerializeField] private BarterManager barterManager;

    [SerializeField] private TMP_Text lvl_txt;
    [SerializeField] private TMP_Text favor_txt;
    [SerializeField] private Merchants merchant;


    private void OnEnable(){
        barterManager.OnBarterLevelUp += UpdateBarterInfo;
        barterManager.OnFavorGained += UpdateFavor;
    }
    private void OnDisable(){
        barterManager.OnBarterLevelUp -= UpdateBarterInfo;
        barterManager.OnFavorGained -= UpdateFavor;
    }
    private void Awake(){
        favor_txt.text = 0.ToString();
    }
    private void Start(){
        UpdateBarterInfo(merchant);
    }

    private void UpdateFavor(Merchants _merchant,int totalFavor){
        if(merchant != _merchant) return;
        favor_txt.text = $"{barterManager.merchantInfos[merchant].favor:F0}";
    }


    public void UpdateBarterInfo(Merchants _merchants){
        if(_merchants != merchant) return;
        lvl_txt.text ="Lv."+ barterManager.merchantInfos[merchant].merchantLevel.ToString();

    }

}
