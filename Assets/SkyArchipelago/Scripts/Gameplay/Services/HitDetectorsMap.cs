using System.Collections.Generic;
using UnityEngine;

public class HitDetectorsMap
{
    private Dictionary<GameObject, EntityModel> _mapColliderByEntity = new();

    public void Register(GameObject objectOfCollider, EntityModel ownerEntity)
    {
        _mapColliderByEntity.TryAdd(objectOfCollider, ownerEntity);
    }

    public void Unregister(GameObject objectOfCollider)
    {
        _mapColliderByEntity.Remove(objectOfCollider);
    }

    public bool TryGetEntityByGOOfTrigger(GameObject goOfTrigger, out EntityModel entityModel)
    {
        return _mapColliderByEntity.TryGetValue(goOfTrigger, out entityModel);
    }
}