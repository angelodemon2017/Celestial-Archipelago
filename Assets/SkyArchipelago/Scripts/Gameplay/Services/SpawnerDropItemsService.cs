using UnityEngine;

public class SpawnerDropItemsService
{
    private readonly DataService _dataService;
    private readonly EntitiesCatalogManager _catalogManager;
    private readonly EntityRuntimeService _entityRuntimeService;
    private readonly WorldShowerService _worldShowerService;

    private ModelConfig _configDropItem;

    public SpawnerDropItemsService(
        DataService dataService,
        EntityRuntimeService entityRuntimeService,
        EntitiesCatalogManager catalogManager,
        WorldShowerService worldShowerService)
    {
        _dataService = dataService;
        _entityRuntimeService = entityRuntimeService;
        _catalogManager = catalogManager;

        if (_catalogManager.TryGetConfigByKey(EEntityType.DroppedItem, out var pare))
            _configDropItem = pare.modelConfig;
        _worldShowerService = worldShowerService;
    }

    public int SpawnDropItem(Vector3 worldPos, EItemType itemType, int count, int idOwner)
    {
        var entData = (DroppedItemData)EntityDataMap.CreateData(EEntityType.DroppedItem);
        entData.IdOwner = idOwner;
        entData.position = worldPos;
        entData.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        entData.TypeItem = itemType;
        entData.Count = count;
        entData.InitConfig(_configDropItem);
        _dataService.worldData.StaticIslands.Datas[0].entities.AddNewData(entData);

        var entModel = _entityRuntimeService.CreateEntityModel(entData);
        _entityRuntimeService.AddModel(entModel);

        _worldShowerService.SpawnViewModelEntity(entModel);

        return entData.Id;
    }
}