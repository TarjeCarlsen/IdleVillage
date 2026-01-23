using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class OpenCloseCanvasGroup : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasToShow;
    [SerializeField] private CanvasGroup canvasToHide;
    [SerializeField] private List <CanvasGroup> allCanvasesToHide; 
    [SerializeField] private bool useCanvasList = true;
    [SerializeField] private Draggable disableDraggable;

    [SerializeField] private bool findCanvasFromTag = false;
    [SerializeField] private string canvasToHideTag;
    [SerializeField] private string canvasToShowTag;


    private void Awake(){
    
    if (canvasToHide == null && !useCanvasList)
    {
        GameObject obj = GameObject.FindGameObjectWithTag(canvasToHideTag);
        if (obj != null)
            canvasToHide = obj.GetComponent<CanvasGroup>();
        else
            Debug.LogWarning($"[OpenCloseCanvasGroup] No object found with tag '{canvasToHideTag}'", this);
    }



    if (canvasToShow == null && findCanvasFromTag)
    {

        GameObject obj = GameObject.FindGameObjectWithTag(canvasToShowTag);
        if (obj != null)
            canvasToShow = obj.GetComponent<CanvasGroup>();
        else
            Debug.LogWarning($"[OpenCloseCanvasGroup] No object found with tag '{canvasToShowTag}'", this);
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

    public void HideAllCanvasInList(){
        foreach(CanvasGroup canvas in allCanvasesToHide){
            canvas.alpha = 0;
            canvas.interactable = false;
            canvas.blocksRaycasts = false;
        }
    }

    public void ShowAndHideAll(){
        ShowCanvas();
        HideAllCanvasInList();
    }

    public void ShowAndHideBehind(){
        ShowCanvas();
        HideCanvas();
    }
}
