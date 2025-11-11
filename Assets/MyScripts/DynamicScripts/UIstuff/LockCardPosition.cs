using UnityEngine;

public class LockCardPosition : MonoBehaviour
{
    [SerializeField] private string tagForWhereToCenter; // e.g. "FarmingCanvas"
    [SerializeField] private GameObject objectForWhereToCenter;
    [SerializeField] private Vector2 anchoredPosition;   // editable in Inspector (UI units)

    private RectTransform targetCanvas;
    private RectTransform thisRect;

    private void Awake()
    {
        if(objectForWhereToCenter == null){
            var canvasObj = GameObject.FindGameObjectWithTag(tagForWhereToCenter);
            if (canvasObj != null)
                targetCanvas = canvasObj.GetComponent<RectTransform>();
        }else{
            targetCanvas = objectForWhereToCenter.GetComponent<RectTransform>();
        }

        thisRect = GetComponent<RectTransform>();

        if (targetCanvas != null)
            thisRect.SetParent(targetCanvas, false); // reparent to farming canvas but keep UI scale
    }

    private void LateUpdate()
    {
        if (thisRect == null) return;

        // Lock UI anchors (so anchoredPosition works as intended)
        thisRect.anchorMin = new Vector2(0.5f, 0.5f);
        thisRect.anchorMax = new Vector2(0.5f, 0.5f);
        thisRect.pivot = new Vector2(0.5f, 0.5f);

        // Apply manual inspector offset
        thisRect.anchoredPosition = anchoredPosition;
    }
}
