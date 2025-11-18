using UnityEngine;

public class OpenCloseCards : MonoBehaviour
{
    [SerializeField] GameObject CardObject;
    [SerializeField] CardInfo cardInfo;
    public void OnOpenClick(){

        CardObject.SetActive(true);
    }


}
