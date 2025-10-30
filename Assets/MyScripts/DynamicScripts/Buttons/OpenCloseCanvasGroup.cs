using UnityEngine;

public class OpenCloseCanvasGroup : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasToShow;
    [SerializeField] private CanvasGroup canvasToHide;
    [SerializeField] private Draggable disableDraggable;

    [SerializeField] private string canvasToHideTag;
    [SerializeField] private string canvasToShowTag;

    private void Awake(){
        if(canvasToHide == null){
            canvasToHide = GameObject.FindGameObjectWithTag(canvasToHideTag).GetComponent<CanvasGroup>();
        }
        if(canvasToShow == null){
            canvasToShow = GameObject.FindGameObjectWithTag(canvasToShowTag).GetComponent<CanvasGroup>();
        }
    }

    public void DisableDragging(){
        disableDraggable.draggableEnabled = false;
    }

    public void EnableDragging(){
        disableDraggable.draggableEnabled = true;
    }
    public void ShowCanvas(){
        canvasToShow.alpha = 1;
        canvasToShow.interactable = true;
        canvasToShow.blocksRaycasts = true;
    }
    public void HideCanvas(){
        canvasToHide.alpha = 0;
        canvasToHide.interactable = false;
        canvasToHide.blocksRaycasts = false;

    }

    public void ShowAndHideBehind(){
        ShowCanvas();
        HideCanvas();
    }
}
