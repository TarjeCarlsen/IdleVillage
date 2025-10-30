using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class UIZoomViewPort : MonoBehaviour
{
    [Header("References")]
    public ScrollRect scrollRect;             // Auto-assigned if left null
    public RectTransform content;             // Usually scrollRect.content
    public RectTransform viewport;            // Usually scrollRect.viewport (auto if null)

    [Header("Zoom Settings")]
    [Tooltip("Minimum scale for zooming out.")]
    public float minScale = 0.5f;

    [Tooltip("Maximum scale for zooming in.")]
    public float maxScale = 2.5f;

    [Tooltip("How much to scale per mouse wheel notch. 0.1 = 10% per step.")]
    public float zoomSpeed = 0.1f;

    [Tooltip("Keep content inside viewport after zooming.")]
    public bool clampToViewport = true;

    private Camera _eventCamera;

    void Awake()
    {
        if (scrollRect == null)
            scrollRect = GetComponent<ScrollRect>();

        if (content == null && scrollRect != null)
            content = scrollRect.content;

        if (viewport == null && scrollRect != null)
            viewport = scrollRect.viewport;

        var canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
            _eventCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
    }

    void Update()
    {
        var canvasGroup = GetComponentInParent<CanvasGroup>();
        if (canvasGroup != null && (canvasGroup.alpha <= 0f || !canvasGroup.interactable))
        return;
        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) > Mathf.Epsilon)
        {
            float factor = 1f + (scroll * zoomSpeed);
            ZoomAtScreenPoint(Input.mousePosition, factor);
        }
    }

    void ZoomAtScreenPoint(Vector2 screenPoint, float scaleFactor)
    {
        float current = content.localScale.x;
        float target = Mathf.Clamp(current * scaleFactor, minScale, maxScale);
        float actualFactor = target / current;

        if (Mathf.Approximately(actualFactor, 1f))
            return;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(content, screenPoint, _eventCamera, out Vector2 localPoint))
            return;

        Vector3 worldBefore = content.TransformPoint(localPoint);
        content.localScale = new Vector3(target, target, 1f);
        Vector3 worldAfter = content.TransformPoint(localPoint);

        content.position += (worldBefore - worldAfter);

        if (clampToViewport)
            ClampContentToViewport();
    }

    void ClampContentToViewport()
    {
        Bounds contentBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(viewport, content);
        Rect vRect = viewport.rect;
        Vector3 offset = Vector3.zero;

        if (contentBounds.size.x < vRect.width)
            offset.x = vRect.center.x - contentBounds.center.x;
        else
        {
            if (contentBounds.min.x > vRect.xMin) offset.x = vRect.xMin - contentBounds.min.x;
            if (contentBounds.max.x < vRect.xMax) offset.x = vRect.xMax - contentBounds.max.x;
        }

        if (contentBounds.size.y < vRect.height)
            offset.y = vRect.center.y - contentBounds.center.y;
        else
        {
            if (contentBounds.min.y > vRect.yMin) offset.y = vRect.yMin - contentBounds.min.y;
            if (contentBounds.max.y < vRect.yMax) offset.y = vRect.yMax - contentBounds.max.y;
        }

        if (offset.sqrMagnitude > 0f)
            content.position += viewport.TransformVector(offset);
    }
}
