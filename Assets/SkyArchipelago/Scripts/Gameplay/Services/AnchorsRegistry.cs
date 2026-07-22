using System.Collections.Generic;
using UnityEngine;

public class AnchorsRegistry
{
    private Dictionary<GameObject, AnchorMB> _mapColliderByEntity = new();

    public void Register(GameObject objectOfCollider, AnchorMB ownerEntity)
    {
        _mapColliderByEntity.TryAdd(objectOfCollider, ownerEntity);
    }

    public void Unregister(GameObject objectOfCollider)
    {
        _mapColliderByEntity.Remove(objectOfCollider);
    }

    public bool TryGetEntityByGOOfTrigger(GameObject goOfTrigger, out AnchorMB entityModel)
    {
        return _mapColliderByEntity.TryGetValue(goOfTrigger, out entityModel);
    }
}