using System;
using UnityEngine;

public class CollectObject : MonoBehaviour
{

    private RectTransform dropZoneHitbox;
    [SerializeField] private string dropzoneTag;
   [SerializeField] private Draggable draggable;
    public event Action OnCollect;

    private void Awake()
    {
    Transform current = transform.parent; // get the parent of the object
    while (current != null)
    {
        if (current != transform) // skip self
        {
            dropZoneHitbox = current.GetComponent<RectTransform>();
            break;
        }
        current = current.parent;
    }

    if (dropZoneHitbox == null)
    {
        Debug.LogWarning("No parent RectTransform found", this);
    }
    }

    private void OnEnable()
    {
        if (draggable != null)
        {
            draggable.OnPlaced += TryPlaceObject;
        }
    }

    private void OnDisable()
    {
        if (draggable != null)
        {
            draggable.OnPlaced -= TryPlaceObject;
        }
    }

    private void TryPlaceObject(Draggable placed)
    {
        if (placed != draggable) return;

        Rect spawnObjectRect = GetWorldRect(GetComponent<RectTransform>());
        Rect dropArea = GetWorldRect(dropZoneHitbox);

        if (dropArea.Overlaps(spawnObjectRect))
        {
            Collect();
        }
    }

    private void Collect()
    {
        OnCollect?.Invoke();
        Destroy(gameObject);
    }

    private Rect GetWorldRect(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        Vector2 size = corners[2] - corners[0];
        return new Rect(corners[0], size);
    }
}
