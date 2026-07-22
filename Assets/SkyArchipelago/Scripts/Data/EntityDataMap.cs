using System;
using System.Collections.Generic;
using UnityEngine;

public static class EntityDataMap
{
    private static readonly Dictionary<EEntityType, Func<EntityData>> _creators = new();

    private static Dictionary<EEntityType, Queue<EntityData>> _pools = new();

    static EntityDataMap()
    {
        InitTypes();
    }

    static void InitTypes()
    {
        Register<SpawnPointData>();
        Register<DebugLabelData>();
        Register<PlayerData>();
        Register<DemoNPCData>();
        Register<WoodChestData>();
        Register<WorkTableData>();
        Register<FurnaceData>();
        Register<DroppedItemData>();
        Register<SpawnerDropsData>();
        Register<MaquetteOfEntityData>();
        Register<BuildFloorData>();
        Register<BuildWallData>();
    }

    public static void Register<T>()
        where T : EntityData, new()
    {
        var eType = new T().EntityType;
        _creators[eType] = () => new T();
    }

    public static EntityData CreateData(EEntityType entityType)
    {
        if (_creators.Count == 0)
            InitTypes();

        var pool = GetPoolByType(entityType);
        if (pool.Count > 0)
            return pool.Dequeue();

        if (_creators.TryGetValue(entityType, out var creator))
            return creator();

        Debug.LogError($"Неизвестный тип сущности: {entityType}");
        return new EntityData { EntityType = entityType };
    }

    public static void ReturnData(EntityData entityData)
    {
        entityData.ResetData();
        var pool = GetPoolByType(entityData.EntityType);
        pool.Enqueue(entityData);
    }

    private static Queue<EntityData> GetPoolByType(EEntityType entityType)
    {
        if (!_pools.ContainsKey(entityType))
        {
            _pools[entityType] = new();
        }
        return _pools[entityType];
    }

    public static IEnumerable<EEntityType> RegisteredTypes => _creators.Keys;
}