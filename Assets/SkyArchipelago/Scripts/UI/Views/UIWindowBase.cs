using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIWindowBase : MonoBehaviour
{
    [SerializeField] protected CanvasGroup _canvasGroup;

    private void OnValidate()
    {
        if(!_canvasGroup)
            _canvasGroup = GetComponent<CanvasGroup>();
    }

    public virtual void Show()
    {
        _canvasGroup.alpha = 1f;
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        _canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }
}