using UnityEngine;
using UnityEngine.UI;

public class OpenCloseCanvas : MonoBehaviour
{
    [SerializeField] Canvas canvasToShow;
    private GraphicRaycaster showRaycaster;
    [SerializeField] Canvas canvasToHide;
    private GraphicRaycaster hideRaycaster;

    private void Awake(){
        if(canvasToShow != null){
            showRaycaster = canvasToShow.GetComponent<GraphicRaycaster>();
        }
        if(canvasToHide != null){
            hideRaycaster = canvasToHide.GetComponent<GraphicRaycaster>();
        }
    }

    public void OnButtonClick(){
        ShowCanvas();
        HideCanvas();
    }


    public void ShowCanvas(){
        print("inside show");
        if(canvasToShow == null) return;
        print("inside show SHOW!");
        canvasToShow.gameObject.SetActive(true);
        canvasToShow.enabled = true;
        showRaycaster.enabled = true;
    }
    public void HideCanvas(){
        if(canvasToHide == null) return;
        canvasToHide.enabled = false;
        hideRaycaster.enabled = false;
    }
}
