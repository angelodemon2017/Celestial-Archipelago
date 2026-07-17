using UnityEngine;
using UnityEngine.EventSystems;

public class DropDebugReceiver : MonoBehaviour, IDropHandler, IPointerEnterHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("<color=green>=== ON DROP —–¿¡Œ“¿À Õ¿ " + gameObject.name + " ===</color>", gameObject);
        Debug.Log("Dragged object: " + (eventData.pointerDrag != null ? eventData.pointerDrag.name : "null"));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Pointer Enter Ì‡ " + gameObject.name);
    }
}