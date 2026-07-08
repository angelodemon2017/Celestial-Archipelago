using System.Collections.Generic;

public class ContainersService
{
    private readonly ContainersCatalogConfig _containersCatalog;
    private readonly SimpleFactory<ContainerConfig, ContainerData> _containerDataFactory;
    private readonly SimpleFactory<ContainerData, ContainerModel> _containerModelFactory;
    private readonly DataService _dataService;

    private Dictionary<int, ContainerModel> _mapContainerModels = new();

    public ContainersService(
        ContainersCatalogConfig containersCatalog,
        SimpleFactory<ContainerConfig, ContainerData> containerDataFactory,
        SimpleFactory<ContainerData, ContainerModel> containerModelFactory,
        DataService dataService)
    {
        _containersCatalog = containersCatalog;
        _containerDataFactory = containerDataFactory;
        _containerModelFactory = containerModelFactory;
        _dataService = dataService;
    }

    public ContainerModel GetContainerModel(IHaveContainer entityWithContainer)
    {
        var data = GetContainerData(entityWithContainer);
        return GetContainerModel(data);
    }

    private ContainerData GetContainerData(IHaveContainer entityWithContainer)
    {
        if (entityWithContainer.ContainerId == -1)
        {
            var containerConfig = _containersCatalog.GetContainerConfig(entityWithContainer.GetContainerType);
            var newDataContainer =
                _containerDataFactory.Create(containerConfig);
            var newContainerId = _dataService.AddNewContainer(newDataContainer);
            entityWithContainer.ContainerId = newContainerId;
        }

        return _dataService.GetContainer(entityWithContainer.ContainerId);
    }

    private ContainerModel GetContainerModel(ContainerData containerData)
    {
        if (_mapContainerModels.TryGetValue(containerData.Id, out ContainerModel container))
            return container;

        _mapContainerModels[containerData.Id] = _containerModelFactory.Create(containerData);

        return _mapContainerModels[containerData.Id];
    }

    public ContainerModel GetContainerModelById(int id)
    {
        return _mapContainerModels[id];
    }

    public void DeleteContainer(int containerId)
    {
        if (_mapContainerModels.TryGetValue(containerId, out ContainerModel container))
        {
            _mapContainerModels.Remove(containerId);
            _containerModelFactory.Despawn(container);
        }
    }
}