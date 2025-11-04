using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpawnDragObject : MonoBehaviour
{
    [Header("Set the parent to be the dropzone of where the spawned item will be collected. or else it wont get collected")]
    [SerializeField] private GameObject objectPrefab;
    [SerializeField] private RectTransform creationHitBox;
    [SerializeField] private RectTransform parentToSpawnUnder;
    private GameObject spawnedObject;
    public bool condition = true;
    public void SetCondition(bool state) => condition = state;

    void Update()
    {
    if (Input.GetMouseButtonDown(0) && IsPointerOverUIHitbox())
    {
        GameObject selected = EventSystem.current.currentSelectedGameObject;
        if (selected != null && selected.transform.IsChildOf(transform))
        {
            SpawnObjectOnMouse();
        }
    }
    }

    public void SpawnObject(){
            GameObject selected = EventSystem.current.currentSelectedGameObject;
            if (selected != null && selected.transform.IsChildOf(transform))
        {
            SpawnObjectOnMouse();
        }
    }

    private void SpawnObjectOnMouse()
    {
        if(!condition) return;
        spawnedObject = Instantiate(objectPrefab, parentToSpawnUnder);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentToSpawnUnder,
            Input.mousePosition,
            null,
            out Vector2 localPoint
        );

        RectTransform objectRect = spawnedObject.GetComponent<RectTransform>();
        objectRect.anchoredPosition = localPoint;

        // Start drag on next frame
        StartCoroutine(DelayedStartDrag());
    }

    private System.Collections.IEnumerator DelayedStartDrag()
    {
        yield return null; // Wait one frame

        if (spawnedObject == null || !Input.GetMouseButton(0))
            yield break;

        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        ExecuteEvents.Execute<IPointerDownHandler>(spawnedObject, eventData, ExecuteEvents.pointerDownHandler);
        ExecuteEvents.Execute<IBeginDragHandler>(spawnedObject, eventData, ExecuteEvents.beginDragHandler);
    }

    private bool IsPointerOverUIHitbox()
    {
        if (creationHitBox == null)
            return false;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            creationHitBox, Input.mousePosition, null, out Vector2 localMousePosition);

        return creationHitBox.rect.Contains(localMousePosition);
    }
}
