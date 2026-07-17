using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class DebugDropLogger : MonoBehaviour,
    // Основные интерфейсы для Drop
    IDropHandler,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerEnterHandler,
    IPointerExitHandler,
    // Дополнительно для полного трейса
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler,
    IPointerClickHandler
{
    [Header("Настройки логирования")]
    [Tooltip("Логировать все события")]
    public bool logAllEvents = true;

    [Tooltip("Логировать только Drop и PointerUp")]
    public bool logOnlyCritical = false;

    [Tooltip("Выводить полную иерархию объекта")]
    public bool logHierarchy = true;

    private static int eventCounter = 0;

    public void OnPointerDown(PointerEventData eventData)
    {
        LogEvent("OnPointerDown", eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        LogEvent("OnPointerUp", eventData, isCritical: true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        LogEvent("OnPointerClick", eventData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        LogEvent("OnPointerEnter", eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LogEvent("OnPointerExit", eventData);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        LogEvent("OnBeginDrag", eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Чтобы не спамить — можно закомментировать
        if (logAllEvents) LogEvent("OnDrag", eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        LogEvent("OnEndDrag", eventData);
    }

    public void OnDrop(PointerEventData eventData)
    {
        LogEvent(">>> OnDrop <<<", eventData, isCritical: true);
    }

    private void LogEvent(string eventName, PointerEventData eventData, bool isCritical = false)
    {
        if (!logAllEvents && !isCritical && logOnlyCritical) return;

        eventCounter++;

        string message = $"[{eventCounter}] {eventName} → {gameObject.name}";

        if (eventData != null && eventData.pointerCurrentRaycast.gameObject != null)
        {
            message += $" | Raycast hit: {eventData.pointerCurrentRaycast.gameObject.name}";
        }

        Debug.Log(message, gameObject);

        if (logHierarchy)
        {
            PrintHierarchy();
        }
    }

    private void PrintHierarchy()
    {
        string hierarchy = "Hierarchy: ";
        Transform t = transform;
        while (t != null)
        {
            hierarchy += t.name + " → ";
            t = t.parent;
        }
        Debug.Log(hierarchy.TrimEnd(' ', '→'), gameObject);
    }

    // Удобный метод для быстрого теста
    [ContextMenu("Log Current CanvasGroup State")]
    private void LogCanvasGroupState()
    {
        CanvasGroup[] groups = GetComponentsInParent<CanvasGroup>(true);
        for (int i = 0; i < groups.Length; i++)
        {
            Debug.Log($"CanvasGroup {groups[i].name}: blocksRaycasts={groups[i].blocksRaycasts}, interactable={groups[i].interactable}", groups[i]);
        }
    }
}