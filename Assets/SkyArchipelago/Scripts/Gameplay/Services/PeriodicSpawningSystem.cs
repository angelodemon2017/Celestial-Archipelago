using System;
using UnityEngine;
using Zenject;

public class PeriodicSpawningSystem : ITimeTickable, IInitializable, IDisposable
{
    private readonly SystemSO _systemSO;
    private readonly IGameTimeService _timeService;
    private readonly EntityRuntimeService _entityRuntimeService;
    private readonly EntitiesCatalogManager _entitiesCatalogManager;
    private readonly SpawnerDropItemsService _spawnerDropItemsService;

    private float _spawnTime;

    public PeriodicSpawningSystem(
        SystemSO systemSO,
        IGameTimeService gameTimeService,
        EntityRuntimeService entityRuntimeService,
        EntitiesCatalogManager entitiesCatalogManager,
        SpawnerDropItemsService spawnerDropItemsService)
    {
        _systemSO = systemSO;
        _timeService = gameTimeService;
        _entityRuntimeService = entityRuntimeService;
        _entitiesCatalogManager = entitiesCatalogManager;
        _spawnerDropItemsService = spawnerDropItemsService;
    }

    public void OnGameTick(float gameDeltaTime)
    {
        _spawnTime += gameDeltaTime;

        while (_spawnTime > _systemSO.globalSpawnTimer)
        {
            _spawnTime -= _systemSO.globalSpawnTimer;
            SpawnTick();
        }
    }

    private void SpawnTick()
    {
        var spawnItems = _entityRuntimeService.GetModelsByEType(EEntityType.SpawnerDrops);
        var count = spawnItems.Count;
        for (int i = 0; i < count; i++)
        {
            if (_entitiesCatalogManager.TryGetModule(spawnItems[i].ConfigId, CtxFlag.ItemSpawner, out var module) &&
                module is SpawnerDropsModuleConfig spawnerDropsModule &&
                spawnItems[i] is SpawnerDropsModel spawnerDrops)
                Check(spawnerDrops, spawnerDropsModule);
        }
    }

    private void Check(SpawnerDropsModel spawnerDrops, SpawnerDropsModuleConfig spawnerDropsModule)
    {
        if (spawnerDrops.CurrentCountDrops >= spawnerDropsModule.MaxDrops)
            return;

        if (UnityEngine.Random.value > spawnerDrops.CurrentChance)
        {
            spawnerDrops.CurrentChance += spawnerDropsModule.IncChanceByFall;
            return;
        }

        var rndCirc = UnityEngine.Random.insideUnitCircle * spawnerDropsModule.Radius;
        var pos = new Vector3(spawnerDrops.Position.x + rndCirc.x, spawnerDrops.Position.y, spawnerDrops.Position.z + rndCirc.y);
        if (!pos.TryGetDownPoint(_systemSO.SpawnRaycastMask, out var totalPoint, out var hit))
        {
            spawnerDrops.Falls++;
            if(spawnerDrops.Falls > 10)
                Debug.LogWarning($"spawnerDrops({spawnerDrops.Id.ToTxt()} have more 10 fall tries");
            return;
        }

        spawnerDrops.CurrentChance = spawnerDropsModule.BaseChance;
        spawnerDrops.Falls = 0;
        pos = totalPoint;
        var rndVar = spawnerDropsModule.ItemVariants.GetRandomWeighted();
        EItemType itemType = rndVar.ItemConfig.TypeItem;
        var count = UnityEngine.Random.Range(rndVar.Min, rndVar.Max);
        var idEntity = _spawnerDropItemsService.SpawnDropItem(pos, itemType, count, spawnerDrops.Id);
        spawnerDrops.OwnItems.Add(idEntity);
        spawnerDrops.CurrentCountDrops++;
    }

    private void CheckBeforeDeleteEntity(EntityModel Entity, int IdEntityOwner)
    {
        if (Entity.EntType == EEntityType.DroppedItem &&
            _entityRuntimeService.TryGetEntityById(IdEntityOwner, out var owner) &&
            owner.EntType == EEntityType.SpawnerDrops &&
            owner is SpawnerDropsModel spawnerDrops &&
            spawnerDrops.OwnItems.Remove(Entity.Id))
                spawnerDrops.CurrentCountDrops--;
    }

    public void Initialize()
    {
        _timeService.Register(this);
        _entityRuntimeService.BeforeDeleteEntity += CheckBeforeDeleteEntity;
    }

    public void Dispose()
    {
        _timeService.Unregister(this);
        _entityRuntimeService.BeforeDeleteEntity -= CheckBeforeDeleteEntity;
    }
}