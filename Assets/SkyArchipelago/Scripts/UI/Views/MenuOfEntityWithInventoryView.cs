using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MenuOfEntityWithInventoryView : UIWindowBase
{
    [SerializeField] private Transform _myinventoryParentTransform;
    [SerializeField] private Transform _entityInventoryParentTransform;
    [SerializeField] private Button _buttonClose;

    [Inject] private UIMBFactory<ContainerModel, ItemsListViewMB> _factoryContainer;
    [Inject] private MenuEntityWithInventoryModel _menuEntityWithInventoryModel;

    private ItemsListViewMB _myinventoryListView;
    private ItemsListViewMB _entityInventoryListView;

    private void Awake()
    {
        _buttonClose.onClick.AddListener(OnClickClose);
    }

    public override void Show()
    {
        base.Show();
        _myinventoryListView = _factoryContainer.Create(_menuEntityWithInventoryModel.ContainerModelOfPlayer, _myinventoryParentTransform);
        _entityInventoryListView = _factoryContainer.Create(_menuEntityWithInventoryModel.ContainerModelOfEntity, _entityInventoryParentTransform);
    }

    private void OnClickClose()
    {
        _menuEntityWithInventoryModel.OnTryClosed?.Invoke();
    }

    public override void Hide()
    {
        base.Hide();
        _factoryContainer.Despawn(_myinventoryListView);
        _factoryContainer.Despawn(_entityInventoryListView);
    }

    private void OnDestroy()
    {
        _buttonClose.onClick.RemoveAllListeners();
    }
}