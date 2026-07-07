using System.Collections.Generic;
using UnityEngine;

public class HitsCoordinatorService
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

    public EntityModel GetEntityByGOOfTrigger(GameObject goOfTrigger)
    {
        if (_mapColliderByEntity.TryGetValue(goOfTrigger, out EntityModel entityModel))
            return entityModel;

        return null;
    }
}