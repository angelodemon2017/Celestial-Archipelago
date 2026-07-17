using UnityEngine;
using Zenject;

public class BaseViewOfModelEntity<Tmodel> : BaseViewOfModelEntity
    where Tmodel : EntityModel
{
    [Inject] protected UIMBFactory<ContainerModel, ItemsListViewMB> _factoryContainer;
    [Inject] private FPSCommonModel _fPSCommonModel;
    [Inject] private ContainersService _containersService;

    protected Tmodel _model;
    protected PlayerModel PLM => _fPSCommonModel.LocalPlayerModel;
    protected ContainerModel PlayerContainer => _fPSCommonModel.ContainerModel;

    public sealed override void UpdateView(EntityModel entity)
    {
        if (_model != entity)
            _model = (Tmodel)entity;
        UpdateViewWithSettedModel();
    }

    protected virtual void UpdateViewWithSettedModel()
    {

    }

    protected ItemsListViewMB GetILVMB(IHaveContainer haveContainer, EContainerType eContainer, Transform parent)
    {
        return _factoryContainer.Create(_containersService.GetContainerModel(haveContainer, eContainer), parent);
    }

    public override void OnDespawned()
    {
    }
}

public abstract class BaseViewOfModelEntity : MonoBehaviour
{
    public abstract void UpdateView(EntityModel entity);
    public virtual void MainAction() { }
    public virtual void OnDespawned()
    {
        gameObject.SetActive(false);
    }
}