using UnityEngine;

public class OpenCloseCanvasGroup : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasToShow;
    [SerializeField] private CanvasGroup canvasToHide;
    [SerializeField] private Draggable disableDraggable;


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
}
