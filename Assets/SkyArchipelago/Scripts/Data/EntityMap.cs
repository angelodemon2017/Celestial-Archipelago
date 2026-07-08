using System;
using System.Collections.Generic;
using UnityEngine;

public static class EntityMap
{
    private static readonly Dictionary<EEntityType, Func<EntityData>> _creators = new();
    
    static EntityMap()
    {
        InitTypes();
    }

    static void InitTypes()
    {
        Register<SpawnPointData>();
        Register<DebugLabelData>();
        //?!        Register<PlayerData>();
        Register<DemoNPCData>();
        Register<WoodChestData>();
        Register<WorkTableData>();
        //DEMO:
        Register<ResourceEntityData>();
        Register<BuildingEntityData>();
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

        if (_creators.TryGetValue(entityType, out var creator))
            return creator();

        Debug.LogError($"Неизвестный тип сущности: {entityType}");
        return new EntityData { EntityType = entityType };
    }

    public static IEnumerable<EEntityType> RegisteredTypes => _creators.Keys;
}