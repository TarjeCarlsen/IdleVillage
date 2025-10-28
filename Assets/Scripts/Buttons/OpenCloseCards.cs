using UnityEngine;

public class OpenCloseCards : MonoBehaviour
{
    [SerializeField] GameObject CardObject;
    public void OnOpenClick(){
        CardObject.SetActive(true);
    }
}
