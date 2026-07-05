using UnityEngine;
using Zenject;

public class InventoryView : UIWindowBase
{
    [SerializeField] private Transform _inventoryParentTransform;

    private UIMBFactory<ContainerModel, ItemsListViewMB> _factoryContainer;
    private FPSCommonModel _fPSCommonModel;

    private ItemsListViewMB _inventoryListView;

    [Inject]
    private void Construct(
        UIMBFactory<ContainerModel, ItemsListViewMB> factoryContainer,
        FPSCommonModel fPSCommonModel)
    {
        _factoryContainer = factoryContainer;
        _fPSCommonModel = fPSCommonModel;
    }

    public override void Show()
    {
        base.Show();
        _inventoryListView = _factoryContainer.Create(_fPSCommonModel.ContainerModel, _inventoryParentTransform);
    }

    public override void Hide()
    {
        base.Hide();
        _factoryContainer.Despawn(_inventoryListView);
    }
}