using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EntityViewCatalog", menuName = "Entity/EntityViewCatalog")]
public class EntityViewCatalog : ScriptableObject
{
    public EntityViewMB entityViewPrefab;
    public List<RootViewHandler> rootViewHandlers;

    private Dictionary<EEntityType, RootViewHandler> _cacheItems = new();

    private void OnValidate()
    {
        for (int i = 0; i < rootViewHandlers.Count; i++)
        {
            rootViewHandlers[i].modelConfig.Uid = i;
        }
    }

    public RootViewHandler GetModelConfigByType(EEntityType eEntityType)
    {
        if (!_cacheItems.ContainsKey(eEntityType))
        {
            _cacheItems.Clear();
            rootViewHandlers.ForEach(c => _cacheItems.Add(c.modelConfig.eEntityType, c));
        }

        return _cacheItems[eEntityType];
    }
}

[System.Serializable]
public struct RootViewHandler
{
    public EntityRootHandlerMB entityRootHandlerPrefab;
    public ModelConfig modelConfig;
}