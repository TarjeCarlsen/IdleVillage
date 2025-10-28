using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private RectTransform dragHitBox;
    [SerializeField] private RectTransform dropZone;
    [SerializeField] private Image objectImage;
    private Color originalColor;
    [SerializeField] private Color draggingColor = new Color(0f, 1f, 0f, 1f);
    [SerializeField] private Color overlapColor = new Color(1f, 0f, 0f, 1f);
    [SerializeField] private Color sellColor = new Color(1f, 0.65f, 0f, 1f);
    [SerializeField] private Color overAcceptsWheatColor = new Color(0f, 0.5f, 1f, 1f);  
    private Vector3 startPosition;
    public Action OnDragging;
    public Action OnStopDragging;
    public Action OnPlaced;
    public bool hasBeenPlaced;


    private void Awake(){
        if(objectImage == null)
        {
         objectImage = GetComponent<Image>();
         originalColor = objectImage.color;
        }
        originalColor = objectImage.color;

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        OnDragging?.Invoke();
        if(isPositionValid()){
            objectImage.color = overlapColor;
        }
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OnStopDragging?.Invoke();

        objectImage.color = originalColor;
        
        if (!isPositionValid())
        {
            print("inside not valid");
            transform.position = startPosition;
            objectImage.color = originalColor;
            return;
        }

        if (!hasBeenPlaced)
        {   
            print("Placed");
            hasBeenPlaced = true;
            OnPlaced?.Invoke();
        }
    }



    private bool isPositionValid()
    {
        Rect myRect = GetWorldRect(dragHitBox);
        Draggable[] otherObjects = FindObjectsByType<Draggable>(FindObjectsSortMode.None);
        Rect dropZoneRect = GetWorldRect(dropZone);

        foreach (var other in otherObjects)
        {
            if (other == this) continue;

            RectTransform otherHitBox = other.dragHitBox;
            Rect otherRect = GetWorldRect(otherHitBox);

            if (myRect.Overlaps(otherRect))
            {
                objectImage.color = overlapColor;
                return false;
            }
        }
        if (myRect.Overlaps(dropZoneRect))
        {
            return true;
        }
        return false;
    }
    Rect GetWorldRect(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        Vector2 size = corners[2] - corners[0];
        return new Rect(corners[0], size);
    }
}
