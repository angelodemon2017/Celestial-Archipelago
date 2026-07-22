using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnerDropItemsService : IDisposable
{
    private readonly SystemSO _systemSO;
    private readonly DataService _dataService;
    private readonly EntityRuntimeService _entityRuntimeService;
    private readonly WorldShowerService _worldShowerService;
    private readonly ContainersService _containersService;

    public SpawnerDropItemsService(
        SystemSO systemSO,
        DataService dataService,
        EntityRuntimeService entityRuntimeService,
        WorldShowerService worldShowerService,
        ContainersService containersService)
    {
        _systemSO = systemSO;
        _dataService = dataService;
        _entityRuntimeService = entityRuntimeService;
        _worldShowerService = worldShowerService;
        _containersService = containersService;

        _containersService.DeletedContainer += OnDeletedContainer;
    }

    private void OnDeletedContainer(ContainerModel containerModel)
    {
        if (!(_entityRuntimeService.TryGetEntityById(containerModel.IdEntityOwner, out var entity)))
            return;

        var worldPos = entity.Position + Vector3.up;
        var list = containerModel.itemModels;
        for (int i = 0; i < containerModel.Slots; i++)
        {
            var itemS = list[i];
            if (itemS.TypeItem != EItemType.None)
                SpawnDropItem(worldPos, itemS.TypeItem, itemS.Count, -1);
        }
    }

    public int SpawnDropItem(Vector3 worldPos, EItemType itemType, int count, int idOwner)
    {
        var entData = (DroppedItemData)EntityDataMap.CreateData(EEntityType.DroppedItem);
        entData.IdOwner = idOwner;
        entData.position = worldPos;
        entData.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        entData.TypeItem = itemType;
        entData.Count = count;
        entData.InitConfig(_systemSO.ConfigDropItem);
        _dataService.worldData.StaticIslands.Datas[0].entities.AddNewData(entData);

        var entModel = _entityRuntimeService.CreateEntityModel(entData);
        _entityRuntimeService.AddModel(entModel);

        _worldShowerService.SpawnViewModelEntity(entModel);

        return entData.Id;
    }

    public void Dispose()
    {
        _containersService.DeletedContainer -= OnDeletedContainer;
    }
}