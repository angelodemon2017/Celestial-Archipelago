using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

public class GlobalPointerLogger : MonoBehaviour
{
    [Header("Глобальный дебаггер pointer событий")]
    public bool logOnPointerUp = true;
    public bool logOnPointerDown = false;
    public bool logHierarchy = true;
    public bool logRaycastHits = true;

    private GraphicRaycaster graphicRaycaster;
    private EventSystem eventSystem;

    private void Awake()
    {
        // Ищем на этом Canvas'е
        graphicRaycaster = GetComponentInParent<GraphicRaycaster>();
        eventSystem = EventSystem.current;

        if (graphicRaycaster == null)
        {
            Debug.LogWarning("GraphicRaycaster не найден! Повесьте скрипт на Canvas или выше.", this);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0) && logOnPointerUp)
        {
            LogCurrentPointerObject("Mouse Button Up");
        }

        if (Input.GetMouseButtonDown(0) && logOnPointerDown)
        {
            LogCurrentPointerObject("Mouse Button Down");
        }
    }

    private void LogCurrentPointerObject(string trigger)
    {
        if (eventSystem == null || graphicRaycaster == null) return;

        PointerEventData pointerData = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(pointerData, results);

        Debug.Log($"=== {trigger} ===", this);

        if (results.Count > 0)
        {
            for (int i = 0; i < results.Count; i++)
            {
                RaycastResult res = results[i];
                Debug.Log($"  [{i}] Hit: <color=yellow>{res.gameObject.name}</color> | Sorting: {res.sortingOrder} | Depth: {res.depth}",
                          res.gameObject);

                if (logHierarchy)
                {
                    PrintHierarchy(res.gameObject.transform);
                }
            }
        }
        else
        {
            Debug.Log("<color=red>Ничего не найдено под курсором (raycast miss)</color>");
        }
        if (results.Count > 0 && results[0].gameObject.name.Contains("IconItem"))
        {
            var target = results[0].gameObject;
            var cg = target.GetComponentInParent<CanvasGroup>();
            if (cg != null)
            {
                Debug.Log($"<color=orange>CanvasGroup on {cg.name}: blocksRaycasts={cg.blocksRaycasts}, interactable={cg.interactable}</color>", cg);
            }

            var image = target.GetComponent<UnityEngine.UI.Image>();
            if (image != null)
            {
                Debug.Log($"Raycast Target on Image: {image.raycastTarget}");
            }
        }
    }

    private void PrintHierarchy(Transform t)
    {
        string hierarchy = "  Hierarchy: ";
        while (t != null)
        {
            hierarchy += t.name + (t.GetComponent<Canvas>() ? "[Canvas]" : "") + " → ";
            t = t.parent;
        }
        Debug.Log(hierarchy.TrimEnd(' ', '→'));
    }

    [ContextMenu("Log Current Pointer Object Now")]
    public void LogNow()
    {
        LogCurrentPointerObject("Manual Call");
    }
}