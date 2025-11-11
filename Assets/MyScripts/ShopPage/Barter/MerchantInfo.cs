using TMPro;
using UnityEngine;

public class MerchantInfo : MonoBehaviour
{
    [SerializeField] private BarterManager barterManager;

    [SerializeField] private TMP_Text lvl_txt;
    [SerializeField] private TMP_Text bonus_txt;
    [SerializeField] private Merchants merchant;


    private void OnEnable(){
        barterManager.OnBarterLevelUp += UpdateBarterInfo;
    }
    private void OnDisable(){
        barterManager.OnBarterLevelUp -= UpdateBarterInfo;
    }

    private void Start(){
        UpdateBarterInfo(merchant);
    }

    public void UpdateBarterInfo(Merchants _merchants){
        if(_merchants != merchant) return;
        lvl_txt.text ="Lv."+ barterManager.merchantInfos[merchant].merchantLevel.ToString();
        bonus_txt.text = $"{barterManager.merchantInfos[merchant].bonus * 100:F0}" + "%";

    }

}
