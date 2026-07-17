using UnityEngine;
using UnityEngine.EventSystems;

public class UltimateDropDebug : MonoBehaviour,
    IDropHandler,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerEnterHandler,
    IBeginDragHandler,
    IEndDragHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log($"<color=green><b>!!! ON DROP —–¿¡Œ“¿À Õ¿ {gameObject.name} !!!</b></color>", gameObject);
        Debug.Log($"Dragged object: {(eventData.pointerDrag ? eventData.pointerDrag.name : "null")}", gameObject);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log($"PointerDown Ì‡ {gameObject.name}", gameObject);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log($"<color=yellow>PointerUp Ì‡ {gameObject.name}</color>", gameObject);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log($"PointerEnter Ì‡ {gameObject.name}", gameObject);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log($"BeginDrag Ì‡ {gameObject.name}", gameObject);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log($"<color=orange>EndDrag Ì‡ {gameObject.name}</color>", gameObject);
    }
}