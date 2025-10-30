using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private RectTransform dragHitBox;
    [SerializeField] private string dropZoneTag;
    [SerializeField] private bool useFindTag = true;
    [SerializeField] private bool useGetTagInParent = false;
    private RectTransform dropZone;
    [SerializeField] private Image objectImage;
    private Color originalColor;
    [SerializeField] private Color draggingColor = new Color(0f, 1f, 0f, 1f);
    [SerializeField] private Color overlapColor = new Color(1f, 0f, 0f, 1f);
    [SerializeField] private Color sellColor = new Color(1f, 0.65f, 0f, 1f);
    [SerializeField] private Color overAcceptsWheatColor = new Color(0f, 0.5f, 1f, 1f);
    [SerializeField] private bool enableColorsOnDrag = true;
    [SerializeField] private bool staticSnapBack = false;
    [SerializeField] private bool destroyOutsideDropzone;
    [SerializeField] private bool ignoreOverlaps = false;

    [SerializeField] private GameObject gameObjectToDestroy;
    [SerializeField] private bool enableDragOnSpawn = false;
    [SerializeField] public bool draggableEnabled = true;
    private bool isBeingDraggedOnSpawn = false;
    private Vector3 startPosition;
    public event Action OnDragging;
    public event Action OnStopDragging;
    public event Action<Draggable> OnPlaced;
    public bool hasBeenPlaced;


    private void Awake()
    {
        if (objectImage == null && enableColorsOnDrag)
        {
            objectImage = GetComponent<Image>();
            originalColor = objectImage.color;
        }
        else if (enableColorsOnDrag)
            originalColor = objectImage.color;

    }

    private void Start()
    {
        if (useFindTag)
        {
            dropZone = GameObject.FindGameObjectWithTag(dropZoneTag).GetComponent<RectTransform>();
        }
        else if (useGetTagInParent)
        {
            Transform current = transform.parent; // get the parent of the object
            while (current != null)
            {
                if (current != transform) // skip self
                {
                    dropZone = current.GetComponent<RectTransform>();
                    break;
                }
                current = current.parent;
            }
        }
        else
        {
            Debug.LogWarning("Dropzone in draggable not set. Check etither useFindTag or udeGetTagInParent!");
        }

        if (enableDragOnSpawn)
        {
            isBeingDraggedOnSpawn = true;
        }

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!draggableEnabled) return;
        startPosition = transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!draggableEnabled) return;
        // if (!RectTransformUtility.RectangleContainsScreenPoint(dragHitBox, eventData.position, eventData.pressEventCamera))
        // return; // Ignore drags outside the hitbox

        OnDragging?.Invoke();
        if (!isPositionValid() && enableColorsOnDrag)
        {
            objectImage.color = overlapColor;
        }
        else if (enableColorsOnDrag)
        {
            objectImage.color = draggingColor;
        }
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!draggableEnabled) return;
        OnStopDragging?.Invoke();

        if (enableColorsOnDrag) objectImage.color = originalColor;

        if (!isPositionValid())
        {
            if (gameObjectToDestroy != null && destroyOutsideDropzone) Destroy(gameObjectToDestroy);
            transform.position = startPosition;
            if (enableColorsOnDrag) objectImage.color = originalColor;
            return;
        }

        if (!hasBeenPlaced)
        {
            hasBeenPlaced = true;
            OnPlaced?.Invoke(this);
        }
        if (staticSnapBack)
        {
            transform.position = startPosition;
            hasBeenPlaced = false;
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

            if (myRect.Overlaps(otherRect) && !ignoreOverlaps)
            {
                if (enableColorsOnDrag) objectImage.color = overlapColor;
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
    private void Update() // handles dragging on spawn
    {
        if (isBeingDraggedOnSpawn && Input.GetMouseButton(0))
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            OnBeginDrag(eventData);
            OnDrag(eventData);
        }

        if (isBeingDraggedOnSpawn && Input.GetMouseButtonUp(0))
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            OnEndDrag(eventData);
            isBeingDraggedOnSpawn = false;
        }
    }
    public void SetEnableDragOnSpawn(bool value)// handles dragging on spawn
    {
        enableDragOnSpawn = value;
    }

}
