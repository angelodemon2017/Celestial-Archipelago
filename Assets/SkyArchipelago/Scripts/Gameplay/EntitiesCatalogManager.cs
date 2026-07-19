using System.Collections.Generic;

public class EntitiesCatalogManager :
    BaseCatalogManager<EntityViewCatalog, RootViewHandler, EEntityType>
{
    private Dictionary<int, RootViewHandler> _cacheByUid = new();
    private Dictionary<EEntityType, Dictionary<CtxFlag, ModuleConfig>> _cacheModules = new();

    public EntitiesCatalogManager(EntityViewCatalog catalog) :
        base(catalog)
    {
        int count = _catalog.Elements.Count;
        for (int i = 0; i < count; i++)
            _cacheByUid.Add(_catalog.Elements[i].modelConfig.Uid, _catalog.Elements[i]);
    }

    public bool TryGetModule(EEntityType entityType, CtxFlag ctxFlag, out ModuleConfig module)
    {
        if (!_cacheModules.ContainsKey(entityType))
        {
            _cacheModules[entityType] = new();
            if (TryGetConfigByKey(entityType, out var config))
                for (int i = 0; i < config.modelConfig.ModuleConfigs.Count; i++)
                {
                    var mod = config.modelConfig.ModuleConfigs[i];
                    _cacheModules[entityType][mod.KeyFlag] = mod;
                }
        }
        return _cacheModules[entityType].TryGetValue(ctxFlag, out module);
    }

    public bool TryGetConfigByKey(int key, out RootViewHandler configResult)
    {
        return _cacheByUid.TryGetValue(key, out configResult);
    }
}