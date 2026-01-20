using UnityEngine;

public class SetGameobjectActive : MonoBehaviour
{
    [SerializeField] private GameObject objectToSetActive;

    [SerializeField]private GameObject[] objectsToSetInactive;


    public void OnButtonActiveClick(){
        objectToSetActive.SetActive(true);
    }

    public void OnButtonInactiveClick(){
        foreach(GameObject myObject in objectsToSetInactive){
            myObject.SetActive(false);
        }
    }

    public void OnButtonDoBothClick(){
        OnButtonActiveClick();
        OnButtonInactiveClick();
    }

    public void OnButtonToggleActive(){
        if(objectToSetActive.activeSelf == true){
            objectToSetActive.SetActive(false);
        }else{
            objectToSetActive.SetActive(true);
        }
    }
}
