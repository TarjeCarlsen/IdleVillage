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

    private void OnButtonClick(){
        ShowCanvas();
        HideCanvas();
    }


    private void ShowCanvas(){
        if(canvasToShow == null) return;
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
