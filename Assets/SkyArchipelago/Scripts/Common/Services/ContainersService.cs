using System;
using System.Collections.Generic;

public class ContainersService
{
    private readonly ContainersCatalogManager _containersCatalogManager;
    private readonly SimpleFactory<ContainerConfig, ContainerData> _containerDataFactory;
    private readonly SimpleFactory<ContainerData, ContainerModel> _containerModelFactory;
    private readonly DataService _dataService;

    private List<int> _preparingIds = new();
    private Dictionary<int, ContainerModel> _mapContainerModels = new();

    public Action<ContainerModel> DeletedContainer;

    public ContainersService(
        ContainersCatalogManager containersCatalogManager,
        SimpleFactory<ContainerConfig, ContainerData> containerDataFactory,
        SimpleFactory<ContainerData, ContainerModel> containerModelFactory,
        DataService dataService)
    {
        _containersCatalogManager = containersCatalogManager;
        _containerDataFactory = containerDataFactory;
        _containerModelFactory = containerModelFactory;
        _dataService = dataService;
    }

    public ContainerModel GetContainerModel<T>(T entityWithContainer, EContainerType contType)
        where T : IHaveContainer
    {
        var data = GetContainerData(entityWithContainer, contType);
        return GetContainerModel(data);
    }

    public ContainerModel GetContainerModel<T>(T entityWithContainer)
        where T : IHaveContainer
    {
        var data = GetContainerData(entityWithContainer, entityWithContainer.MainContainer);
        return GetContainerModel(data);
    }

    private ContainerData GetContainerData<T>(T entityWithContainer, EContainerType contType)
        where T : IHaveContainer
    {
        var contId = entityWithContainer.GetIdContainerByEType(contType);
        if (contId == -1)
        {
            _containersCatalogManager.TryGetConfigByKey(contType, out var containerConfig);
            var newDataContainer = _containerDataFactory.Spawn(containerConfig);
            newDataContainer.IdEntityOwner = entityWithContainer.IdEntityOwner;
            contId = _dataService.AddNewContainer(newDataContainer);
            if (!entityWithContainer.SetIdContainerByEType(contType, contId))
            {
                //pool for bed containers?
            }
        }

        return _dataService.GetContainer(contId);
    }

    private ContainerModel GetContainerModel(ContainerData containerData)
    {
        if (_mapContainerModels.TryGetValue(containerData.Id, out ContainerModel container))
            return container;

        _mapContainerModels[containerData.Id] = _containerModelFactory.Spawn(containerData);

        return _mapContainerModels[containerData.Id];
    }

    public ContainerModel GetContainerModelById(int id)
    {
        return _mapContainerModels[id];
    }

    /// <summary>
    /// questionable decision
    /// </summary>
    /// <param name="entity"></param>
    public void DeletedEntity(EntityModel entity)
    {
        if ((entity.AvailableTags & CtxFlag.HaveContainers) != CtxFlag.HaveContainers)
            return;

        if (!(entity is IHaveContainer haveContainer))
            return;

        _preparingIds.Clear();
        var count = haveContainer.GetAllContainersId(_preparingIds);
        for (var i = 0; i < count; i++)
            DeleteContainer(_preparingIds[i]);
    }

    public void DeleteContainer(int containerId)
    {
        if (_mapContainerModels.TryGetValue(containerId, out ContainerModel container))
        {
            DeletedContainer?.Invoke(container);
            _dataService.worldData.ContainerDatas.RemoveData(containerId);
            _containerDataFactory.Despawn(container._dataModel);
            _mapContainerModels.Remove(containerId);
            _containerModelFactory.Despawn(container);
        }
    }
}