using System.Collections.Generic;
using UnityEngine;

public class InteractableCoordinatorService
{
    private Dictionary<GameObject, InteractHandlerMB> _mapColliderByEntity = new();

    public void Register(GameObject objectOfCollider, InteractHandlerMB ownerEntity)
    {
        _mapColliderByEntity.TryAdd(objectOfCollider, ownerEntity);
    }

    public void Unregister(GameObject objectOfCollider)
    {
        _mapColliderByEntity.Remove(objectOfCollider);
    }

    public InteractHandlerMB GetInteractHandlerMBOrNull(GameObject goOfTrigger)
    {
        if(_mapColliderByEntity.TryGetValue(goOfTrigger, out var result))
            return result;
        return null;
    }
}