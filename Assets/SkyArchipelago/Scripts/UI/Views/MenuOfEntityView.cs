using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MenuOfEntityView : UIWindowBase
{
    [SerializeField] private Transform _parentOfEntityView;
    [SerializeField] private Button _buttonClose;

    [Inject] private MenuOfEntityModel _menuOfEntityModel;

    public Transform ParentOfView => _parentOfEntityView;

    private void Awake()
    {
        _buttonClose.onClick.AddListener(OnClickClose);
    }

    private void OnClickClose()
    {
        _menuOfEntityModel?.OnTryClosed?.Invoke();
    }

    private void OnDestroy()
    {
        _buttonClose.onClick.RemoveAllListeners();
    }
}