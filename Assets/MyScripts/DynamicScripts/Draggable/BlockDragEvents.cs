using UnityEngine;
using UnityEngine.EventSystems;
public class BlockDragEvents : MonoBehaviour,IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        eventData.Use(); // Stops the event from reaching the parent
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        eventData.Use(); // Stops the drag from reaching parent
    }

    public void OnDrag(PointerEventData eventData)
    {
        eventData.Use(); // Consumes the drag
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        eventData.Use(); // Consumes drag end
    }
}
