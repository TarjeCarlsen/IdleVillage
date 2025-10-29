using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpawnDragObject : MonoBehaviour
{
    [SerializeField] private GameObject objectPrefab;
    [SerializeField] private RectTransform creationHitBox;
    [SerializeField] private RectTransform parentToSpawnUnder;

    private GameObject spawnedObject;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && IsPointerOverUIHitbox())
        {
            SpawnObjectOnMouse();
        }
    }

    private void SpawnObjectOnMouse()
    {
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
